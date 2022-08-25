using UnityEditor;
using UnityEngine;

namespace BUT.Downloader
{
    [CustomEditor(typeof(Downloader))]
    [CanEditMultipleObjects]
    public class DownloaderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var _downloader = (Downloader)target;

            // Handle Downloader Settings Reference
            GUILayout.BeginHorizontal();

            _downloader.settings =
                (DownloaderSettings)EditorGUILayout.ObjectField("Downloader Settings", _downloader.settings,
                    typeof(DownloaderSettings), false);

            GUI.enabled = !_downloader.settings;
            if (GUILayout.Button("New")) _downloader.settings = CreateDownloaderSettings();

            GUI.enabled = true;
            GUILayout.EndHorizontal();


            // Handle Json Data Reference
            var btnTitle = _downloader.debugMode ? "Disable Debug Mode" : "Enable Debug Mode";
            if (GUILayout.Button(btnTitle)) _downloader.ToggleDebug();
        }


        private DownloaderSettings CreateDownloaderSettings()
        {
            var asset = CreateInstance<DownloaderSettings>();

            var name = AssetDatabase.GenerateUniqueAssetPath("Assets/Settings.asset");
            AssetDatabase.CreateAsset(asset, name);
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
            return asset;
        }
    }
}