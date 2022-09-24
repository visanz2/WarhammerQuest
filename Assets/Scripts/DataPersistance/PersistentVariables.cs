using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;


// History of cells walked

public class heroWalkStructure
{
    // Stroting the names of the cells ir more than enough.
    public string cellIniName;
    public string cellEndName;
}

//[System.Serializable]
public class PersistentVariables : MonoBehaviour, IDataPersistence
{
    public static PersistentVariables Instance { get; private set; }

    // Name of the cell pressed and previously pressed
    private string cell_CurrentPressedName;
    
    // Current position of heroes
    private string cell_CurrentPositionOfHeroes;

    // History of previous cells visited
    private List<heroWalkStructure> list_heroWalk = new List<heroWalkStructure>();

    // Option data in the main menu
    public Toggle showFlagsToggle;
    public Toggle showHexToggle;
    public Toggle showHeroWalkToggle;

    [Header("Button from Villain Menu")]
    [SerializeField] private GameObject buttonRecoverCityVillain;
    [SerializeField] private Button buttonKilledVillain;
    [SerializeField] private Button buttonGeneratedVillain;

    [Header("List campaign event cards")]
    [SerializeField] private Canvas renderCardCanvas;
    [SerializeField] private Image renderCardImage;
    [SerializeField] private GameObject button_ReturnToStack;
    [SerializeField] private Sprite prologueImage;
    [SerializeField] private List<Sprite> list_InitialCampaignEvents;
    [SerializeField] private List<Sprite> list_ACampaignEvents;
    [SerializeField] private List<Sprite> list_BCampaignEvents;
    [SerializeField] private List<Sprite> list_CCampaignEvents;
    [SerializeField] private List<Sprite> list_DCampaignEvents;

    [Header("Map and turn of the game")]
    [SerializeField] private GameObject map;
    [SerializeField] private int gameTurn;

    [Header("Canvas for avoid incorrect player movement")]
    [SerializeField] private GameObject canvasMovementNotAllowed;
    [SerializeField] private GameObject canvasMovementNotAllowedDarkSecret;

    // Disctionary of disabling or destroying cities by events
    // dictionary < idCard, list of cities >
    private Dictionary<string, List<string>> dict_EventsDisablingCities = new Dictionary<string, List<string>>();
    private Dictionary<string, string> dict_EventsDestroyingCities = new Dictionary<string, string>();

    // Temporal data to enabling cities after turn has passed
    private List<string> citiesToEnabling = new List<string>();

    // Information about event cards used
    private List<string> list_EventCardsUsed = new List<string>();
    private int eventCardLevel = 0;




    public Dictionary<string, bool> dict_DarkSecretCardsToActiveNextCity = new Dictionary<string, bool>();

    public bool isDarkSecretHasBeenRead { get; set; }



    private void Awake()
    {
        // Creating persistance object
        if (Instance == null)
        {
            cell_CurrentPositionOfHeroes = "Altdorf";
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (DataPersistenceManager.Instance.GetNewCampaing() == true)
                NewGame();

        }
        else
        {
            UnityEngine.Debug.Log("Destroying");
            Destroy(gameObject);
        }

        LoadingDisabledCitiesByEvents();
        LoadingDestroyedCitiesByEvents();
    }


