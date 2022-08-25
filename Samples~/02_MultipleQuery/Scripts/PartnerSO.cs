using System;
using System.Collections.Generic;
using BUT.Downloader;
using MAF.Presentation.Content;
using UnityEngine;

namespace MAF.Presentation.Partners
{

    [CreateAssetMenu(fileName = "Partners SO", menuName = "Downloader/Create Partners DataObject", order = 0)]
    public class PartnerSO : JsonDataModel
    {
        public Data data;

        private Dictionary<int, Asset> _assetsDict;
        private List<Asset>            _assets;

        public void ProcessData()
        {
            // _assetsDict = new Dictionary<int, Asset>();
            // _assets = new List<Asset>();
            // foreach (var entry in data.partners)
            // {
            //     if (entry.logo.Any())
            //     {
            //         _assets.Add(entry.logo[0]);
            //     }
            //     
            //     if (entry.media.Any())
            //     {
            //         _assets.Add(entry.media[0]);
            //     }
            // }
        }

        public List<Asset> GetAssetList()
        {
            // return _assets;
            return data.assetList;
        }
    }

    [Serializable]
    public class Asset : AssetBase
    {
        public int    id;
        public string kind;
        public string title;

        public Asset(ContentAsset asset)
        {
            extension = asset.extension;
            url = asset.url;
            localPath = asset.localPath;
            dateModified = asset.dateModified;
            id = asset.id;
            kind = asset.kind;
            title = asset.title;
        }
    }

    [Serializable]
    public class Partner
    {
        public int         id;
        public string      name;
        public List<int> logo;
        public string      writeup;
        public List<int> media;
    }

    [Serializable]
    public class News
    {
        public int       id;
        public string    headline;
        public string    subheading;
        public string    body;
        public List<int> media;
    }

    [Serializable]
    public class Data
    {
        public List<Partner> partners;
        public List<News>    news;
        public List<Asset>   assetList;
    }
    
}