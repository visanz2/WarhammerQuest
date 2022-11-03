using PhotoViewer.Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;
//using Button = UnityEngine.UI.Button;

public class MainMenuMap : MonoBehaviour
{

    [Header("Text where killed villains are displayed")]
    [SerializeField] private TextMeshProUGUI text_VillainsKilled;

    [Header("Text showing that was saved")]
    [SerializeField] private TextMeshProUGUI text_SavedData;
    [SerializeField] private float timeToAppear = 2f;
    [SerializeField] private float timeWhenDisappear;

    [Header("Red Arrow information")]
    [SerializeField] private GameObject red_Arrow;
    [SerializeField] private float timeToAppearRedArrow = 2f;
    [SerializeField] private float timeWhenDisappearRedArrow;

    [Header("Dropdown to find city")]
    [SerializeField] private TMP_Dropdown dropdown_City;
    [SerializeField] private TMP_Dropdown dropdown_Town;
    [SerializeField] private TMP_Dropdown dropdown_Village;
    [SerializeField] private TMP_Dropdown dropdown_Seaport;
    [SerializeField] private TMP_Dropdown dropdown_Villain;

    [Header("Instructions")]
    [SerializeField] private TestImageLibrary instructions;
    [SerializeField] private GameObject _photoViewerGameObject = null;
    [SerializeField] private PhotoViewer.Scripts.PhotoViewer _photoViewer = null;

    [Header("Dark Secrets Menu")]
    [SerializeField] private TextMeshProUGUI textPlayer1;
    [SerializeField] private TextMeshProUGUI textPlayer2;
    [SerializeField] private TextMeshProUGUI textPlayer3;
    [SerializeField] private TextMeshProUGUI textPlayer4;
    [SerializeField] private TextMeshProUGUI textPlayer5;
    [SerializeField] private TextMeshProUGUI textPlayer6;
    [SerializeField] private Button buttonPlayer1;
    [SerializeField] private Button buttonPlayer2;
    [SerializeField] private Button buttonPlayer3;
    [SerializeField] private Button buttonPlayer4;
    [SerializeField] private Button buttonPlayer5;
    [SerializeField] private Button buttonPlayer6;
    [SerializeField] private GameObject panelPlayer1;
    [SerializeField] private GameObject panelPlayer2;
    [SerializeField] private GameObject panelPlayer3;
    [SerializeField] private GameObject panelPlayer4;
    [SerializeField] private GameObject panelPlayer5;
    [SerializeField] private GameObject panelPlayer6;


    private List<string> optionsCity = new List<string>();
    private List<string> optionsTown = new List<string>();
    private List<string> optionsVillage = new List<string>();
    private List<string> optionsSeaport = new List<string>();
    private List<string> optionsVillain = new List<string>();



    public void Awake()
    {
        
        // Getting all objects to find
        GameObject[] cityCells = GameObject.FindGameObjectsWithTag("City");
        GameObject[] townCells= GameObject.FindGameObjectsWithTag("Town");
        GameObject[] villageCells = GameObject.FindGameObjectsWithTag("Village");
        GameObject[] seaportCells = GameObject.FindGameObjectsWithTag("Seaport");
        GameObject[] CampSiteCells = GameObject.FindGameObjectsWithTag("CampSite");
        GameObject[] dwarfCityCells = GameObject.FindGameObjectsWithTag("DwarfCity");
        GameObject[] dwarfVillageCells = GameObject.FindGameObjectsWithTag("DwarfVillage");
        GameObject[] villainCells = GameObject.FindGameObjectsWithTag("Villain");

        // Setting dropdowns
        foreach (GameObject cityCell in cityCells)
            optionsCity.Add(cityCell.name);
        
        // Dwarfcities are found in the city drowpdown
        foreach (GameObject dwarfCityCell in dwarfCityCells)
            optionsCity.Add(dwarfCityCell.name);

        foreach (GameObject townCell in townCells)
            optionsTown.Add(townCell.name);

        foreach (GameObject campsiteCell in CampSiteCells)
            optionsTown.Add(campsiteCell.name);

        foreach (GameObject villageCell in villageCells)
            optionsVillage.Add(villageCell.name);

        foreach (GameObject dwarfVillageCell in dwarfVillageCells)
            optionsVillage.Add(dwarfVillageCell.name);

        foreach (GameObject seaportCell in seaportCells)
            optionsSeaport.Add(seaportCell.name);

        foreach( GameObject villainCell in villainCells)
            optionsVillain.Add(villainCell.name);

        // Sorting lists
        optionsCity.Sort();
        optionsTown.Sort();
        optionsVillage.Sort();
        optionsSeaport.Sort();
        optionsVillain.Sort();

        // Putting information in dropdowns
        dropdown_City.AddOptions(optionsCity);
        dropdown_Town.AddOptions(optionsTown);
        dropdown_Village.AddOptions(optionsVillage);
        dropdown_Seaport.AddOptions(optionsSeaport);
        dropdown_Villain.AddOptions(optionsVillain);
    }

