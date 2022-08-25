using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace BUT.Downloader
{
    public class ImageUtil
    {

        public bool _debugMode;

        public ImageUtil(bool debugMode = false)
        {
            _debugMode = debugMode;
        }
        
        public async Task DownloadImage(string imageUrl, string dir,  CancellationToken cToken)
        {
            string imageName = imageUrl.Substring(imageUrl.LastIndexOf('/') + 1);
            string saveDir = Path.Combine(dir, imageName);
            
            if (_debugMode) Debug.Log("<color=yellow> Called Download Image With: </color>" + imageUrl);

            try
            {
                using (UnityWebRequest www = UnityWebRequest.Get(imageUrl))
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
                    if (www.isNetworkError || www.isHttpError)
                    {
                        Debug.Log("<color=red>" + www.error + "  \n" + imageUrl + "</color>");
                        return; //EXIT if error
                    }

                    // return valid results:
                    if (_debugMode) Debug.Log("<color=green> Successfully downloaded Image: </color>" + imageName);
                    byte[] imgData = www.downloadHandler.data;
                    var imgTask = Task.Run(() => { SaveImageAsync(saveDir, imgData); }, cToken);
                    await imgTask;
                }
            }catch(Exception e) { Debug.Log("<color=red> IMAGE DOWNLOAD CANCELLED: </color>" + imageUrl); }
        }
        
        public async Task DownloadImage(string imageUrl, string dir, CancellationToken cToken, Action onComplete)
        {
            string imageName = imageUrl.Substring(imageUrl.LastIndexOf('/') + 1);
            string saveDir = Path.Combine(dir, imageName);
            
            if (_debugMode) Debug.Log("<color=yellow> Called Download Image With: </color>" + imageUrl);

            try
            {
                using (UnityWebRequest www = UnityWebRequest.Get(imageUrl))
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
                    if (www.isNetworkError || www.isHttpError)
                    {
                        Debug.Log("<color=red>" + www.error + "  \n" + imageUrl + "</color>");
                        return; //EXIT if error
                    }

                    // return valid results:
                    if (_debugMode) Debug.Log("<color=green> Successfully downloaded Image: </color>" + imageName);
                    byte[] imgData = www.downloadHandler.data;
                    var imgTask = Task.Run(() => { SaveImageAsync(saveDir, imgData); }, cToken);
                    await imgTask;
                    onComplete?.Invoke();
                }
            }catch(Exception e) { Debug.Log("<color=red> IMAGE DOWNLOAD CANCELLED: </color>" + imageUrl); }
        }

        //Saves image byte array to path provided
        public void SaveImage(string path, byte[] vid)
        {
            File.WriteAllBytes(path, vid);
        }
        
        //Saves image byte array to path provided and invokes event on complete
        public void SaveImage(string path, byte[] vid, Action onComplete)
        {
            File.WriteAllBytes(path, vid);
            onComplete?.Invoke();
        }
        
        //Saves image byte array to path provided Asynchronously
        public async Task SaveImageAsync(string path, byte[] vid)
        {
            using (FileStream sourceStream = File.Open(path, FileMode.OpenOrCreate))
            {
                await sourceStream.WriteAsync(vid, 0, vid.Length);
            }
        }
        
        //Saves image byte array to path provided Asynchronously and invokes event on complete
        public async Task SaveImageAsync(string path, byte[] vid, Action onComplete)
        {
            using (FileStream sourceStream = File.Open(path, FileMode.OpenOrCreate))
            {
                await sourceStream.WriteAsync(vid, 0, vid.Length);
                onComplete?.Invoke();
            }
        }
        
        //Reads Image from file Asynchronously and returns Texture2D
        public async Task<Texture2D> LoadTexture2D (string filePath, CancellationToken cToken)
        {
            try
            {
                using( UnityWebRequest www = UnityWebRequestTexture.GetTexture(filePath) )
                {
                    // begin request:
                    var asyncOp = www.SendWebRequest();
                    // await until it's done: 
                    while (asyncOp.isDone == false)
                    {
                        cToken.ThrowIfCancellationRequested();
                        await Task.Yield(); //30 hertz
                    }

                    // read results:
                    if( www.isNetworkError || www.isHttpError )
                    {
                        // log error:
                        Debug.Log( $"<color=red> GET LOCAL TEXTURE FAILED: </color> {www.error}, URL:{www.url}" );
                        // nothing to return on error:
                        return null;
                    }
                    // return valid results:
                    var tex = DownloadHandlerTexture.GetContent(www);
                    return tex;
                }
            } catch(Exception e) 
            { 
                Debug.Log("<color=red> IMAGE LOAD TO TEX2D CANCELLED: </color>" + filePath + e);
                return null;
            }
        }
        
        //Reads Image from file Asynchronously and returns Sprite
        public async Task<Sprite> LoadSprite(string filePath, CancellationToken cToken)
        {
            try
            {
                using( UnityWebRequest www = UnityWebRequestTexture.GetTexture(filePath) )
                {
                    // begin request:
                    var asyncOp = www.SendWebRequest();
                    // await until it's done: 
                    while (asyncOp.isDone == false)
                    {
                        cToken.ThrowIfCancellationRequested();
                        await Task.Yield(); //30 hertz
                    }

                    // read results:
                    if( www.isNetworkError || www.isHttpError )
                    {
                        // log error:
                        Debug.Log( $"<color=red> GET LOCAL TEXTURE FAILED: </color> {www.error}, URL:{www.url}" );
                        // nothing to return on error:
                        return null;
                    }
                    // return valid results:
                    var tex = DownloadHandlerTexture.GetContent(www);
                    Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                    return sprite;
                }
            } catch(Exception e) 
            { 
                Debug.Log("<color=red> IMAGE LOAD TO SPRITE CANCELLED: </color>" + filePath);
                return null;
            }
        }
        
        public void ToggleDebug(bool debugMode)
        {
            _debugMode = debugMode;
        }
    }
}
