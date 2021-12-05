using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace BUT.Downloader
{
    public class Downloader : MonoBehaviour
    {
        
        #region Public Fields

        //------------------------------------------EVENTS---------------------------------------

        //Event invoked when json data has finished downloading
        public event Action OnJsonDownloadComplete;

        //Event invoked when asset data has finished downloading
        public event Action OnAssetsDownloadComplete;

        //Event invoked when an asset is downloaded with float progress value
        public event Action<float> DownloadProgress; 


        //--------------------------------------JSON REFERENCES----------------------------------
        [Header("Json References")] [Tooltip("Scriptable Object containing all download settings")]
        public DownloaderSettings settings;

        #endregion

        #region Private Fields

        //--------------------------------------JSON REFERENCES----------------------------------
        
        //Util class instance to perform json post/get methods
        private readonly JsonUtil _jsonUtil = new JsonUtil();

        //The absolute file path of the json file 
        private string _dataFilePath;

        //--------------------------------------ASSET REFERENCES----------------------------------

        //Util class instance to download and save video with post/get methods
        private readonly VideoUtil _videoUtil = new VideoUtil(true);
        
        //Util class instance to download and save image with post/get methods
        private readonly ImageUtil _imageUtil = new ImageUtil(true);
        
        //The absolute path to the directory where images will be saved 
        private string _imgDirPath;

        //The absolute path to the directory where videos will be saved 
        private string _vidDirPath;

        //List to hold url of images to be downloaded 
        private List<AssetBase> _imagesToDownload;

        //List to hold url of videos to be downloaded 
        private List<AssetBase> _videosToDownload;

        //List to hold url of all images in Json 
        private List<string> _imageNames;

        //List to hold url of all videos in Json 
        private List<string> _videoNames;

        //Count of images to be downloaded 
        private int _imageCount;

        //Count of videos to be downloaded 
        private int _videoCount;

        //Count of images successfully downloaded 
        private int _imagesDownloaded;

        //Count of videos successfully downloaded
        private int _videosDownloaded;

        //Flag to enable/disable Debug logs
        [HideInInspector] public bool debugMode = false;
        
        //Cancellation token for async methods
        CancellationTokenSource tokenSource;
        CancellationToken       token;

        #endregion
        
        #region Init Methods

        //Verifies all public fields in DownloadManager and JsonSettings have been set up properly or throws errors
        private bool VerifyReferences()
        {
            if (settings == null)
            {
                Debug.LogError("<color=red><b> Downloader:" + name + "</b> Settings file not provided </color>");
                return false;
            }

            if (settings.downloadJson)
            {
                if (settings.apiURL == "")
                {
                    Debug.LogError("<color=red><b> Downloader:" + name + "</b> Json API URL not given </color>");
                    return false;
                }

                if (settings.authorizationCode == "")
                {
                    Debug.LogError("<color=red><b> Downloader:" + name + "</b> Authorization Code not given </color>");
                    return false;
                }

                if (settings.jsonFilePath == "")
                {
                    Debug.LogError("<color=red><b> Downloader:" + name + "</b> Json File Path not given </color>");
                    return false;
                }

                if (settings.query == null)
                {
                    Debug.LogError("<color=red><b> Downloader:" + name + "</b> GraphQL query is not provided </color>");
                    return false;
                }

                if (settings.jsonDataObj == null)
                {
                    Debug.LogError("<color=red><b> Downloader:" + name + "</b> Json Data SO is not provided </color>");
                    return false;
                }
            }

            if (settings.downloadAssets)
            {
                if (settings.assetURL == "")
                {
                    Debug.LogError("<color=red><b> Downloader:" + name + "</b> Asset URL not provided </color>");
                    return false;
                }

                if (settings.downloadImages && settings.imagesFileDir == "")
                {
                    Debug.LogError("<color=red><b> Downloader:" + name +
                                   "</b> Image File Directory not provided </color>");
                    return false;
                }

                if (settings.downloadVideos && settings.videosFileDir == "")
                {
                    Debug.LogError("<color=red><b> Downloader:" + name +
                                   "</b> Video File Directory not provided </color>");
                    return false;
                }
            }

            return true;
        }

        //Checks to ensure that all directories/files specified exists on disk else creates them 
        private void VerifyFiles()
        {
            //Check if json file on disk exists or create it  
            if (!File.Exists(_dataFilePath))
            {
                if (_dataFilePath.Contains("/"))
                {
                    var dir = _dataFilePath.Substring(0, _dataFilePath.LastIndexOf('/'));
                    //Debug.Log("<color=purple> Directory: </color>" + dir);
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                }

                File.Create(_dataFilePath).Dispose();
            }

            //Check if image save directory exists else create it 
            if (settings.downloadImages && !Directory.Exists(_imgDirPath))
                Directory.CreateDirectory(_imgDirPath);

            //Check if the video directory has been create else create it 
            if (settings.downloadVideos && !Directory.Exists(_vidDirPath))
                Directory.CreateDirectory(_vidDirPath);
        }

        //Initializes all variables 
        private void Init()
        {
            _imagesToDownload = new List<AssetBase>();
            _videosToDownload = new List<AssetBase>();
            _imageNames = new List<string>();
            _videoNames = new List<string>();
            _imageCount = 0;
            _videoCount = 0;
            _imagesDownloaded = 0;
            _videosDownloaded = 0;
            
            //Set file paths 
            if (settings.downloadJson)
            {
                SetFilePath(settings.jsonFilePath, out _dataFilePath);
                if (debugMode) Debug.Log("<color=yellow> Data File Path </color>" + _dataFilePath);
            }

            if (settings.downloadImages)
            {
                SetFilePath(settings.imagesFileDir, out _imgDirPath);
                if (debugMode) Debug.Log("<color=yellow> Image File Path </color>" + _imgDirPath);
            }

            if (settings.downloadVideos)
            {
                SetFilePath(settings.videosFileDir, out _vidDirPath);
                if (debugMode) Debug.Log("<color=yellow> Video File Path </color>" + _vidDirPath);
            }
        }

        #endregion
        
        #region MonoBehaviour Callbacks

        private void Awake()
        {
            //verify are references are set properly 
            if (!VerifyReferences())
            {
                gameObject.SetActive(false);
                return;
            }

            Init();

            VerifyFiles();
        }

        private void Start()
        {
            if (settings.downloadJson && settings.downloadJsonOnStart)
                DownloadData();
        }

        private void OnDestroy()
        {
            tokenSource?.Cancel();
        }

        #endregion

        #region Public Methods

        //Exposed method to start downloading json data also allows for calling custom method on completion 
        public void DownloadJsonData(Action onComplete = null)
        {
            if(!settings.downloadJson) return;
            DownloadData(onComplete);
        }

        //Exposed method to start downloading assets data. Takes a list of AssetBase as parameter
        public void DownloadAssetData(IEnumerable<AssetBase> assets)
        {
            if(!settings.downloadAssets) return;
            GetAssets(assets);
        }
        
        //Exposed method to start downloading assets data. Takes a Dictionary of AssetBase as parameter
        public void DownloadAssetData<TK,TV>(Dictionary<TK,TV> assets) where TV : AssetBase
        {
            if(!settings.downloadAssets) return;
            GetAssets(assets.Values);
        }

        #endregion
        
        #region Json Methods

        //Uses Json util instance to download data. Calls OnJsonDownloadComplete event
        private async void DownloadData()
        {
            await _jsonUtil.DownloadData(settings.query, settings.apiURL, settings.authorizationCode,
                _dataFilePath, settings.jsonDataObj);
            OnJsonDownloadComplete?.Invoke();
        }

        //Overloaded method that accepts a custom onComplete method. Calls OnJsonDownloadComplete event
        private async void DownloadData(Action onComplete)
        {
            await _jsonUtil.DownloadData(settings.query, settings.apiURL, settings.authorizationCode,
                _dataFilePath, settings.jsonDataObj, onComplete);
            OnJsonDownloadComplete?.Invoke();
        }

        #endregion
        
        #region AssetMethods

        //Gets all images/videos to download and queues it for download 
        private void GetAssets(IEnumerable<AssetBase> assets)
        {
            
            if (debugMode) Debug.Log("<color=lightblue> Total Assets: " + assets.Count() + "</color>");
            foreach (var asset in assets)
            {
                CheckAssetForDownload(asset);
            }

            OnAssetsChecked();
        }

        //Checks if assets doesnt exist on disk or has newer version
        private void CheckAssetForDownload(AssetBase asset)
        {
            string imageName;
            string vidName;
            DateTime localDateUpdated;
            
            if (CheckIfImage(asset.extension))
            {
                imageName = asset.url.Substring(asset.url.LastIndexOf('/') + 1);
                var filepath = Path.Combine(_imgDirPath, imageName);
                if (debugMode)
                    Debug.Log("<color=lightblue> <b>ASSET</b> IMAGE: " + imageName + " | Path: " + filepath +
                              "</color>");

                if (File.Exists(filepath))
                {
                    localDateUpdated = File.GetLastWriteTime(filepath);
                    if (debugMode)
                        Debug.Log("<color=lightblue> <b>DATES</b> LocalDate: " + localDateUpdated +
                                  " | ServerDate: " + asset.dateModified + "</color>");
                    if (localDateUpdated >= asset.dateModified)
                    {
                        if (debugMode)
                            Debug.Log("<color=lightblue> Saving Image Reference: " + imageName + "</color>");
                        _imageNames.Add(imageName);
                        asset.localPath = filepath;
                        if (debugMode)
                            Debug.Log("<Color=#E38B81> -----------------CONTINUE--------------- </color>");
                        return;
                    }
                }

                _imageNames.Add(imageName);
                asset.localPath = filepath;
                _imagesToDownload.Add(asset);
                _imageCount++;
            }
            else if (CheckIfVideo(asset.extension))
            {
                vidName = asset.url.Substring(asset.url.LastIndexOf('/') + 1);
                var filepath = Path.Combine(_vidDirPath, vidName);
                if (debugMode)
                    Debug.Log("<color=lightblue> <b>ASSET</b> VIDEO: " + vidName + " | Path: " + filepath +
                              "</color>");

                if (File.Exists(filepath))
                {
                    localDateUpdated = File.GetLastWriteTime(filepath);
                    if (debugMode)
                        Debug.Log("<color=lightblue> <b>DATES</b> LocalDate: " + localDateUpdated +
                                  " | ServerDate: " + asset.dateModified + "</color>");
                    if (localDateUpdated >= asset.dateModified)
                    {
                        if (debugMode)
                            Debug.Log("<color=lightblue> Saving Video Reference: " + vidName + "</color>");
                        _videoNames.Add(vidName);
                        asset.localPath = filepath;
                        if (debugMode)
                            Debug.Log("<Color=#E38B81> -----------------CONTINUE--------------- </color>");
                        return;
                    }
                }

                _videoNames.Add(vidName);
                asset.localPath = filepath;
                _videosToDownload.Add(asset);
                _videoCount++;
            }
        }
        
        //Once assets are checked either raise the complete event or queue download
        private void OnAssetsChecked()
        {
            if (_imageCount == 0 && _videoCount == 0)
            {
                if (debugMode) Debug.Log("<color=green><b> Finished Downloading All Data </b></color>");
                RaiseCompletionEvents();
            }
            else
            {
                LoadAssets();
                if (debugMode)
                    Debug.Log("<color=orange> Asset Loading has been called and we are continuing operations </color>");
            }
        }

        //Goes through lists of images/videos to download and then calls async functions to download assets and awaits them
        private async void LoadAssets()
        {
            List<Task> downloadTasks = new List<Task>();
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;

            if (_videosToDownload.Any())
            {
                foreach (var vid in _videosToDownload)
                {
                    if (debugMode) Debug.Log("<color=purple> <b> VIDEO </b> ServerPath: " + settings.assetURL + vid.url + "</color>");
                    string req = vid.url.Contains("http") ? vid.url : settings.assetURL + vid.url;
                    downloadTasks.Add(_videoUtil.DownloadVideo(req,_vidDirPath,token,() => {IncrementDownloadedAssetCount(false);}));
                }
            }

            if (_imagesToDownload.Any())
            {
                foreach (var img in _imagesToDownload)
                {
                    if (debugMode) Debug.Log("<color=purple> <b> IMAGE </b> ServerPath: " + settings.assetURL + img.url + "</color>");
                    string req = img.url.Contains("http") ? img.url : settings.assetURL + img.url;
                    downloadTasks.Add(_imageUtil.DownloadImage(req, _imgDirPath, token,() => {IncrementDownloadedAssetCount(true);}));
                }
            }
            
            await Task.WhenAll(downloadTasks);
        }

        //On Asset download completed raises corresponding events
        private void RaiseCompletionEvents()
        {
            OnAssetsDownloadComplete?.Invoke();
            DownloadProgress?.Invoke(1.0f);
            StartCoroutine(DeleteUnusedAssets());
        }
        
        //On Asset download complete increments count of assets downloaded and raises DownloadProgress event
        private void IncrementDownloadedAssetCount(bool isImage)
        {
            if (isImage) _imagesDownloaded++;
            else _videosDownloaded++;
        
            float totalDownload =  _imagesDownloaded + (_videosDownloaded*5);
            float totalAssets = _imageCount + (_videoCount*5);
            float progress = totalDownload / totalAssets;
            if(debugMode) Debug.Log("<color=orange> DOWNLOAD PROGRESS: " + totalAssets + " | " + totalDownload +" | " + progress +" </color>");
            DownloadProgress?.Invoke(progress);
            if(progress >= 1) RaiseCompletionEvents();
        }

        //Co-routine to delete assets on disk but not in json anymore 
        private IEnumerator DeleteUnusedAssets()
        {
            if (_imageNames.Count > 0)
            {
                var savedImages = Directory.GetFiles(_imgDirPath);
                foreach (var file in savedImages)
                    if (!_imageNames.Contains(Path.GetFileName(file)))
                    {
                        if (debugMode) Debug.Log("<color=red> Image to Delete:  </color>" + file);
                        File.Delete(file);
                    }
            }

            if (_videoNames.Count > 0)
            {
                var savedVideos = Directory.GetFiles(_vidDirPath);
                foreach (var file in savedVideos)
                    if (!_videoNames.Contains(Path.GetFileName(file)))
                    {
                        if (debugMode) Debug.Log("<color=red> Video to Delete:  </color>" + file);
                        File.Delete(file);
                    }
            }

            yield break;
        }

        #endregion
        
        #region Util Methods

        private void SetFilePath(string path, out string pathVar)
        {
            if (path[0] == '/')
                pathVar = Application.streamingAssetsPath + path;
            else
                pathVar = Application.streamingAssetsPath + "/" + path;
        }

        private bool CheckIfImage(string ext)
        {
            switch (ext.ToLower())
            {
                case "jpg":
                case "png":
                case "jpeg":
                    return true;
            }

            return false;
        }
        
        private bool CheckIfVideo(string ext)
        {
            switch (ext.ToLower())
            {
                case "mp4":
                case "mov":
                case "avi":
                case "webm":
                case "mkv":
                    return true;
            }

            return false;
        }
        
        public void ToggleDebug()
        {
            debugMode = !debugMode;
            _jsonUtil.ToggleDebug(debugMode);
            _imageUtil.ToggleDebug(debugMode);
            _videoUtil.ToggleDebug(debugMode);
        }

        #endregion
    }
}