    ////////////////////////////////////////////
    ///          Load and Save function      ///
    ////////////////////////////////////////////
    public void LoadData(GameData data)
    {
        int counter = 0;

        isDarkSecretHasBeenRead = false;

        foreach (string cellIniNameSaved in data.list_cellIniName)
        {
            DrawingHeroWalk(cellIniNameSaved, data.list_cellEndName[counter]);
            counter++;
        }

        // Last position saved is the position of the heroes
        if (data.list_cellEndName.Count > 0)
        {
            cell_CurrentPositionOfHeroes = data.list_cellEndName[data.list_cellEndName.Count - 1];
        }
        else
        {
            cell_CurrentPositionOfHeroes = "Altdorf";
        }

        // Chanign father and moving hero Icon
        GameObject.Find("Hero_Icon").transform.SetParent((GameObject.Find(this.cell_CurrentPositionOfHeroes).transform));
        GameObject.Find("Hero_Icon").transform.localPosition = new Vector3(0, 0, 0);

        Debug.Log("heroes position loaded: " + cell_CurrentPositionOfHeroes);

        showFlagsToggle.isOn = data.showFlags; 
        showHexToggle.isOn = data.showHex;
        showHeroWalkToggle.isOn = data.showHeroWalk;

        // Loading Users preferences
        ActivateOrNotFlags();
        ActivateOrNotHexagons();
        ActivateOrNotHeroWalk();

        // copying for future used
        list_EventCardsUsed = data.list_IDCampaignEvents;

        // Removing Event cards already used
        UpdatingEventCardList(ref list_InitialCampaignEvents, data.list_IDCampaignEvents);
        UpdatingEventCardList(ref list_ACampaignEvents, data.list_IDCampaignEvents);
        UpdatingEventCardList(ref list_BCampaignEvents, data.list_IDCampaignEvents);
        UpdatingEventCardList(ref list_CCampaignEvents, data.list_IDCampaignEvents);
        UpdatingEventCardList(ref list_DCampaignEvents, data.list_IDCampaignEvents);

        DataPersistenceManager.Instance.SetListDarkSecretPlayers(data.list_DarkSecretHeroes);
        DataPersistenceManager.Instance.SetListDarkSecretPlayerCard(data.list_DarkSecretCardPosition);

        DataPersistenceManager.Instance.UpdateDarkSecrets();

        this.citiesToEnabling = data.list_CityDisabled;

        // Disabling cities that were disable in the games
        foreach (string cityName in this.citiesToEnabling)
        {
            GameObject hexagonCellCity = GameObject.Find(cityName);
            if (hexagonCellCity.GetComponent<City>() != null)
            {
                hexagonCellCity.GetComponent<City>().SetDisablingCity(true);
            }
        }

        this.gameTurn = data.gameTurn;
    }

    public void SaveData(ref GameData data)
    {
        foreach (heroWalkStructure eachWalk in list_heroWalk)
        {
            data.list_cellIniName.Add(eachWalk.cellIniName);
            data.list_cellEndName.Add(eachWalk.cellEndName);
        }

        data.showFlags = showFlagsToggle.isOn;
        data.showHex = showHexToggle.isOn;
        data.showHeroWalk = showHeroWalkToggle.isOn;

        data.list_IDCampaignEvents = list_EventCardsUsed;

        data.list_CityDisabled = this.citiesToEnabling;

        data.list_DarkSecretHeroes = DataPersistenceManager.Instance.GetListDarkSecretPlayers();
        data.list_DarkSecretCardPosition = DataPersistenceManager.Instance.GetListDarkSecretPlayerCard();

        data.gameTurn = this.gameTurn;
    }

    /////////////////////////////

    public void LoadingDisabledCitiesByEvents()
    {
        //CampaignEventCards
        int countLine = 0;

        TextAsset file = Resources.Load("CampaignEventCards/CityDisabled") as TextAsset;

        string[] fileLines = file.text.Split('\n');

        foreach (string line in fileLines)
        {
            // Removing headers
            if (countLine == 0)
            {
                countLine++;
                continue;
            }

            string[] eventInfo = line.Split(';');
            // 0 - EventCardId
            // 1 - Ciudad1
            //...
            // 4 - Ciudad4
            List<string> citiesDisabled = new List<string>();
            citiesDisabled.Add(eventInfo[1]);

            if (eventInfo[2] != "")
            {
                citiesDisabled.Add(eventInfo[2]);
                if (eventInfo[3] != "")
                {
                    citiesDisabled.Add(eventInfo[3]);
                    if (eventInfo[4] != "")
                    {
                        citiesDisabled.Add(eventInfo[4]);
                    }
                }
            }
            dict_EventsDisablingCities.Add(eventInfo[0], citiesDisabled);
        }
    }

    public void LoadingDestroyedCitiesByEvents()
    {
        //CampaignEventCards
        int countLine = 0;

        TextAsset file = Resources.Load("CampaignEventCards/CityDestroyed") as TextAsset;

        string[] fileLines = file.text.Split('\n');

        foreach (string line in fileLines)
        {
            // Removing headers
            if (countLine == 0)
            {
                countLine++;
                continue;
            }

            string[] eventInfo = line.Split(';');
            // 0 - EventCardId
            // 1 - Ciudad1
            dict_EventsDestroyingCities.Add(eventInfo[0], eventInfo[1]);
        }
    }

