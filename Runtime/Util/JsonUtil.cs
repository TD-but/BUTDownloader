using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace BUT.Downloader
{
    public class JsonUtil
    {
        public bool _debugMode;

        public JsonUtil(bool debugMode = false)
        {
            _debugMode = debugMode;
        }
        
        public async Task DownloadData<T>(TextAsset query, string serverURL, string authCode, string filePath,
            T dataObject)
        {
            if (_debugMode) Debug.Log($"<color=yellow> Get Data : </color> {query} ");
            try
            {
                var data = await Http.PostRequest(serverURL, authCode, query.ToString());
                SetData(data, dataObject);
                SaveToFile(filePath, data);
                if (_debugMode) Debug.Log("<color=green> Data Downloaded Successfully </color>");
            }
            catch (Exception e)
            {
                Debug.LogError($"<color=red> Error: </color> {e} ");
                ReadJsonData(filePath, dataObject);
            }
        }


        public async Task DownloadData<T>(TextAsset query, string serverURL, string authCode, string filePath,
            T dataObject, Action onComplete)
        {
            if (_debugMode) Debug.Log($"<color=yellow> Get Data : </color> {query} ");
            try
            {
                var data = await Http.PostRequest(serverURL, authCode, query.ToString());
                SetData(data, dataObject);
                SaveToFile(filePath, data);
                onComplete?.Invoke();
                if (_debugMode) Debug.Log("<color=green> Data Downloaded Successfully </color>");
            }
            catch (Exception e)
            {
                Debug.LogError($"<color=red> Error: </color> {e} ");
                ReadJsonData(filePath, dataObject, onComplete);
            }
        }

        public void ReadJsonData<T>(string filePath, T dataObject, Action onComplete = null)
        {
            if (_debugMode) Debug.Log("<color=yellow> Getting Local Data </color>");
            if (File.Exists(filePath))
            {
                var jsonString = File.ReadAllText(filePath);
                if (_debugMode) Debug.Log($"<color=green> Successfully read file data : </color> {jsonString} ");
                SetData(jsonString, dataObject);
            }
            else
            {
                if (_debugMode) Debug.Log($"<color=red> Could not read message data from file </color> {filePath}");
                return;
            }

            onComplete?.Invoke();
        }

        public void SaveToFile(string filePath, string jsonString)
        {
            using var writer = new StreamWriter(filePath, false);
            writer.Write(jsonString);
        }

        public void SetData<T>(string jsonString, T dataObject)
        {
            JsonUtility.FromJsonOverwrite(jsonString, dataObject);
        }

        public void ToggleDebug(bool debugMode)
        {
            _debugMode = debugMode;
        }
    }
}