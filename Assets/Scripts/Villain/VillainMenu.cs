using PhotoViewer.Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VillainMenu : MonoBehaviour
{
    // Objects in the same panel
    public GameObject textField;
    public GameObject inputField;

    // Where the villain name is displayed
    public GameObject textFieldVillainName;
    public GameObject imageIconFaction;
    public GameObject textFieldVillainBook;
    public GameObject textFieldVillainPage;
    public GameObject imageIsDeadBackground;

    // Canvas that will be enabled or disabled
    public GameObject panelMainMenuVillain;
    public GameObject panelSelectingVillain;
    public GameObject panelShowingVillain;

    // Factions allowed checkbox
    public Toggle toggle_FactionDwarfCaos;
    public Toggle toggle_FactionLizardMen;
    public Toggle toggle_FactionCaos;
    public Toggle toggle_FactionNoDead;
    public Toggle toggle_FactionDarkElfs;
    public Toggle toggle_FactionSkavens;
    public Toggle toggle_FactionOrcsAndGoblins;
    public Toggle toggle_FactionMonsters;

    public TextMeshProUGUI Text_NamePlace;
    [Header("Prefab to load Villain card")]
    [SerializeField] private GameObject _photoViewerGameObject = null;
    [SerializeField] private PhotoViewer.Scripts.PhotoViewer _photoViewer = null;
    //[SerializeField] private TestImageLibrary _imageLibrary = null;

    public void SetNamePlace(string namePlace)
    {
        Text_NamePlace.text = namePlace;
    }

    public void KillVillainOnThisCell()
    {
        string hexagonID = PersistentVariables.Instance.GetCurrentCell();

        string villainID = VillainsGameData.Instance.GetVillainInThatHexagon(hexagonID);

        if (villainID == "")
            return;

        VillainsGameData.Instance.SetVillainHasBeingKilled(true, villainID);

        GameObject hexagonCell = GameObject.Find(hexagonID);

        // Informing cell what happen to the villain contained in that cell
        hexagonCell.GetComponent<Villain>().VillainHasBeingKilled();
        hexagonCell.GetComponent<Villain>().SetFlagMaterial(villainID);

        // Deactivate kill button
        PersistentVariables.Instance.SetInteractableKillVillainButton(false);
        PersistentVariables.Instance.SetInteractableGenerateVillainButton(false);
    }

    // Search Villain by level
    public void SearchingVillain()
    {
        // Reading data in the input field
        string villainLevelText = inputField.GetComponent<TMP_InputField>().text;

        if (villainLevelText == "")
        {
            textField.GetComponent<TMP_Text>().enabled = true;
            return;
        }


        // Check if value is bigger than 10, if bigger show that needs to be lower
        int villainLevel = int.Parse(villainLevelText);

        if (villainLevel <= 0 || villainLevel > 10)
        {
            textField.GetComponent<TMP_Text>().text = "El n伹ero introducido debe estar entre 1 y 10";
            textField.GetComponent<TMP_Text>().enabled = true;
            return;
        }
        else
        {
            textField.GetComponent<TMP_Text>().enabled = false;
        }

        string hexagonID = PersistentVariables.Instance.GetCurrentCell();

        // Removing previous villains in this cell
        string villainID = VillainsGameData.Instance.GetVillainInThatHexagon(hexagonID);

        if (villainID != "")
        {
            VillainsGameData.Instance.SetHexagonToVillain("", villainID);
        }

        // Factions Allowed, checking information in the cell
        GameObject hexagonCell = GameObject.Find(hexagonID);
        List<string> factions = hexagonCell.GetComponent<Villain>().GetFactionsAllowed();

        if (factions.Count <= 0)
        {
            textField.GetComponent<TMP_Text>().text = "No se ha encontrado ninguna faccion permitida!";
            textField.GetComponent<TMP_Text>().enabled = true;
            return;
        }
        else
        {
            textField.GetComponent<TMP_Text>().enabled = false;
        }

        // Found villain
        villainID = VillainsGameData.Instance.SearchingVillain(villainLevel - 1, factions);

        if (villainID == "")
        {
            textField.GetComponent<TMP_Text>().text = "No se encontro villano que cumpla los requisitos de nivel y facciones.";
            textField.GetComponent<TMP_Text>().enabled = true;
            return;
        }
        else
        {
            textField.GetComponent<TMP_Text>().enabled = false;
        }

        // Set hexagonID to the villain assigned
        Debug.Log("CELL: " + PersistentVariables.Instance.GetCurrentCell() + " villainID: " + villainID);
        VillainsGameData.Instance.SetHexagonToVillain(PersistentVariables.Instance.GetCurrentCell(), villainID);

        // Obtaining data from villain
        string villainName = VillainsGameData.Instance.GetVillainName(villainID);
        string villainBook = VillainsGameData.Instance.GetVillainBook(villainID);
        string villainPage = VillainsGameData.Instance.GetVillainPage(villainID);

        // Puttin information of villain to be watch by the user
        textFieldVillainName.GetComponent<TMP_Text>().text = villainName;
        textFieldVillainBook.GetComponent<TMP_Text>().text = "Libro: " + villainBook;
        textFieldVillainPage.GetComponent<TMP_Text>().text = "Pagina: " + villainPage;

        // Adding Icon
        try
        {
            imageIconFaction.GetComponent<Image>().sprite = Resources.Load<Sprite>("FactionIcon/" + VillainsGameData.Instance.GetVillainFactionName(villainID));
        }
        catch (System.Exception e)
        {
            Debug.Log("Icon did not find: " + e.Message);
        }

        // Adding Flag to the correspondent cell
        hexagonCell.GetComponent<Villain>().SetFlagMaterial(villainID);

        // Disabling dead image
        imageIsDeadBackground.GetComponent<Image>().enabled = false;

        // deactivate canvas and activate villainInfo Canvas
        panelSelectingVillain.SetActive(false);
        panelShowingVillain.SetActive(true);

        // I will not do next to do for now
        // TODO - In case there are not villains in that level, show that needs to be used another level
    }

    public void OnGeneratingVillain()
    {
        panelMainMenuVillain.SetActive(false);
        panelSelectingVillain.SetActive(true);
        panelShowingVillain.SetActive(false);
    }

    // Shows information of villain
    public void VillainInfo()
    {
        string hexagonID = PersistentVariables.Instance.GetCurrentCell();

        string villainID = VillainsGameData.Instance.GetVillainInThatHexagon(hexagonID);

        if (villainID == "")
        {
            panelMainMenuVillain.SetActive(false);
            panelSelectingVillain.SetActive(true);
            panelShowingVillain.SetActive(false);
        }
        else
        {
            // Loading villain data
            string villainName = VillainsGameData.Instance.GetVillainName(villainID);
            string villainBook = VillainsGameData.Instance.GetVillainBook(villainID);
            string villainPage = VillainsGameData.Instance.GetVillainPage(villainID);
            bool hasBeingKilled = VillainsGameData.Instance.GetVillainhasBeingKill(villainID);

            // Adding Icon
            try
            {
                imageIconFaction.GetComponent<Image>().sprite = Resources.Load<Sprite>("FactionIcon/" + VillainsGameData.Instance.GetVillainFactionName(villainID));
            }
            catch (System.Exception e)
            {
                Debug.Log("Icon did not find: " + e.Message);
            }

            // Putting information of villain to be watch by the user
            textFieldVillainName.GetComponent<TMP_Text>().text = villainName;
            textFieldVillainBook.GetComponent<TMP_Text>().text = "Libro: " + villainBook;
            textFieldVillainPage.GetComponent<TMP_Text>().text = "Página: " + villainPage;

            if (hasBeingKilled == false)
            {
                imageIsDeadBackground.GetComponent<Image>().enabled = false;
            }
            else
            {
                imageIsDeadBackground.GetComponent<Image>().enabled = true;
            }

            panelMainMenuVillain.SetActive(false);
            panelSelectingVillain.SetActive(false);
            panelShowingVillain.SetActive(true);
        }
    }

    public void GetFactions()
    {
        // Obteining information of hexagon pressed
        string hexagonID = PersistentVariables.Instance.GetCurrentCell();
        GameObject hexagonCell = GameObject.Find(hexagonID);

        // Adding Flag to the correspondent cell
        toggle_FactionDwarfCaos.isOn = hexagonCell.GetComponent<Villain>().factionDwarfCaos;
        toggle_FactionLizardMen.isOn = hexagonCell.GetComponent<Villain>().factionLizardMen;
        toggle_FactionCaos.isOn = hexagonCell.GetComponent<Villain>().factionCaos;
        toggle_FactionNoDead.isOn = hexagonCell.GetComponent<Villain>().factionNoDead;
        toggle_FactionDarkElfs.isOn = hexagonCell.GetComponent<Villain>().factionDarkElfs;
        toggle_FactionSkavens.isOn = hexagonCell.GetComponent<Villain>().factionSkavens;
        toggle_FactionOrcsAndGoblins.isOn = hexagonCell.GetComponent<Villain>().factionOrcAndGoblins;
        toggle_FactionMonsters.isOn = hexagonCell.GetComponent<Villain>().factionMonsters;

    }

    public void SetFactions()
    {
        string hexagonID = PersistentVariables.Instance.GetCurrentCell();
        GameObject hexagonCell = GameObject.Find(hexagonID);

        // Adding Flag to the correspondent cell
        hexagonCell.GetComponent<Villain>().factionDwarfCaos = toggle_FactionDwarfCaos.isOn;
        hexagonCell.GetComponent<Villain>().factionLizardMen = toggle_FactionLizardMen.isOn;
        hexagonCell.GetComponent<Villain>().factionCaos = toggle_FactionCaos.isOn;
        hexagonCell.GetComponent<Villain>().factionNoDead = toggle_FactionNoDead.isOn;
        hexagonCell.GetComponent<Villain>().factionDarkElfs = toggle_FactionDarkElfs.isOn;
        hexagonCell.GetComponent<Villain>().factionSkavens = toggle_FactionSkavens.isOn;
        hexagonCell.GetComponent<Villain>().factionOrcAndGoblins = toggle_FactionOrcsAndGoblins.isOn;
        hexagonCell.GetComponent<Villain>().factionMonsters = toggle_FactionMonsters.isOn;
    }

    public void OnClickRecoverCity()
    {
        string hexagonID = PersistentVariables.Instance.GetCurrentCell();
        GameObject hexagonCell = GameObject.Find(hexagonID);

        hexagonCell.GetComponent<Villain>().RecoverCity();
    }

    public void OnClickGetVillainCard()
    {
        _photoViewerGameObject.SetActive(true);
        string hexagonID = PersistentVariables.Instance.GetCurrentCell();

        string villainID = VillainsGameData.Instance.GetVillainInThatHexagon(hexagonID);

        // Get Faction of villain
        string villainBookName = VillainsGameData.Instance.GetVillainBook(villainID);
        int villainPage = int.Parse(VillainsGameData.Instance.GetVillainPage(villainID));

        // Loading faction book
        // There are 10 levels of villains

        TestImageLibrary villainFlagMaterial = Resources.Load<TestImageLibrary>("VillainsCards/" + villainBookName);

        // Get Page where villain appears

        // Load images in the viewer
        _photoViewer.AddImageData(villainFlagMaterial.ImageDatas);

        // Load Viewer with the page of the villain
        _photoViewer.Show();
        _photoViewer.ShowImage(villainPage-1);
    }
}
