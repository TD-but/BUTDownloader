
# Downloader

Custom downloader asset that allows easy downloading of Json files and assets from the backend using GraphQL queries.


## Features

- Easy Downloading and saving of Json from backend using GraphQL
- Uses Scriptable Objects for easy decoupling of data and functionality 
- Asynchronous and multithreaded downloading and saving of assets from backend



## Installation

You can add this library to your project using the Package Manager.

Go to the package manager and click on "Add package from git URL".
From there, add this URL:

```
  https://github.com/LastAbyss/SimpleGraphQL-For-Unity.git
```
    
## API Reference

#### Download Json Data

```c#
  Downloader.DownloadJsonData(onComplete)
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `onComplete` | `Action` | **Optional**. Method to execute when json data is downloaded |

#### Download Assets

```c#
  Downloader.DownloadAssetData(assets)
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `assets`      | `IEnumerable<AssetBase>` | **Required**. List of assets to download |
| `assets`      | `Dictionary<TK,TV>` | **Required**. Dictionary of assets to download |



## Configuration
#### Import: Put your GraphQL query files (.txt) somewhere in your Assets folder.
### Create a Settings File
1. Right Click -> Create -> Downloader -> Create Downloader Settings

    ![Settings File Screenshot](https://i.postimg.cc/xjkwqmNv/Downloader-Settings.png)

2. Check **Download Json** option if you want to download json data and fill in the required data. 
3. Check **Download Assets** option if you want to download Assets and fill in the required data.

### Create Json Data Model
If you are downloading Json data to access it from within Unity it has to be serialized in a C# class. Thus you will have to create your own Json Data Model Scriptable Object Class to serialize json data into.
1. Create a Scriptable Object class that will hold your Json data and must inherit from **JsonDataModel**

    ```
    [CreateAssetMenu(fileName = "JsonRoot", menuName = "Downloader/Create Json Model", order = 0)]
    public class JsonRoot : JsonDataModel
    {
        public List<Entry> financeEntries;
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
    ```

2. Create an instance of the Scriptable Object somwhere in your assets folder.
3. Provide a reference of this instance to the Downloader Settings file. 

    ![Settings File Screenshot highlighting json model field](https://i.postimg.cc/Dywvw63x/Downloader-Settings-Highlighted.png)