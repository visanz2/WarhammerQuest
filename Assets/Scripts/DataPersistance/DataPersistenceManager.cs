using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;



public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool intializeDataIfNull = false;

    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    [Header("Is a new Campaign?")]
    [SerializeField] private bool newCampaing = false;


    // This class can be only insatnce one in the whole project
    public static DataPersistenceManager Instance { get; private set; }

    private List<IDataPersistence> dataPersistenceObjects = new List<IDataPersistence>();

    private FileDataHandler dataHandler;

    // Data that will be loaded and saved
    private GameData gameData;



    // Keeps track of the save/load profile data 
    public string selectedProfileID = "";

    // dictionary < icon name aventurer, list of cities >
    public Dictionary<int, List<string>> dict_DarkSecret = new Dictionary<int, List<string>>();

    // Name of the aventurer and abreviation of the aventurers
    public List<string> list_AventurerType = new List<string>();
    public List<string> list_AventurerTypeIcon = new List<string>();

    // The ID of the player in the menu selection
    private List<int> list_playersDarkSecrets = new List<int>();

    // List of key values of dictionary per player (the dark secret card where the player is currently)
    private List<int> list_playersCityOnDarkSecrets = new List<int>();



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            UnityEngine.Debug.Log("Found more than one Data Persistence Manager in the scene. Destroying new one.");
            Destroy(gameObject);
            return;
        }

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        
        this.selectedProfileID = dataHandler.GetMostRecentlyUpdatedProfileId();
        LoadingDarkSecrets();
    }


    public void LoadingDarkSecrets()
    {
        int countLine = 0;

        TextAsset file = Resources.Load("DarkSecrets/DarkSecrets") as TextAsset;
        
        string[] fileLines = file.text.Split('\n');
        //List<string> fileLines = File.ReadAllLines(path, Encoding.UTF8).ToList();
        foreach (string line in fileLines)
        {
            // Removing headers
            if (countLine == 0)
            {
                countLine++;
                continue;
            }

            
            string[] darkSecretInfo = line.Split(';');
            // 0 - Aventurero
            // 1 - Mazo
            // 2 - Ciudad1
            //...
            // 7 - Ciudad6
            List<string> citiesDarkSecret = new List<string>();
            citiesDarkSecret.Add(darkSecretInfo[2]);
            citiesDarkSecret.Add(darkSecretInfo[3]);
            citiesDarkSecret.Add(darkSecretInfo[4]);
            citiesDarkSecret.Add(darkSecretInfo[5]);

            citiesDarkSecret.Add(darkSecretInfo[6]);
            if (darkSecretInfo[6] != "-")
                citiesDarkSecret.Add(darkSecretInfo[7]);

            list_AventurerType.Add(darkSecretInfo[0]);
            list_AventurerTypeIcon.Add(darkSecretInfo[1]);

            dict_DarkSecret.Add(countLine-1, citiesDarkSecret);

            countLine++;

        }
        
    }

    /////////////////////////////////////////////////
    // Functions to pass information through scenes//
    /////////////////////////////////////////////////
    private void OnEnable()
    {
       SceneManager.sceneLoaded += OnSceneLoaded;
       SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // This function is called after OnEnable() but before than Start()
        // And it will not called the first time that the game starts

        // Application.persistentDataPath returns the folder of unity of persistence data
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void ChangeSelectedProfileID(string newProfileID)
    {
        // Update the profile to use for saving and loading
        this.selectedProfileID = newProfileID;

        // Load the game, which will use that profile, updating our fame data accordingly
        LoadGame();
    }

    public void OnSceneUnloaded(Scene scene)
    {
        SaveGame();
    }
    /////////////////////////////////////////////////

    public void NewGame(string campaignName)
    {
        this.gameData = new GameData(campaignName, list_playersDarkSecrets, list_playersCityOnDarkSecrets);   
    }

    public void LoadGame()
    {
        // Loading any saved data from a file using the data handler
        this.gameData = dataHandler.Load(this.selectedProfileID);

        // start a new game if the data is null an we are configured to initialize data for debug
        if (this.gameData == null && intializeDataIfNull)
        {
            NewGame("debug");
        }


        // If no data can be loaded, dont continue
        if (this.gameData == null)
        {
            Debug.Log("No data was found. A New Game needs to be started before data can be loaded.");
            return ;
        }

        // Push de loaded dta to all other scripts that need it
        // Other functions are collecting the data form loading
        foreach (IDataPersistence dataPersistenceObj in this.dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);  
        }

        Debug.Log("Data loaded");

    }

    public void SaveGame()
    {
        // if we do not have any data to save, log a warning here
        if (this.gameData == null)
        {
            Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved.");
            return;
        }
        // Pass the data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in this.dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        gameData.lastUpdated = System.DateTime.Now.ToBinary();

        Debug.Log("Data saved");

        // Save that data to a file using the data handler
        dataHandler.Save(gameData, this.selectedProfileID);
    }



    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        // The scripts with Monobehaviour and IDataPersistence reference are found 
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        
        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    // Check if there is any data. It is used to disable continue button in main menu
    public bool HasGameData()
    {
        return this.gameData != null;
    }

    // 
    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }

    public void SetNewCampaing(bool bool_NewCampaign)
    {
        this.newCampaing = bool_NewCampaign;
    }

    public bool GetNewCampaing()
    {
        return this.newCampaing;
    }

    public int GetTotalDarkSecretPlayers()
    {
        return list_playersDarkSecrets.Count;
    }

    public void AddPlayerDarkSecret(int position)
    {
        list_playersDarkSecrets.Add(position);
    }

    public int GetPlayerDarkSecret(int position)
    {
        return list_playersDarkSecrets[position];
    }

    public void AddPlayersCityOnDarkSecrets(int position)
    {
        list_playersCityOnDarkSecrets.Add(position);
    }

    public void MovePlayerToNextCityDarkSecret(int player)
    {
        list_playersCityOnDarkSecrets[player]++;
    }

    public int GetPlayerCityDarkSecret(int player)
    {
        return list_playersCityOnDarkSecrets[player];
    }

    public void UpdateDarkSecrets()
    {
        // Disable all dark secret flags
        // Next function only founds the falgs that are active
        GameObject[] darkSecretFlags = GameObject.FindGameObjectsWithTag("DarkSecretFlag");

        foreach (GameObject flag in darkSecretFlags)
        {
            flag.SetActive(false);
        }

        int countPlayer = 0;
        // Activating the current flags of the dark secrets
        foreach (int player in list_playersDarkSecrets)
        {
            List<string> playersDarkSecretCities = dict_DarkSecret[player];
           
            string cityName = playersDarkSecretCities[list_playersCityOnDarkSecrets[countPlayer]];

            if (cityName == "X")
            {
                PersistentVariables.Instance.AddSecretCardToBeActivated(this.list_AventurerTypeIcon[player] + "/" + list_playersCityOnDarkSecrets[countPlayer].ToString());
                continue;
            }
            

            if (cityName.Contains("---"))
            {
                Debug.Log("Player " + this.list_AventurerType[player] + " has finished his own story!");
                continue;
            }
            GameObject hexagonCell = GameObject.Find(cityName);

            hexagonCell.GetComponent<City>().SetMaterialDarkSecret(this.list_AventurerTypeIcon[player], countPlayer);
            countPlayer++;
        }
    }


    public List<int> GetListDarkSecretPlayers()
    {
        return list_playersDarkSecrets;
    }

    public List<int> GetListDarkSecretPlayerCard()
    {
        return list_playersCityOnDarkSecrets;
    }

    public void SetListDarkSecretPlayers(List<int> listToCopy)
    {
        list_playersDarkSecrets = listToCopy;
    }

    public void SetListDarkSecretPlayerCard(List<int> listToCopy)
    {
        list_playersCityOnDarkSecrets = listToCopy; ;
    }

}   


