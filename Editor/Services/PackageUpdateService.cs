using System;
using System.Linq;
using MCPForUnity.Editor.Constants;
using UnityEditor;

namespace MCPForUnity.Editor.Services
{
    /// <summary>
    /// Personal fork: update checks are disabled to avoid unsolicited outbound requests.
    /// </summary>
    public class PackageUpdateService : IPackageUpdateService
    {
        public UpdateCheckResult CheckForUpdate(string currentVersion) => NoUpdate(currentVersion);

        public UpdateCheckResult TryGetCachedResult(string currentVersion) => NoUpdate(currentVersion);

        public UpdateCheckResult FetchAndCompare(string currentVersion) => NoUpdate(currentVersion);

        public UpdateCheckResult FetchAndCompare(string currentVersion, bool isGitInstallation, string gitBranch) => NoUpdate(currentVersion);

        public void CacheFetchResult(string currentVersion, string fetchedVersion) { }

        public bool IsGitInstallation() => true;

        public string GetGitUpdateBranch(string currentVersion) => "main";

        public void ClearCache()
        {
            EditorPrefs.DeleteKey(EditorPrefKeys.LastUpdateCheck);
            EditorPrefs.DeleteKey(EditorPrefKeys.LatestKnownVersion);
            EditorPrefs.DeleteKey(EditorPrefKeys.LastAssetStoreUpdateCheck);
            EditorPrefs.DeleteKey(EditorPrefKeys.LatestKnownAssetStoreVersion);
        }

        public bool IsNewerVersion(string version1, string version2)
        {
            if (string.IsNullOrWhiteSpace(version1) || string.IsNullOrWhiteSpace(version2))
                return false;

            int[] left = ParseVersion(version1);
            int[] right = ParseVersion(version2);
            int count = Math.Max(left.Length, right.Length);
            for (int i = 0; i < count; i++)
            {
                int a = i < left.Length ? left[i] : 0;
                int b = i < right.Length ? right[i] : 0;
                if (a > b) return true;
                if (a < b) return false;
            }
            return false;
        }

        private static UpdateCheckResult NoUpdate(string currentVersion)
        {
            return new UpdateCheckResult
            {
                CheckSucceeded = true,
                UpdateAvailable = false,
                LatestVersion = currentVersion,
                Message = "Update checks are disabled in this personal fork."
            };
        }

        private static int[] ParseVersion(string version)
        {
            string core = version.Split('-')[0];
            return core.Split('.')
                .Select(part => int.TryParse(part, out int value) ? value : 0)
                .ToArray();
        }
    }
}
