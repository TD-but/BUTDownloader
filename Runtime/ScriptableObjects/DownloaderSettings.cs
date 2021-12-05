using UnityEngine;

namespace BUT.Downloader
{
    [CreateAssetMenu(fileName = "Downloader Settings", menuName = "Downloader/Create Downloader Settings", order = 0)]
    public class DownloaderSettings : ScriptableObject
    {
        public bool          downloadJson;
        public bool          downloadJsonOnStart;
        public string        apiURL;
        public string        authorizationCode;
        public string        jsonFilePath;
        public TextAsset     query;
        public JsonDataModel jsonDataObj;


        public bool   downloadAssets;
        public string assetURL;
        public bool   downloadImages;
        public string imagesFileDir;
        public bool   downloadVideos;
        public string videosFileDir;
    }
}