    public void NewGame()
    {
        // Show prologue
        button_ReturnToStack.SetActive(false);
        renderCardCanvas.enabled = true;
        //renderCardImage.rectTransform.sizeDelta = new Vector2(1654, 2339);
        //renderCardImage.rectTransform.localScale = new Vector3(0.4f, 0.4f, 1);
        renderCardImage.sprite = prologueImage;
        map.GetComponent<PinchAndZoom>().enabled = false;
    }

    private void UpdatingEventCardList(ref List<Sprite> list_Sprites, List<string> list_NamesToRemove)
    {
        List<Sprite> list_copy = new List<Sprite> (list_Sprites);

        // Removing Event cards already used
        foreach (Sprite eventCard in list_Sprites)
        {
            foreach (string eventCardAlreadyDone in list_NamesToRemove)
            {
                if (eventCardAlreadyDone == eventCard.name)
                {
                    list_copy.Remove(eventCard);
                    break;
                }
            }
        }

        list_Sprites = list_copy;
    }

    private void DrawingHeroWalk(string cellIniName, string cellDestinationName)
    {
        bool wasAlreadyPainted = false;

        if (cellIniName == "")
            return;
       

        for (int i = 0; i < list_heroWalk.Count; i++)
        {
            if (list_heroWalk.Count > 0 && list_heroWalk[i].cellIniName == cellIniName && list_heroWalk[i].cellEndName == cellDestinationName)
            {
                wasAlreadyPainted = true;
                //UnityEngine.Debug.Log("It was already drawn!!!");
                break;
            }
        }

        if (wasAlreadyPainted == false)
        {
            // Position of Initial cell
            GameObject iniCellTemp = GameObject.Find(cellIniName);
            Vector3 cellIniLocation = new Vector3(iniCellTemp.transform.position.x, iniCellTemp.transform.position.y, iniCellTemp.transform.position.z);

            // Position of destination cell
            GameObject destinationCellTemp = GameObject.Find(cellDestinationName);
            Vector3 cellFinLocation = new Vector3(destinationCellTemp.transform.position.x, destinationCellTemp.transform.position.y, destinationCellTemp.transform.position.z);

            // Creating Gameobject for line renderer under HeroWalks  gameobject
            GameObject HeroWalks = GameObject.Find("HeroWalks");
            GameObject Temporal = new GameObject(cellIniName + "-" + cellDestinationName);
            Temporal.transform.parent = HeroWalks.transform;

            //For creating line renderer object
            LineRenderer lineRenderer = Temporal.AddComponent<LineRenderer>();
            lineRenderer.startColor = Color.black;
            lineRenderer.endColor = Color.black;
            lineRenderer.startWidth = 1f;
            lineRenderer.endWidth = 1f;
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = false;
            lineRenderer.sortingOrder = 1;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.material.color = Color.red;

            //For drawing line in the world space, provide the x,y,z values
            lineRenderer.SetPosition(0, cellIniLocation);
            lineRenderer.SetPosition(1, cellFinLocation); //x,y and z position of the end point of the line

            // Check if the user want it to see it or not
            lineRenderer.enabled = showHeroWalkToggle.isOn;

            heroWalkStructure temporal = new heroWalkStructure();

            temporal.cellIniName = cellIniName;
            temporal.cellEndName = cellDestinationName;

            list_heroWalk.Add(temporal);
        }
    }

    // Function to keep tracking of the cell pressed
    public void UpdateCurrentCell(string cellDestinationName)
    {
        this.cell_CurrentPressedName = cellDestinationName;
    }
    
    // Returns the name of the cell where the heroes are now
    public string GetHeroesCellCurrentPosName()
    {
        return this.cell_CurrentPositionOfHeroes;
    }

    // Move heroes from previous pressded cell to the current one
    public void MovingHero()
    {
        StartCoroutine(MovingHeroCoroutine());
    }

