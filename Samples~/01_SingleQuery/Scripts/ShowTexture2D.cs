using System.Threading;
using BUT.Downloader;
using UnityEngine;
using UnityEngine.UI;

public class ShowTexture2D : MonoBehaviour
{
    public JsonChild jsonData;
    public RawImage  rawImgComponent;

    private ImageUtil       _imageUtil;
    private Texture2D       _tex;
    CancellationTokenSource tokenSource = new CancellationTokenSource();
    CancellationToken       token;

    private void Awake()
    {
        _imageUtil = new ImageUtil();
        token = tokenSource.Token;
    }

    public async void LoadTexture()
    {
        var imgPath = jsonData.GetAssetList()[0].localPath;
        _tex = await _imageUtil.LoadTexture2D(imgPath, token);
        rawImgComponent.texture = _tex;
    }
    
    void OnDestroy () => Dispose();
    public void Dispose ()
    {
        tokenSource.Cancel();
        Destroy(_tex);          // memory released, leak otherwise
    }
}
