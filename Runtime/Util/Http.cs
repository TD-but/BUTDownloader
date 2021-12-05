using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace BUT
{
    public static class Http
    {
        public static async Task<T> GetRequest<T>(string url)
        {
            var request = UnityWebRequest.Get(url);
            await request.SendWebRequest();

            if (CheckResultForErrors(request)) throw new Exception($"Https Error {request.error} url:{request.url}");

            var json = request.downloadHandler.text;
            Debug.Log(json);

            return JsonUtility.FromJson<T>(json);
        }

        public static async Task<string> PostRequest(string url, string authCode, string query)
        {
            var data = Encoding.UTF8.GetBytes(query);

            try
            {
                var request = new UnityWebRequest(url, "POST");

                request.SetRequestHeader("CacheControl", "no-cache");
                request.SetRequestHeader("Content-Type", "application/graphql");
                request.SetRequestHeader("Authorization", $"Bearer {authCode}");

                //attach a upload handler (for post) and download handler (for reply) to the WebRequest 
                request.uploadHandler = new UploadHandlerRaw(data);
                request.downloadHandler = new DownloadHandlerBuffer();

                await request.SendWebRequest();

                if (CheckResultForErrors(request))
                {
                    var msg = $"Https Error {request.result} url:{request.url}";
                    request.Dispose();
                    request = null;
                    throw new Exception(msg);
                }

                var json = request.downloadHandler.text;
                Debug.Log($"<color=cyan> Json Data: </color> {json}");
                request.Dispose();
                request = null;
                return json;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static bool CheckResultForErrors(UnityWebRequest request)
        {
            return request.result == UnityWebRequest.Result.ConnectionError ||
                   request.result == UnityWebRequest.Result.ProtocolError ||
                   request.result == UnityWebRequest.Result.DataProcessingError;
        }
    }
}