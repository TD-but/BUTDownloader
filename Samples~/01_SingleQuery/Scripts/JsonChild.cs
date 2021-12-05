using System;
using System.Collections.Generic;
using System.Linq;
using BUT.Downloader;
using UnityEngine;

[CreateAssetMenu(fileName = "JsonChild", menuName = "Downloader/Create Json Child", order = 0)]
public class JsonChild : JsonDataModel
{
    public Data data;
    
    private Dictionary<int, Asset> _assetsDict;
    private List<Asset>            _assets;

    public void ProcessData()
    {
        _assetsDict = new Dictionary<int, Asset>();
        _assets = new List<Asset>();
        foreach (var entry in data.financeEntries)
        {
            if (entry.assets.Any())
            {
                _assets.Add(entry.assets[0]);
                _assetsDict.Add(entry.id,entry.assets[0]);
            }
        }

        foreach (var partner in data.partners.Where(partner => partner.logo.Any()))
        {
            if (!_assetsDict.ContainsKey(partner.id))
            {
                _assetsDict.Add(partner.id, partner.logo[0]);
                _assets.Add(partner.logo[0]);
            }
        }
    }

    public List<Asset> GetAssetList()
    {
        return _assets;
    }

    public Dictionary<int, Asset> GetAssetDict()
    {
        return _assetsDict;
    }

    public Asset GetAsset(int id)
    {
        return _assetsDict.ContainsKey(id) ? _assetsDict[id] : null;
    }
}


[Serializable]
public class EntryData
{
    public int    id;
    public string name;
}

[Serializable]
public class Body
{
    public string text;
}

[Serializable]
public class Asset: AssetBase
{
    public int      id;
    public string   kind;
}

[Serializable]
public class Entry
{
    public int             id;
    public string          orientation;
    public string          title;
    public List<Body>      body;
    public List<EntryData> country;
    public List<EntryData> tech;
    public List<EntryData> partners;
    public List<Asset>     assets;
}

[Serializable]
public class Partner
{
    public int         id;
    public List<Asset> logo;
}

[Serializable]
public class Data
{
    public List<Entry>   financeEntries;
    public List<Partner> partners;
}