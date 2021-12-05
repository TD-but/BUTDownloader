using BUT.Downloader;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    public Downloader downloader;
    public JsonChild  jsonData;
    public Slider     progressBar;
    public Text       downloadText;
    public RawImage   rawImg;
    public Image      uiImg;
    
    // Start is called before the first frame update
    void Start()
    {
        downloader.DownloadJsonData(() =>
        {
            jsonData.ProcessData();
            //Passing a list of assets to the downloader 
            // downloader.DownloadAssetData(jsonData.GetAssetList());
            //Passing a dictionary of assets to the downloader 
            downloader.DownloadAssetData(jsonData.GetAssetDict());
        });
    }

    private void OnEnable()
    {
        downloader.OnJsonDownloadComplete += JsonDownloadedHandler;
        downloader.OnAssetsDownloadComplete += AssetDownloadedHandler;
        downloader.DownloadProgress += DownloadProgressHandler;
    }

    private void OnDisable()
    {
        downloader.OnJsonDownloadComplete -= JsonDownloadedHandler;
        downloader.OnAssetsDownloadComplete -= AssetDownloadedHandler;
        downloader.DownloadProgress -= DownloadProgressHandler;
    }

    private void JsonDownloadedHandler()
    {
        Debug.Log("Json Downloaded");
    }

    private void AssetDownloadedHandler()
    {
        Debug.Log("Assets Downloaded");
    }

    private void OnDownloadComplete()
    {
        progressBar.gameObject.SetActive(false);
        downloadText.gameObject.SetActive(true);
        rawImg.transform.parent.gameObject.SetActive(true);
        uiImg.transform.parent.gameObject.SetActive(true);
    }

    private void DownloadProgressHandler(float progress)
    {
        progressBar.value = progress;
        if(progress >= 1) OnDownloadComplete();
    }
}
