using System.Collections.Generic;
using MCPForUnity.Editor.Clients.Configurators;

namespace MCPForUnity.Editor.Clients
{
    /// <summary>
    /// Personal fork registry. Only Claude Code and Codex are supported.
    /// </summary>
    public static class McpClientRegistry
    {
        private static readonly IReadOnlyList<IMcpClientConfigurator> SupportedClients =
            new List<IMcpClientConfigurator>
            {
                new ClaudeCodeConfigurator(),
                new CodexConfigurator(),
            };

        public static IReadOnlyList<IMcpClientConfigurator> All => SupportedClients;
    }
}
