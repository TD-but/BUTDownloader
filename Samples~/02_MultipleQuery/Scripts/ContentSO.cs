using System;
using System.Collections.Generic;
using System.Linq;
using BUT.Downloader;
using UnityEngine;

namespace MAF.Presentation.Content
{
    [CreateAssetMenu(fileName = "Content SO", menuName = "Downloader/Create Content Catalog DataObject", order = 0)]
    public class ContentSO : JsonDataModel
    {
        public Data data;

        [SerializeField] private List<Partners.Asset> _assets;
        [SerializeField] private List<Partners.Asset> _assetThumbnails;
        
        private Dictionary<int, Partners.Asset> _assetsDict;

        public void ProcessData()
        {
            _assetsDict = new Dictionary<int, Partners.Asset>();
            _assets = new List<Partners.Asset>();
            _assetThumbnails = new List<Partners.Asset>();
            var assetList = data.assetList;
            for (int i = 0; i < assetList.Count; i++)
            {
                if (_assetsDict.ContainsKey(assetList[i].id)) continue;
                // Process Asset 
                _assetsDict.Add(assetList[i].id, assetList[i]);
                _assets.Add(assetList[i]);
                // Process Asset Thumbnail 
                if (assetList[i].kind != "image")
                {
                    var thumbnail = assetList[i].thumbnail[0];
                    if(_assetsDict.ContainsKey(thumbnail.id)) continue;
                    _assetsDict.Add(thumbnail.id,thumbnail);
                    _assetThumbnails.Add(thumbnail);
                }
                // Process Image Thumbnail
                else
                {
                    var thumbnail = new Partners.Asset(assetList[i]);
                    thumbnail.id = -thumbnail.id;
                    if(_assetsDict.ContainsKey(thumbnail.id)) continue;
                    thumbnail.url = assetList[i].imageThumbURL;
                    assetList[i].thumbnailID = thumbnail.id;
                    _assetsDict.Add(thumbnail.id,thumbnail);
                    _assetThumbnails.Add(thumbnail);
                }
            }
        }

        public List<Partners.Asset> GetAssetList()
        {
            return _assets;
        }
        
        public List<Partners.Asset> GetThumbnailsList()
        {
            return _assetThumbnails;
        }
    }

    [Serializable]
    public class Data
    {
        public List<Content>      contentFolders;
        public List<ContentAsset> assetList;
    }

    [Serializable]
    public class ContentAsset : Partners.Asset
    {
        public string               imageThumbURL;
        public int                  thumbnailID;
        public List<Partners.Asset> thumbnail;

        public ContentAsset(ContentAsset asset) : base(asset) { }
    }
    
    [Serializable]
    public class Content
    {
        public string          id;
        public string          title;
        public List<ContentID> contentList;
    }

    [Serializable]
    public class ContentID
    {
        public int               id;
        public List<ThumbnailID> thumbnailIds;
    }

    [Serializable]
    public class ThumbnailID
    {
        public int id;
    }
}
