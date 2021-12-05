using System.Threading;
using BUT.Downloader;
using UnityEngine;
using UnityEngine.UI;

public class ShowSprite : MonoBehaviour
{
    public JsonChild jsonData;
    public Image     imgComponent;

    private ImageUtil       _imageUtil;
    private Sprite          _sprite;
    CancellationTokenSource tokenSource = new CancellationTokenSource();
    CancellationToken       token;

    private void Awake()
    {
        _imageUtil = new ImageUtil();
        token = tokenSource.Token;
    }

    public async void LoadSprite()
    {
        var imgPath = jsonData.GetAssetList()[0].localPath;
        _sprite = await _imageUtil.LoadSprite(imgPath, token);
        imgComponent.sprite = _sprite;
    }
    
    void OnDestroy () => Dispose();
    public void Dispose ()
    {
        tokenSource.Cancel();
        Destroy(_sprite);          // memory released, leak otherwise
    }
}