    public void ShowVillainsKilled()
    {
        // search of villains killed
        List<string> list_villainsKilled = VillainsGameData.Instance.GetVillainsKilled();

        string sentence = "";

        // Obtaining information that we want to show about the villain
        foreach (string villainID in list_villainsKilled)
        {
            string villainName = VillainsGameData.Instance.GetVillainName(villainID);
            string villainFaction = VillainsGameData.Instance.GetVillainFactionName(villainID);
            string villainWhereItWasKilled = VillainsGameData.Instance.GetVillainHexagonID(villainID);
            string villainBook = VillainsGameData.Instance.GetVillainBook(villainID);
            string villainPage = VillainsGameData.Instance.GetVillainPage(villainID);

            sentence += " - " + villainName + " de la facción " + villainFaction + " fue asesinado en " + villainWhereItWasKilled + "\n";
        }

        if (list_villainsKilled.Count == 0)
            sentence = "¡Aun no mataste a ningun villano!";

        text_VillainsKilled.text = sentence;
    }

    public void SetActivationOfFlags()
    {
        PersistentVariables.Instance.ActivateOrNotFlags();
    }

    public void SetActivationOfHexagons()
    {
        PersistentVariables.Instance.ActivateOrNotHexagons();
    }

    public void SetActivationOfHeroWalks()
    {
        PersistentVariables.Instance.ActivateOrNotHeroWalk();
    }

    public void OnClickSaveData()
    {
        DataPersistenceManager.Instance.SaveGame();

        EnableText();
    }

    public void OnClickLoadData()
    {
        DataPersistenceManager.Instance.LoadGame();

        EnableText();
    }


    //Call to enable the text, which also sets the timer
    private void EnableText()
    {
        text_SavedData.enabled = true;
        timeWhenDisappear = Time.time + timeToAppear;
    }

    //We check every frame if the timer has expired and the text should disappear
    void Update()
    {
        if (text_SavedData.enabled && (Time.time >= timeWhenDisappear))
        {
            text_SavedData.enabled = false;
        }
        
        if (red_Arrow.activeSelf == true && (Time.time >= timeWhenDisappearRedArrow))
        {
            red_Arrow.SetActive(false);
        }
    }

    private void EnableRedArrow(string cellName)
    {
        // Changing father and moving hero icon
        red_Arrow.transform.SetParent((GameObject.Find(cellName).transform));
        red_Arrow.transform.localPosition = new Vector3(-65, 35, 0);
        red_Arrow.SetActive(true);
        timeWhenDisappearRedArrow = Time.time + timeToAppearRedArrow;
    }

    // We move the map to focus on the cell wanted
    private void MoveMap(float positionX, float positionY)
    {
        GameObject map = GameObject.Find("Map");
        Vector3 temporalScale = map.transform.localScale;

        // Necessary to change scale to move correctly
        //map.transform.localScale = new Vector3(3, 3, 3);
        //map.transform.localPosition = new Vector3((positionX * -3.19f), (positionY * -2.57f), map.transform.position.z);
        map.transform.localPosition = new Vector3((positionX * -1f), (positionY *-1f), map.transform.position.z);

        // Putting the scale of the user after moving
        //map.transform.localScale = temporalScale;
    }

