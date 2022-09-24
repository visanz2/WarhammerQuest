using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class City: MonoBehaviour, IDataPersistence
{
    [Header("Surrounding Cells for Reputation")]
    // Cell to calculate Reputation
    [SerializeField] private GameObject surroundingCell1;
    [SerializeField] private GameObject surroundingCell2;
    [SerializeField] private GameObject surroundingCell3;
    [SerializeField] private GameObject surroundingCell4;
    [SerializeField] private GameObject surroundingCell5;
    [SerializeField] private GameObject surroundingCell6;


    [Header("GameObject for city destroy or converted")]
    [SerializeField] private GameObject cityOriginal;
    [SerializeField] private GameObject cityVillainCellConversion;
    [SerializeField] private GameObject cityDestroy;

    [Header("City disabled by event")]
    [SerializeField] private bool isCityDisabled;

    [Header("Dark secrets flags activated in this cell")]
    [SerializeField] private GameObject flagDarkSecret1;
    [SerializeField] private GameObject flagDarkSecret2;
    [SerializeField] private GameObject flagDarkSecret3;
    [SerializeField] private GameObject flagDarkSecret1Material;
    [SerializeField] private GameObject flagDarkSecret2Material;
    [SerializeField] private GameObject flagDarkSecret3Material;

    [SerializeField] private int playerDarkSecret1;
    [SerializeField] private int playerDarkSecret2;
    [SerializeField] private int playerDarkSecret3;


    private bool hasCityDisapper = false;


    public void LoadData(GameData data)
    {
        int positionInList = data.list_citiesConverted.IndexOf(gameObject.name);

        // City was converted
        if (positionInList > -1)
        {
            ConvertCityToVillain();
            return;
        }

        positionInList = data.list_citiesDestroyed.IndexOf(gameObject.name);

        // City was destroyed
        if (positionInList > -1)
        {
            DestroyCity();
        }
    }

    public void SaveData(ref GameData data)
    {
        if (hasCityDisapper == true)
        {
            int positionInList = data.list_citiesConverted.IndexOf(gameObject.name);

            // City was converted
            if (positionInList == -1 && cityVillainCellConversion.activeSelf == true)
            {
                data.list_citiesConverted.Add(gameObject.name);
                data.list_citiesDestroyed.Remove(gameObject.name);
                return;
            }

            positionInList = data.list_citiesDestroyed.IndexOf(gameObject.name);

            // City was destroyed
            if (positionInList == -1 && cityVillainCellConversion.activeSelf == false)
            {
                data.list_citiesConverted.Remove(gameObject.name);
                data.list_citiesDestroyed.Add(gameObject.name);
            }
        }
        else
        {
            data.list_citiesConverted.Remove(gameObject.name);
            data.list_citiesDestroyed.Remove(gameObject.name);
        }
    }

    public void PopUpMenu()
    {
        GameObject gameObjectCityMenu = GameObject.Find("CanvasMenuCity");
        Canvas canvasCityMenu;

        if(gameObjectCityMenu != null)
        { 
            canvasCityMenu = gameObjectCityMenu.GetComponent<Canvas>();

            // Updating Name of place
            canvasCityMenu.GetComponent<CityMenu>().SetNamePlace(this.gameObject.GetComponentInChildren<TextMeshProUGUI>().text);

            // Activating or disabling menu
            // It will need to be on the popup menu and the button that activates de popup
            if (canvasCityMenu.enabled == true)
            {
                canvasCityMenu.enabled = false;
            }
            else if (canvasCityMenu.enabled == false)
            {
                canvasCityMenu.enabled = true;

                PersistentVariables.Instance.UpdateCurrentCell(this.gameObject.name);
            }

            // If city is disabled, not possible to move there
            if (this.GetCityDeactivated() == true)
            {
                GameObject.Find("ButtonMoveHereCity").GetComponent<UnityEngine.UI.Button>().interactable = false;
            }
            else
            {
                GameObject.Find("ButtonMoveHereCity").GetComponent<UnityEngine.UI.Button>().interactable = true;
            }

            // If city destroy or converted, not possible to see the reputation
            if (hasCityDisapper == true)
            {
                GameObject.Find("ButtonCheckReputation").GetComponent<UnityEngine.UI.Button>().interactable = false;
                GameObject.Find("ButtonRecoverCity").GetComponent<UnityEngine.UI.Button>().interactable = true;
            }
            else
            {
                GameObject.Find("ButtonCheckReputation").GetComponent<UnityEngine.UI.Button>().interactable = true;
                GameObject.Find("ButtonRecoverCity").GetComponent<UnityEngine.UI.Button>().interactable = false;
            }

            // Not possible to destroy or convert to villain a cell if heroes are in and the city is already destroyed
            if (hasCityDisapper == false && PersistentVariables.Instance.GetHeroesCellCurrentPosName() != this.gameObject.name)
            {
                GameObject.Find("ButtonDestroyCity").GetComponent<UnityEngine.UI.Button>().interactable = true;
                GameObject.Find("ButtonConvertToVillain").GetComponent<UnityEngine.UI.Button>().interactable = true;
            }
            else
            {
                GameObject.Find("ButtonDestroyCity").GetComponent<UnityEngine.UI.Button>().interactable = false;
                GameObject.Find("ButtonConvertToVillain").GetComponent<UnityEngine.UI.Button>().interactable = false;
            }

            // Only possible to see dark secrets if they are in cell
            if (flagDarkSecret1.activeInHierarchy == false && flagDarkSecret2.activeInHierarchy == false && flagDarkSecret3.activeInHierarchy == false)
            {

                GameObject.Find("ButtonDarkSecret").GetComponent<UnityEngine.UI.Button>().interactable = false;
            }
            else
            {
                //And the player are in that cell
                if (PersistentVariables.Instance.GetHeroesCellCurrentPosName() == this.gameObject.name)
                {
                    GameObject.Find("ButtonDarkSecret").GetComponent<UnityEngine.UI.Button>().interactable = true;
                }
                else
                {
                    GameObject.Find("ButtonDarkSecret").GetComponent<UnityEngine.UI.Button>().interactable = false;
                }
            }

            

        }
    }


    // Return reputation of the cells associated to this city
    public int GetReputation()
    {
        int reputation = 0;
        if (surroundingCell1!= null && surroundingCell1.GetComponent<Villain>().hasVillainBeingKilled == true)
            reputation++;

        if (surroundingCell2 != null && surroundingCell2.GetComponent<Villain>().hasVillainBeingKilled == true)
            reputation++;

        if (surroundingCell3 != null && surroundingCell3.GetComponent<Villain>().hasVillainBeingKilled == true)
            reputation++;

        if (surroundingCell4 != null && surroundingCell4.GetComponent<Villain>().hasVillainBeingKilled == true)
            reputation++;

        if (surroundingCell5 != null && surroundingCell5.GetComponent<Villain>().hasVillainBeingKilled == true)
            reputation++;

        if (surroundingCell6 != null && surroundingCell6.GetComponent<Villain>().hasVillainBeingKilled == true)
            reputation++;

        Debug.Log("Reputation: " + reputation.ToString());
        return reputation;
    }

    public void DestroyCity()
    {
        GameObject.Find("ButtonCheckReputation").GetComponent<UnityEngine.UI.Button>().interactable = false;

        cityDestroy.SetActive(true);
        //cityOriginal.SetActive(false);
        hasCityDisapper = true;
    }

    public void ConvertCityToVillain()
    {
        string namePlace = this.gameObject.name + " \n(Aventura)";
        cityVillainCellConversion.GetComponentInChildren<TextMeshProUGUI>().text = namePlace;
        cityVillainCellConversion.SetActive(true);
        cityDestroy.SetActive(false);
        cityOriginal.SetActive(false);
        hasCityDisapper = true;
    }

    public void RecoverCity()
    {
        hasCityDisapper = false;
        cityVillainCellConversion.SetActive(false);
        cityDestroy.SetActive(false);
        cityOriginal.SetActive(true);
        GameObject.Find("ButtonCheckReputation").GetComponent<UnityEngine.UI.Button>().interactable = true;
    }

    public void SetMaterialDarkSecret(string flagPlayer, int player)
    {
        Material darkSecretFlagMaterial = Resources.Load<Material>("DarkSecrets/FlagsMaterial/" + flagPlayer);
        if (flagDarkSecret1.activeInHierarchy == true && flagDarkSecret2.activeInHierarchy == true) // Activating third flag
        {
            playerDarkSecret3 = player;
            flagDarkSecret3.SetActive(true);
            flagDarkSecret3Material.GetComponent<Renderer>().material = darkSecretFlagMaterial;
        }
        else if (flagDarkSecret1.activeInHierarchy == true)
        {
            playerDarkSecret2 = player;
            playerDarkSecret3 = -1;
            flagDarkSecret2.SetActive(true);
            flagDarkSecret2Material.GetComponent<Renderer>().material = darkSecretFlagMaterial;
        }
        else
        {
            playerDarkSecret3 = -1;
            playerDarkSecret2 = -1;
            playerDarkSecret1 = player;
            flagDarkSecret1.SetActive(true);
            flagDarkSecret1Material.GetComponent<Renderer>().material = darkSecretFlagMaterial;
        }
    }

    public bool hasAnyDarkSecretActive()
    {
        return flagDarkSecret1.activeInHierarchy || flagDarkSecret2.activeInHierarchy || flagDarkSecret3.activeInHierarchy;
    }

    public int GetPlayer1()
    {
        return playerDarkSecret1;
    }

    public int GetPlayer2()
    {
        return playerDarkSecret2;
    }

    public int GetPlayer3()
    {
        return playerDarkSecret3;
    }

    public void SetDisablingCity(bool isDisable)
    {
        isCityDisabled = isDisable;
    }

    public bool GetCityDeactivated()
    {
        // If city has reputation, it cannot be disabled
        if (this.GetReputation() > 0)
            return false;

        return this.isCityDisabled;
    }

}

