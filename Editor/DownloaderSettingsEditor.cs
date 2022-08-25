using UnityEditor;
using UnityEngine;

namespace BUT.Downloader
{
    [CustomEditor(typeof(DownloaderSettings))]
    public class DownloaderSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var settings = (DownloaderSettings)target;
            DisplayQuerySettings(settings);
            EditorGUILayout.Space(20);
            DisplayAssetSettings(settings);
            
            EditorUtility.SetDirty(settings);
        }

        private void DisplayQuerySettings(DownloaderSettings settings)
        {
            EditorGUILayout.BeginVertical();
            //Header 
            EditorGUILayout.LabelField("Query Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            // boolean for whether user wants to download JsonData
            settings.downloadJson = EditorGUILayout.Toggle("Download Json", settings.downloadJson);
            if (settings.downloadJson)
            {
                settings.downloadJsonOnStart =
                    EditorGUILayout.Toggle("Download Json On Start", settings.downloadJsonOnStart);
                // Textfield to enter Url of backend API and authorization code 
                settings.apiURL = EditorGUILayout.TextField(
                    new GUIContent("API URL", "Reference to the url with the api used for querying the backend"),
                    settings.apiURL);
                settings.authorizationCode =
                    EditorGUILayout.TextField("Authorization Code", settings.authorizationCode);
                settings.jsonFilePath = EditorGUILayout.TextField(
                    new GUIContent("Json File Path",
                        "Path relative to Streaming Assets where Json data is stored locally"),
                    settings.jsonFilePath);

                // Handle Query Text Asset Reference
                settings.query =
                    (TextAsset)EditorGUILayout.ObjectField(
                        new GUIContent("Query Asset", "Reference to .txt file containing your GraphQL Query"),
                        settings.query, typeof(TextAsset), false);

                // Handle Json Data Reference
                settings.jsonDataObj =
                    (JsonDataModel)EditorGUILayout.ObjectField(
                        new GUIContent("Json Data Model",
                            "Reference to your Data Model Scriptable Object which will hold json data.\n[SHOULD INHERIT FROM JSON DATA MODEL]"),
                        settings.jsonDataObj, typeof(JsonDataModel), false);
            }

            EditorGUILayout.EndVertical();
        }

        private void DisplayAssetSettings(DownloaderSettings settings)
        {
            EditorGUILayout.BeginVertical();
            //Header 
            EditorGUILayout.LabelField("Asset Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            settings.downloadAssets =
                EditorGUILayout.Toggle(
                    new GUIContent("Download Assets",
                        "Whether you want to download assets.\nYou will still have to call the method manually"),
                    settings.downloadAssets);
            if (settings.downloadAssets)
            {
                settings.assetURL = EditorGUILayout.TextField(
                    new GUIContent("Asset URL",
                        "url to server endpoint from where assets can be downloaded"),
                    settings.assetURL);

                settings.downloadImages = EditorGUILayout.Toggle("Download Images", settings.downloadImages);
                if (settings.downloadImages)
                    settings.imagesFileDir = EditorGUILayout.TextField(
                        new GUIContent("Images Directory",
                            "Path to Directory relative to Streaming Assets where images will be saved"),
                        settings.imagesFileDir);

                settings.downloadVideos = EditorGUILayout.Toggle("Download Videos", settings.downloadVideos);
                if (settings.downloadVideos)
                    settings.videosFileDir = EditorGUILayout.TextField(
                        new GUIContent("Videos Directory",
                            "Path to Directory relative to Streaming Assets where videos will be saved"),
                        settings.videosFileDir);
                
                settings.downloadBinary = EditorGUILayout.Toggle("Download Binary", settings.downloadBinary);
                if (settings.downloadBinary)
                    settings.binaryFileDir = EditorGUILayout.TextField(
                        new GUIContent("Binary Files Directory",
                            "Path to Directory relative to Streaming Assets where binary files will be saved"),
                        settings.binaryFileDir);
            }

            EditorGUILayout.EndVertical();
        }
    }
}