    public void OnDropdownValueChange_City()
    {
        GameObject cellCity = GameObject.Find(optionsCity[dropdown_City.value - 1]);

        if (cellCity != null)
        {
            MoveMap(cellCity.transform.parent.localPosition.x, cellCity.transform.parent.localPosition.y);

            EnableRedArrow(optionsCity[dropdown_City.value - 1]);
        }
    }

    public void OnDropdownValueChange_Town()
    {
        GameObject cellTown = GameObject.Find(optionsTown[dropdown_Town.value - 1]);

        if(cellTown != null)
        {
            MoveMap(cellTown.transform.parent.localPosition.x, cellTown.transform.parent.localPosition.y);

            EnableRedArrow(optionsTown[dropdown_Town.value - 1]);
        }
    }

    public void OnDropdownValueChange_Village()
    {
        GameObject cellVillage = GameObject.Find(optionsVillage[dropdown_Village.value - 1]);

        if (cellVillage != null)
        {
            MoveMap(cellVillage.transform.parent.localPosition.x, cellVillage.transform.parent.localPosition.y);

            EnableRedArrow(optionsVillage[dropdown_Village.value - 1]);
        }
        
    }

    public void OnDropdownValueChange_Seaport()
    {
        GameObject cellSeaport = GameObject.Find(optionsSeaport[dropdown_Seaport.value - 1]);

        if (cellSeaport != null)
        {
            MoveMap(cellSeaport.transform.parent.localPosition.x, cellSeaport.transform.parent.localPosition.y);

            EnableRedArrow(optionsSeaport[dropdown_Seaport.value - 1]);
        }
        
    }

    public void OnDropdownValueChange_Villain()
    {
        GameObject cellVillain = GameObject.Find(optionsVillain[dropdown_Villain.value - 1]);

        if (cellVillain != null)
        {
            MoveMap(cellVillain.transform.parent.localPosition.x, cellVillain.transform.parent.localPosition.y);

            EnableRedArrow(optionsVillain[dropdown_Villain.value - 1]);
        }
        
    }

    public void OnClickGetInstructions()
    {
        _photoViewerGameObject.SetActive(true);
        
        // Load images in the viewer
        _photoViewer.AddImageData(instructions.ImageDatas);

        // Load Viewer with the page of the villain
        _photoViewer.Show();
        _photoViewer.ShowImage(0);
    }

    public void OnClickDarkSecretRecord()
    {
        int totalPlayers = DataPersistenceManager.Instance.GetTotalDarkSecretPlayers();

        List<TextMeshProUGUI> listTextPlayers = new List<TextMeshProUGUI> { textPlayer1, textPlayer2, textPlayer3, textPlayer4, textPlayer5, textPlayer6 };
        List<Button> listButtonPlayers = new List<Button>() { buttonPlayer1, buttonPlayer2, buttonPlayer3, buttonPlayer4, buttonPlayer5, buttonPlayer6 };
        List<GameObject> listPanelPlayers = new List<GameObject> { panelPlayer1, panelPlayer2, panelPlayer3, panelPlayer4, panelPlayer5, panelPlayer6 };

        for (int i = 0; i < totalPlayers; i++)
        {
            int playerDarkSecret = DataPersistenceManager.Instance.GetPlayerDarkSecret(i);
            listTextPlayers[i].text = DataPersistenceManager.Instance.list_AventurerType[playerDarkSecret];
            Sprite sprite = Resources.Load<Sprite>("DarkSecrets/Icons/" + DataPersistenceManager.Instance.list_AventurerTypeIcon[playerDarkSecret]);
            listButtonPlayers[i].GetComponent<Image>().sprite = sprite;
            listPanelPlayers[i].SetActive(true);
        }
    }

    public void OnClickDarkSecretPlayer(int player)
    {
        PersistentVariables.Instance.ShowingDarkSecret(player, true);
    }

}