    // Coroutine functions
    IEnumerator MovingHeroCoroutine()
    {
        GameObject hexagonCellCheckingDarkSecret = GameObject.Find(this.cell_CurrentPositionOfHeroes);
        // If it is a city, check if the current cell still has dark secrets that players must complete
        if (hexagonCellCheckingDarkSecret.GetComponent<City>() != null) 
        {
            if (hexagonCellCheckingDarkSecret.GetComponent<City>().hasAnyDarkSecretActive() == true)
            {
                canvasMovementNotAllowedDarkSecret.GetComponent<Canvas>().enabled = true;
                Debug.Log("Dark secrets still active");
                while (!isDarkSecretHasBeenRead)
                {
                    yield return null;
                }
                isDarkSecretHasBeenRead = false;
                yield break;
            }
        }

        // If heroes are moving from a Villain cell, then a campaign card is shown
        if (GameObject.Find(this.cell_CurrentPositionOfHeroes).tag == "Villain")
        {
            renderCardImage.rectTransform.sizeDelta = new Vector2(489, 688);
            button_ReturnToStack.SetActive(true);
            renderCardCanvas.enabled = true;
            isDarkSecretHasBeenRead = false;
            string eventCardId = SelectingEventCard();
            // Starting corotuine until player finishes reading card
            // Waiting until player presses
            while (!isDarkSecretHasBeenRead)
            {
                yield return null;
            }
            isDarkSecretHasBeenRead = false;

            // Enabling cities that were disable in the previous turn
            foreach (string cityName in this.citiesToEnabling)
            {
                GameObject hexagonCellCity = GameObject.Find(cityName);
                if (hexagonCellCity.GetComponent<City>() != null)
                {
                    hexagonCellCity.GetComponent<City>().SetDisablingCity(false);
                }
            }

            // Checking if event causes Disabling a city
            bool hasEventCauseDisability = dict_EventsDisablingCities.ContainsKey(eventCardId);
            if (hasEventCauseDisability == true)
            {
                foreach (string cityName in dict_EventsDisablingCities[eventCardId])
                {
                    GameObject hexagonCellCity = GameObject.Find(cityName);

                    if (hexagonCellCity.GetComponent<City>() != null)
                    {
                        hexagonCellCity.GetComponent<City>().SetDisablingCity(true);
                    }
                }

                // Next turn, these cities will be enabled
                this.citiesToEnabling = dict_EventsDisablingCities[eventCardId];
            }
            else // cleaning list of enabling cities 
            {
                this.citiesToEnabling = new List<string>();
            }
            

            // Destroying city 
            bool hasEventCauseDestroying = dict_EventsDestroyingCities.ContainsKey(eventCardId);
            if (hasEventCauseDestroying == true)
            {
                string cityName = dict_EventsDestroyingCities[eventCardId];
                GameObject hexagonCellCity = GameObject.Find(cityName);

                if (hexagonCellCity.GetComponent<City>() != null)
                {
                    hexagonCellCity.GetComponent<City>().ConvertCityToVillain();
                }
            }

            bool hasHeroCanMove = true;
            // Checking if players try to move to a cell that has been disabled or destroyed this turn
            GameObject hexagonCell = GameObject.Find(this.cell_CurrentPressedName);
            if (hexagonCell.GetComponent<City>() != null)
            {
                if (hexagonCell.GetComponent<City>().GetCityDeactivated() == false) // City is active, players can move there
                {
                    // Updating Heroes Position
                    cell_CurrentPositionOfHeroes = this.cell_CurrentPressedName;

                    // Changing father and moving hero icon
                    GameObject.Find("Hero_Icon").transform.SetParent((GameObject.Find(this.cell_CurrentPositionOfHeroes).transform));
                    GameObject.Find("Hero_Icon").transform.localPosition = new Vector3(0, 0, 0);
                    
                }
                else
                {
                    canvasMovementNotAllowed.GetComponent<Canvas>().enabled = true;

                    // Starting corotuine until player finishes reading card
                    // Using same coroutine as darksecret
                    // Waiting until player presses
                    while (!isDarkSecretHasBeenRead)
                    {
                        yield return null;
                    }
                    isDarkSecretHasBeenRead = false;
                    hasHeroCanMove = false;
                }
                
            }
            else // Not possible to find the City. So, the city has been destroyed 
            {
                canvasMovementNotAllowed.GetComponent<Canvas>().enabled = true;

                // Starting corotuine until player finishes reading card
                // Using same coroutine as darksecret
                while (!isDarkSecretHasBeenRead)
                {
                    yield return null;
                }
                isDarkSecretHasBeenRead = false;
                hasHeroCanMove = false;
            }

            if (hasHeroCanMove == true)
            {
                // List dark secret cards
                if (dict_DarkSecretCardsToActiveNextCity.Count > 0)
                {
                    List<string> list_DarkSecretToChange = new List<string>();
                    foreach (var key in dict_DarkSecretCardsToActiveNextCity.Keys)
                    {
                        bool HasBeenShownBefore = dict_DarkSecretCardsToActiveNextCity[key];
                        if (HasBeenShownBefore == false)
                        {
                            renderCardImage.rectTransform.sizeDelta = new Vector2(489, 688);
                            button_ReturnToStack.SetActive(false);
                            renderCardCanvas.enabled = true;
                            renderCardImage.sprite = Resources.Load<Sprite>("DarkSecrets/Cards/" + key);
                            list_DarkSecretToChange.Add(key);

                            // Waiting until player presses
                            while (!isDarkSecretHasBeenRead)
                            {
                                yield return null;
                            }
                            isDarkSecretHasBeenRead = false;
                        }
                    }

                    foreach (string card in list_DarkSecretToChange)
                    {
                        dict_DarkSecretCardsToActiveNextCity[card] = true;
                    }
                }

                this.gameTurn++;
            }
        }
        else // The players move from city cell to villain cell
        {
            DrawingHeroWalk(this.cell_CurrentPositionOfHeroes, this.cell_CurrentPressedName);
            // Updating Heroes Position
            cell_CurrentPositionOfHeroes = this.cell_CurrentPressedName;

            // Changing father and moving hero icon
            GameObject.Find("Hero_Icon").transform.SetParent((GameObject.Find(this.cell_CurrentPositionOfHeroes).transform));
            GameObject.Find("Hero_Icon").transform.localPosition = new Vector3(0, 0, 0);
        }
        yield break;
    }

