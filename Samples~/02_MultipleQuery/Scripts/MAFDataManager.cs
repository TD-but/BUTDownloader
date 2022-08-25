using BUT.Downloader;
using MAF.Presentation.Content;
using MAF.Presentation.Partners;
using UnityEngine;
using UnityEngine.UI;

public class MAFDataManager : MonoBehaviour
{
    [Header("Feed Data References")]
    public Downloader feedDataDownloader;
    public PartnerSO  feedData;

    [Header("Content Data References")]
    public Downloader contentDataDownloader;
    public Downloader contentThumbnailsDownloader;
    public ContentSO  contentData;
    
    [Header("UI References")]
    public Slider progressBar;
    public Text downloadText;

    // Start is called before the first frame update
    void Start()
    {
        feedDataDownloader.DownloadJsonData(() =>
        {
            feedData.ProcessData();
            feedDataDownloader.DownloadAssetData(feedData.GetAssetList());
        });
        
        contentDataDownloader.DownloadJsonData(() =>
        {
            contentData.ProcessData();
            contentDataDownloader.DownloadAssetData(contentData.GetAssetList());
            contentThumbnailsDownloader.DownloadAssetData(contentData.GetThumbnailsList());
        });
    }

    private void OnEnable()
    {
        feedDataDownloader.OnJsonDownloadComplete += JsonDownloadedHandler;
        feedDataDownloader.OnAssetsDownloadComplete += AssetDownloadedHandler;
        feedDataDownloader.DownloadProgress += DownloadProgressHandler;
        contentDataDownloader.DownloadProgress += DownloadProgressHandler;
        contentThumbnailsDownloader.DownloadProgress += DownloadProgressHandler;
    }

    private void OnDisable()
    {
        feedDataDownloader.OnJsonDownloadComplete -= JsonDownloadedHandler;
        feedDataDownloader.OnAssetsDownloadComplete -= AssetDownloadedHandler;
        feedDataDownloader.DownloadProgress -= DownloadProgressHandler;
        contentDataDownloader.DownloadProgress -= DownloadProgressHandler;
        contentThumbnailsDownloader.DownloadProgress -= DownloadProgressHandler;
    }

    [ContextMenu("Download Assets")]
    public void DownloadAssets()
    {
        feedDataDownloader.DownloadAssetData(feedData.GetAssetList());
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
    }

    private void DownloadProgressHandler(float progress)
    {
        progressBar.value += progress/3;
        if(progress >= 1) OnDownloadComplete();
    }
}