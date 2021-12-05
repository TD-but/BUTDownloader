using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace BUT.Downloader
{
    public class VideoUtil
    {
        public bool _debugMode;

        public VideoUtil(bool debugMode = false)
        {
            _debugMode = debugMode;
        }
        
        public async Task DownloadVideo(string videoUrl, string dir, CancellationToken cToken)
        {
            string videoName = videoUrl.Substring(videoUrl.LastIndexOf('/') + 1);
            string saveDir = Path.Combine(dir, videoName);

            if (_debugMode) Debug.Log("<color=yellow> Called Download Video With: </color>" + videoUrl);

            try
            {
                using (UnityWebRequest www = UnityWebRequest.Get(videoUrl))
                {
                    // begin request:
                    var asyncOp = www.SendWebRequest();
                    // await until it's done: 
                    while (asyncOp.isDone == false)
                    {
                        cToken.ThrowIfCancellationRequested();
                        await Task.Delay(1000 / 30); //30 hertz   
                    }

                    // read results:
                    if (Http.CheckResultForErrors(www))
                    {
                        Debug.Log("<color=red>" + www.error + "  \n" + videoUrl + "</color>");
                        return; //EXIT if error
                    }

                    // return valid results:
                    if (_debugMode) Debug.Log("<color=green> Successfully downloaded Video: </color>" + videoName);
                    byte[] vidData = www.downloadHandler.data;
                    var vidTask = Task.Run(() => { SaveVideoAsync(saveDir, vidData); }, cToken);
                    await vidTask;
                }
            }catch(Exception e) { Debug.Log("<color=red> VIDEO DOWNLOAD CANCELLED: </color>" + videoUrl + "\n" + e); }
        }
        
        public async Task DownloadVideo(string videoUrl, string dir, CancellationToken cToken, Action onComplete)
        {
            string videoName = videoUrl.Substring(videoUrl.LastIndexOf('/') + 1);
            string saveDir = Path.Combine(dir, videoName);
            
            if (_debugMode) Debug.Log("<color=yellow> Called Download Video With: </color>" + videoUrl);
            
            try{
                using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(videoUrl))
                {
                    // begin request:
                    var asyncOp = www.SendWebRequest();
                    // await until it's done: 
                    while (asyncOp.isDone == false)
                    {
                        cToken.ThrowIfCancellationRequested();
                        await Task.Delay(1000 / 30); //30 hertz
                    }

                    // read results:
                    if (Http.CheckResultForErrors(www))
                    {
                        Debug.Log("<color=red>" + www.error + "  \n" + videoUrl + "</color>");
                        return; //EXIT if error
                    }

                    // return valid results:
                    if (_debugMode) Debug.Log("<color=green> Successfully downloaded Video: </color>" + videoName);
                    byte[] vidData = www.downloadHandler.data;
                    var vidTask = Task.Run(() => { SaveVideoAsync(saveDir, vidData); }, cToken);
                    await vidTask;
                    onComplete?.Invoke();
                }
            }catch(Exception e) { Debug.Log("<color=red> VIDEO DOWNLOAD CANCELLED: </color>" + videoUrl + "\n" + e); }
        }

        //Saves video byte array to path provided
        public void SaveVideo(string path, byte[] vid)
        {
            File.WriteAllBytes(path, vid);
        }
        
        //Saves video byte array to path provided and invokes event on complete
        public void SaveVideo(string path, byte[] vid, Action onComplete)
        {
            File.WriteAllBytes(path, vid);
            onComplete?.Invoke();
        }
        
        //Saves video byte array to path provided Asynchronously
        public async Task SaveVideoAsync(string path, byte[] vid)
        {
            using (FileStream sourceStream = File.Open(path, FileMode.OpenOrCreate))
            {
                await sourceStream.WriteAsync(vid, 0, vid.Length);
            }
        }
        
        //Saves video byte array to path provided Asynchronously and invokes event on complete
        public async Task SaveVideoAsync(string path, byte[] vid, Action onComplete)
        {
            using (FileStream sourceStream = File.Open(path, FileMode.OpenOrCreate))
            {
                await sourceStream.WriteAsync(vid, 0, vid.Length);
                onComplete?.Invoke();
            }
        }
        
        public void ToggleDebug(bool debugMode)
        {
            _debugMode = debugMode;
        }
    }
}