    // Search for a random event card depending on the event card order
    public string SelectingEventCard()
    {
        renderCardImage.GetComponent<UIZoomImage>().initialScale = new Vector3(1f, 1f, 1);
        renderCardImage.rectTransform.localScale = new Vector3(1f, 1f, 1);

        if (this.list_InitialCampaignEvents.Count != 0)
        {
            int randomInt = UnityEngine.Random.Range(0, this.list_InitialCampaignEvents.Count);
            renderCardImage.sprite = this.list_InitialCampaignEvents[randomInt];

            list_EventCardsUsed.Add(list_InitialCampaignEvents[randomInt].name);

            list_InitialCampaignEvents.RemoveAt(randomInt);

            eventCardLevel = 0;
        }
        else if (this.list_ACampaignEvents.Count != 0)
        {
            int randomInt = UnityEngine.Random.Range(0, this.list_ACampaignEvents.Count);
            renderCardImage.sprite = this.list_ACampaignEvents[randomInt];

            
            list_EventCardsUsed.Add(list_ACampaignEvents[randomInt].name);

            list_ACampaignEvents.RemoveAt(randomInt);

            eventCardLevel = 1;
        }
        else if (this.list_BCampaignEvents.Count != 0)
        {
            int randomInt = UnityEngine.Random.Range(0, this.list_BCampaignEvents.Count);
            renderCardImage.sprite = this.list_BCampaignEvents[randomInt];
            
            list_EventCardsUsed.Add(list_BCampaignEvents[randomInt].name);

            list_BCampaignEvents.RemoveAt(randomInt);
            eventCardLevel = 2;
        }
        else if (this.list_CCampaignEvents.Count != 0)
        {
            int randomInt = UnityEngine.Random.Range(0, this.list_CCampaignEvents.Count);
            renderCardImage.sprite = this.list_CCampaignEvents[randomInt];

            list_EventCardsUsed.Add(list_CCampaignEvents[randomInt].name);

            list_CCampaignEvents.RemoveAt(randomInt);
            eventCardLevel = 3;
        }
        else if (this.list_DCampaignEvents.Count != 0)
        {
            int randomInt = UnityEngine.Random.Range(0, this.list_DCampaignEvents.Count);
            renderCardImage.sprite = this.list_DCampaignEvents[randomInt];

            list_EventCardsUsed.Add(list_DCampaignEvents[randomInt].name);

            list_DCampaignEvents.RemoveAt(randomInt);
            eventCardLevel = 4;
        }

        map.GetComponent<PinchAndZoom>().enabled = false;
        return renderCardImage.sprite.name;
    }

    public void OnClickReturnToStack()
    {
        list_EventCardsUsed.Remove(renderCardImage.sprite.name);

        if (eventCardLevel == 0)
            list_InitialCampaignEvents.Add(renderCardImage.sprite);
        else if (eventCardLevel == 1)
            list_ACampaignEvents.Add(renderCardImage.sprite);
        else if (eventCardLevel == 2)
            list_BCampaignEvents.Add(renderCardImage.sprite);
        else if (eventCardLevel == 3)
            list_CCampaignEvents.Add(renderCardImage.sprite);
        else if (eventCardLevel == 4)
            list_DCampaignEvents.Add(renderCardImage.sprite);
    }

