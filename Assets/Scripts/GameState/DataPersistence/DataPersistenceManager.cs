using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    public static DataPersistenceManager Instance { get; private set; }
    
    [Header("Debugging")]
    [SerializeField] private bool disableDataPersistence = false;
    [SerializeField] private bool initializeDataIfNull = false;

    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    // other fields
    public GameData GameData;
    private IDataPersistence[] dataPersistenceObjects;
    private GameStateFileIO fileHandler;

    private string selectedProfileId;
    
#if DEBUG
    private int dataPersistenceObjectsLength;
#endif
    
    private void Awake() 
    {
        if (Instance != null) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (disableDataPersistence) 
        {
            Debug.LogWarning("Data Persistence is currently disabled!");
        }
        
        fileHandler = new GameStateFileIO(Application.persistentDataPath, fileName);
        InitializeSelectedProfileId();
    }
    
#if DEBUG
    private void Update()
    {
        dataPersistenceObjectsLength = dataPersistenceObjects.Length;
    }
#endif
    
    private void OnEnable() 
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() 
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dataPersistenceObjects = FindAllDataPersistenceObjects();
    }

    public void ChangeSelectedProfileId(string newProfileId) 
    {
        selectedProfileId = newProfileId;
        LoadGame();
    }

    public void NewGame(string profileId) 
    {
        GameData = new GameData(profileId);
        fileHandler.Save(GameData, selectedProfileId);
    }
    
    private void LoadGame(bool reload = true)
    {
        if (disableDataPersistence) 
            return;
        
        // if reload=true, gameData will be reloaded from disk
        if(reload)
            GameData = fileHandler.Load(selectedProfileId);

        if (GameData == null && initializeDataIfNull)
            NewGame("dev_default_profile");
        
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects) 
            dataPersistenceObj.LoadData(GameData);
    }
    
    /// iterates through all data persistence objects, and calls SaveData on each of them
    public void SaveGame()
    {
        if (disableDataPersistence || GameData == null)
        {
#if DEBUG
            if (!disableDataPersistence)
                Debug.LogError("GameData is Null. Could not save.");
#endif
            return;
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects) 
            dataPersistenceObj.SaveData(GameData);

        // timestamp the data
        GameData.LastUpdated = DateTime.Now.ToBinary();

        fileHandler.Save(GameData, selectedProfileId);
    }

    public void DeleteProfileData(string profileId)
    {
        fileHandler.Delete(profileId);
        InitializeSelectedProfileId();
        LoadGame();
    }

    private void InitializeSelectedProfileId() 
    {
        GameData = fileHandler.GetMostRecentlyUpdatedProfileId();
        selectedProfileId = GameData.ProfileName;
    }

    private IDataPersistence[] FindAllDataPersistenceObjects() 
    {
        return FindObjectsOfType<MonoBehaviour>(true)
            .OfType<IDataPersistence>()
            .ToArray();
    }

    public bool HasGameData()
    {
        if (GameData != null)
            return true;
        return GetAllProfilesGameData().Count > 0;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData() 
    {
        return fileHandler.LoadAllProfiles();
    }

    public void OnDeath()
    {
        Player.Instance.SaveCurrencyAndInventory(GameData);
        
        // timestamp the data
        GameData.LastUpdated = DateTime.Now.ToBinary();

        fileHandler.Save(GameData, selectedProfileId);
    }
}