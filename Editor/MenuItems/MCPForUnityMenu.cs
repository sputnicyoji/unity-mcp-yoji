using MCPForUnity.Editor.Setup;
using MCPForUnity.Editor.Windows;
using UnityEditor;
using UnityEngine;

namespace MCPForUnity.Editor.MenuItems
{
    public static class MCPForUnityMenu
    {
        [MenuItem("Window/Yoji Unity MCP/Toggle MCP Window %#m", priority = 1)]
        public static void ToggleMCPWindow()
        {
            MCPForUnityEditorWindow.ShowWindow();
        }

        [MenuItem("Window/Yoji Unity MCP/Local Setup Window", priority = 2)]
        public static void ShowSetupWindow()
        {
            SetupWindowService.ShowSetupWindow();
        }


        [MenuItem("Window/Yoji Unity MCP/Edit EditorPrefs", priority = 3)]
        public static void ShowEditorPrefsWindow()
        {
            EditorPrefsWindow.ShowWindow();
        }
    }
}
