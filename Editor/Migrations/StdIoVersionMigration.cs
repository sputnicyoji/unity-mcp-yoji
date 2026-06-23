using System;
using System.IO;
using System.Linq;
using MCPForUnity.Editor.Clients;
using MCPForUnity.Editor.Constants;
using MCPForUnity.Editor.Helpers;
using MCPForUnity.Editor.Models;
using MCPForUnity.Editor.Services;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace MCPForUnity.Editor.Migrations
{
    /// <summary>
    /// Keeps stdio MCP clients in sync with the current package version by rewriting their configs when the package updates.
    /// </summary>
    [InitializeOnLoad]
    internal static class StdIoVersionMigration
    {
        private const string c_LastUpgradeKey = EditorPrefKeys.LastStdIoUpgradeVersion;
        private const string c_MigrationMarkerSuffix = "codex-stdio-toml-v1";

        static StdIoVersionMigration()
        {
            if (Application.isBatchMode)
                return;

            EditorApplication.delayCall += RunMigrationIfNeeded;
        }

        private static void RunMigrationIfNeeded()
        {
            EditorApplication.delayCall -= RunMigrationIfNeeded;

            string currentVersion = AssetPathUtility.GetPackageVersion();
            if (string.IsNullOrEmpty(currentVersion) || string.Equals(currentVersion, "unknown", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string lastUpgradeVersion = string.Empty;
            string migrationMarker = $"{currentVersion}:{c_MigrationMarkerSuffix}";
            try { lastUpgradeVersion = EditorPrefs.GetString(c_LastUpgradeKey, string.Empty); } catch { }

            if (string.Equals(lastUpgradeVersion, migrationMarker, StringComparison.OrdinalIgnoreCase))
            {
                return; // Already refreshed for this package version
            }

            bool hadFailures = false;
            bool touchedAny = false;

            var configurators = McpClientRegistry.All.OfType<McpClientConfiguratorBase>().ToList();
            foreach (var configurator in configurators)
            {
                try
                {
                    if (!configurator.SupportsAutoConfigure)
                        continue;

                    // Handle CLI-based configurators (e.g., Claude Code CLI)
                    // CheckStatus with attemptAutoRewrite=true will auto-reregister if version mismatch
                    if (configurator is ClaudeCliMcpConfigurator)
                    {
                        var previousStatus = configurator.Status;
                        configurator.CheckStatus(attemptAutoRewrite: true);
                        if (configurator.Status != previousStatus)
                        {
                            touchedAny = true;
                        }
                        continue;
                    }

                    // Handle file-based stdio configurators.
                    if (!ConfigUsesStdIo(configurator))
                        continue;

                    // Skip clients that don't support the current transport setting.
                    // Configure() would throw for incompatible legacy JSON clients.
                    bool useHttp = EditorConfigurationCache.Instance.UseHttpTransport;
                    if (useHttp && !configurator.Client.SupportsHttpTransport)
                        continue;

                    MCPServiceLocator.Client.ConfigureClient(configurator);
                    touchedAny = true;
                }
                catch (Exception ex)
                {
                    hadFailures = true;
                    McpLog.Warn($"Failed to refresh stdio config for {configurator.DisplayName}: {ex.Message}");
                }
            }

            if (hadFailures)
            {
                McpLog.Warn("Stdio MCP upgrade encountered errors; will retry next session.");
                return;
            }

            if (!touchedAny)
            {
                // Nothing needed refreshing; still record version so we don't rerun every launch
                try { EditorPrefs.SetString(c_LastUpgradeKey, migrationMarker); } catch { }
                return;
            }

            try
            {
                EditorPrefs.SetString(c_LastUpgradeKey, migrationMarker);
            }
            catch { }

            McpLog.Info($"Updated stdio MCP configs to package version {currentVersion}.");
        }

        private static bool ConfigUsesStdIo(McpClientConfiguratorBase configurator)
        {
            if (configurator is CodexMcpConfigurator)
            {
                return CodexConfigUsesStdIo(configurator.Client);
            }

            return JsonConfigUsesStdIo(configurator.Client);
        }

        private static bool CodexConfigUsesStdIo(McpClient client)
        {
            string configPath = McpConfigurationHelper.GetClientConfigPath(client);
            if (string.IsNullOrEmpty(configPath) || !File.Exists(configPath))
            {
                return false;
            }

            try
            {
                string toml = File.ReadAllText(configPath);
                return CodexConfigHelper.TryParseCodexServer(toml, out _, out var args, out var url)
                       && string.IsNullOrEmpty(url)
                       && args != null
                       && args.Length > 0;
            }
            catch
            {
                return false;
            }
        }

        private static bool JsonConfigUsesStdIo(McpClient client)
        {
            string configPath = McpConfigurationHelper.GetClientConfigPath(client);
            if (string.IsNullOrEmpty(configPath) || !File.Exists(configPath))
            {
                return false;
            }

            try
            {
                var root = JObject.Parse(File.ReadAllText(configPath));

                JToken unityNode = null;
                if (client.UsesServersLayout)
                {
                    unityNode = root.SelectToken("servers.unityMCP")
                               ?? root.SelectToken("mcp.servers.unityMCP");
                }
                else
                {
                    unityNode = root.SelectToken("mcpServers.unityMCP");
                }

                if (unityNode == null) return false;

                return unityNode["command"] != null;
            }
            catch
            {
                return false;
            }
        }

    }
}