    public string GetCurrentCell()
    {
        return this.cell_CurrentPressedName;
    }

    public bool GetActivationOfFlags()
    {
        return showFlagsToggle.isOn;
    }


    public void ActivateOrNotFlags()
    {
        List<string> villain_cells = VillainsGameData.Instance.GetAllHexagonWithVillains();

        if (showFlagsToggle.isOn == false)
        {
            GameObject[] activeFlags = GameObject.FindGameObjectsWithTag("Flag");

            foreach(GameObject flag in activeFlags)
            {
                flag.SetActive(false);
            }
        }
        else
        {
            foreach (string hexagonID in villain_cells)
            {

                GameObject hexagonCell = GameObject.Find(hexagonID);

                Villain[] list_AllVillainCellsInGame = Resources.FindObjectsOfTypeAll<Villain>();

                // This step is necessary because the villain cells in cities can be in deactivate state
                foreach (Villain villain in list_AllVillainCellsInGame)
                {
                    if (villain.gameObject.name == hexagonID)
                    {
                        // Informing cell what happen to the villain contained in that cell
                        //hexagonCell.GetComponent<Villain>().SetFlagVisibility(this.showFlags);
                        string villainID = VillainsGameData.Instance.GetVillainInThatHexagon(hexagonID);
                        hexagonCell.GetComponent<Villain>().SetFlagMaterial(villainID);
                    }
                }
                
            }
        }
    }

    public void ActivateOrNotHexagons()
    {
        // Tags that can be enabled/disabled
        List<String> tagWithHexagons = new List<String>();
        tagWithHexagons.Add("City");
        tagWithHexagons.Add("Town");
        tagWithHexagons.Add("Village");
        tagWithHexagons.Add("Villain");
        tagWithHexagons.Add("DwarfCity");

        // Cheking hexagons with the correspondent tags
        foreach (string tagID in tagWithHexagons)
        {
            GameObject[] activeHex = GameObject.FindGameObjectsWithTag(tagID);
            foreach (GameObject hexagonCell in activeHex)
            {
                hexagonCell.GetComponent<Image>().enabled = showHexToggle.isOn;
            }
        }
    }

    // This functions shows or hides the Hero Walks (LineRenderers)
    public void ActivateOrNotHeroWalk()
    {
        GameObject HeroWalks = GameObject.Find("HeroWalks");
        int totalCellWalks = HeroWalks.transform.childCount;

        for (int i = 0; i < totalCellWalks; i++)
        {
            HeroWalks.transform.GetChild(i).GetComponent<LineRenderer>().enabled = showHeroWalkToggle.isOn;
        }

    }

    public void SetActivateRecoverCityButton(bool isActive)
    {
        buttonRecoverCityVillain.SetActive(isActive);
    }
    public void SetInteractableKillVillainButton(bool isInteractable)
    {
        buttonKilledVillain.interactable = isInteractable;
    }

    public void SetInteractableGenerateVillainButton(bool isInteractable)
    {
        buttonGeneratedVillain.interactable = isInteractable;
    }


    public void ShowingDarkSecret(int player)
    {
        renderCardImage.rectTransform.sizeDelta = new Vector2(489, 688);
        button_ReturnToStack.SetActive(false);
        renderCardCanvas.enabled = true;

        renderCardImage.GetComponent<UIZoomImage>().initialScale = new Vector3(1f, 1f, 1);
        renderCardImage.rectTransform.localScale = new Vector3(1f, 1f, 1);
        
        int playerDarkSecret = DataPersistenceManager.Instance.GetPlayerDarkSecret(player);
        int darkSecretPosition = DataPersistenceManager.Instance.GetPlayerCityDarkSecret(player) + 1;

        string darkSecretIconName = DataPersistenceManager.Instance.list_AventurerTypeIcon[playerDarkSecret];
        
        renderCardImage.sprite = Resources.Load<Sprite>("DarkSecrets/Cards/" + darkSecretIconName + "/" + darkSecretPosition.ToString());
    }

    public void AddSecretCardToBeActivated(string cardName)
    {
        if (dict_DarkSecretCardsToActiveNextCity.ContainsKey(cardName))
        { 
            return;
        }
        else
        {
            dict_DarkSecretCardsToActiveNextCity.Add(cardName, false);
        }
        
    }
}
