using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewCampaign : MonoBehaviour
{
    [Header("Menu Navigation")]
    [SerializeField] private SaveSlotMenu saveSlotMenu;

    [Header("Input UI")]
    [SerializeField] private TMP_InputField campaingName;
    [SerializeField] private TMP_Text campaigNameError;

    [Header("Menu Buttons")]
    [SerializeField] private Button buttonBack;

    [Header("Dropdowns Dark Secrets")]
    [SerializeField] private TMP_Dropdown dropdown_Player_1;
    [SerializeField] private TMP_Dropdown dropdown_Player_2;
    [SerializeField] private TMP_Dropdown dropdown_Player_3;
    [SerializeField] private TMP_Dropdown dropdown_Player_4;
    [SerializeField] private TMP_Dropdown dropdown_Player_5;
    [SerializeField] private TMP_Dropdown dropdown_Player_6;

    private int dropdown_Player_1_value = -1;
    private int dropdown_Player_2_value = -1;
    private int dropdown_Player_3_value = -1;
    private int dropdown_Player_4_value = -1;
    private int dropdown_Player_5_value = -1;
    private int dropdown_Player_6_value = -1;

    private string profileID;

    public void Awake()
    {
        // LOADING DROPDOWN DATA HERE
    }


    public void OnClickSubmitCampaignName()
    {
        if (campaingName.text == "")
        {
            campaigNameError.enabled = true;
            campaigNameError.text = "¡La campaña no tiene nombre!";
            return;
        }
        else
        {
            campaigNameError.enabled = false;
        }

        // Checking if different dark secrets have been selected
        // The no selection of value is equal to -1
        List<int> list_DarkSecrets = new List<int>();
        if (dropdown_Player_1_value != -1)
            list_DarkSecrets.Add(dropdown_Player_1_value);

        if (dropdown_Player_2_value != -1)
            list_DarkSecrets.Add(dropdown_Player_2_value);

        if (dropdown_Player_3_value != -1)
            list_DarkSecrets.Add(dropdown_Player_3_value);

        if (dropdown_Player_4_value != -1)
            list_DarkSecrets.Add(dropdown_Player_4_value);

        if (dropdown_Player_5_value != -1)
            list_DarkSecrets.Add(dropdown_Player_5_value);

        if (dropdown_Player_6_value != -1)
            list_DarkSecrets.Add(dropdown_Player_6_value);

        if (list_DarkSecrets.Count != list_DarkSecrets.Distinct().Count())
        {
            campaigNameError.enabled = true;
            campaigNameError.text = "¡Secretos oscuros compartido por jugadores!";
            return;
        }


        // Checking dropdowns
        if (dropdown_Player_1_value != -1)
        {
            DataPersistenceManager.Instance.AddPlayerDarkSecret(dropdown_Player_1_value); // the key value in dictionary
            DataPersistenceManager.Instance.AddPlayersCityOnDarkSecrets(0); // Initializing that player has the first city to visit
        }
        if (dropdown_Player_2_value != -1)
        {
            DataPersistenceManager.Instance.AddPlayerDarkSecret(dropdown_Player_2_value);
            DataPersistenceManager.Instance.AddPlayersCityOnDarkSecrets(0); // Initializing that player has the first city to visit
        }

        if (dropdown_Player_3_value != -1)
        {
            DataPersistenceManager.Instance.AddPlayerDarkSecret(dropdown_Player_3_value);
            DataPersistenceManager.Instance.AddPlayersCityOnDarkSecrets(0); // Initializing that player has the first city to visit
        }
            
        if (dropdown_Player_4_value != -1)
        {
            DataPersistenceManager.Instance.AddPlayerDarkSecret(dropdown_Player_4_value);
            DataPersistenceManager.Instance.AddPlayersCityOnDarkSecrets(0); // Initializing that player has the first city to visit
        }
            

        if (dropdown_Player_5_value != -1)
        {
            DataPersistenceManager.Instance.AddPlayerDarkSecret(dropdown_Player_5_value);
            DataPersistenceManager.Instance.AddPlayersCityOnDarkSecrets(0); // Initializing that player has the first city to visit
        }
            

        if (dropdown_Player_6_value != -1)
        {
            DataPersistenceManager.Instance.AddPlayerDarkSecret(dropdown_Player_6_value);
            DataPersistenceManager.Instance.AddPlayersCityOnDarkSecrets(0); // Initializing that player has the first city to visit
        }

        Debug.Log("New campaign " + this.profileID.ToString());
        // Update the selected profile id to be used for data persistence
        DataPersistenceManager.Instance.ChangeSelectedProfileID(this.profileID);

        // Setting variable to show prologue
        DataPersistenceManager.Instance.SetNewCampaing(true);

        // create a new game which will initialize our data to a clean slate
        DataPersistenceManager.Instance.NewGame(campaingName.text);
        

        // Load the scene which will in turn save the game because of OnSceneUnLoaded() in the DataPersitenceManager
        SceneManager.LoadSceneAsync("SampleScene");
    }


    public void OnBackClicked()
    {
        saveSlotMenu.ActivateMenu(false);
        buttonBack.interactable = true;
        this.DeactivateMenu();
    }

    public void SetprofileID(string profileID)
    {
        this.profileID = profileID;
    }
    public void ActivateMenu()
    {
        // Initializing dropdowns
        dropdown_Player_1.ClearOptions();
        dropdown_Player_2.ClearOptions();
        dropdown_Player_3.ClearOptions();
        dropdown_Player_4.ClearOptions();
        dropdown_Player_5.ClearOptions();
        dropdown_Player_6.ClearOptions();

        List<string> none = new List<string> { "Ninguno" };
        none.AddRange(DataPersistenceManager.Instance.list_AventurerType);
        dropdown_Player_1.AddOptions(none);
        dropdown_Player_2.AddOptions(none);
        dropdown_Player_3.AddOptions(none);
        dropdown_Player_4.AddOptions(none);
        dropdown_Player_5.AddOptions(none);
        dropdown_Player_6.AddOptions(none);

        /*dropdown_Player_1.AddOptions(DataPersistenceManager.Instance.list_AventurerType);
        dropdown_Player_2.AddOptions(DataPersistenceManager.Instance.list_AventurerType);
        dropdown_Player_3.AddOptions(DataPersistenceManager.Instance.list_AventurerType);
        dropdown_Player_4.AddOptions(DataPersistenceManager.Instance.list_AventurerType);
        dropdown_Player_5.AddOptions(DataPersistenceManager.Instance.list_AventurerType);
        dropdown_Player_6.AddOptions(DataPersistenceManager.Instance.list_AventurerType);
        */

        this.gameObject.SetActive(true);
    }

    public void DeactivateMenu()
    {
        this.gameObject.SetActive(false);
    }

    // Dropdown on change functions
    public void OnDropdownPlayer1Selection()
    {
        dropdown_Player_1_value = dropdown_Player_1.value - 1;
        //Debug.Log("dropdown_Player_1_value: " + dropdown_Player_1_value.ToString());

        if (dropdown_Player_1_value == -1)
        {
            dropdown_Player_2.interactable = false;
            dropdown_Player_3.interactable = false;
            dropdown_Player_4.interactable = false;
            dropdown_Player_5.interactable = false;
            dropdown_Player_6.interactable = false;

            dropdown_Player_2.value = -1;
            dropdown_Player_3.value = -1;
            dropdown_Player_4.value = -1;
            dropdown_Player_5.value = -1;
            dropdown_Player_6.value = -1;

            dropdown_Player_2_value = -1;
            dropdown_Player_3_value = -1;
            dropdown_Player_4_value = -1;
            dropdown_Player_5_value = -1;
            dropdown_Player_6_value = -1;
        }
    }

    public void OnDropdownPlayer2Selection()
    {
        dropdown_Player_2_value = dropdown_Player_2.value - 1;
        if (dropdown_Player_2_value == -1)
        {
            dropdown_Player_3.interactable = false;
            dropdown_Player_4.interactable = false;
            dropdown_Player_5.interactable = false;
            dropdown_Player_6.interactable = false;

            dropdown_Player_3.value = -1;
            dropdown_Player_4.value = -1;
            dropdown_Player_5.value = -1;
            dropdown_Player_6.value = -1;

            dropdown_Player_3_value = -1;
            dropdown_Player_4_value = -1;
            dropdown_Player_5_value = -1;
            dropdown_Player_6_value = -1;
        }
    }

    public void OnDropdownPlayer3Selection()
    {
        dropdown_Player_3_value = dropdown_Player_3.value - 1;

        if (dropdown_Player_3_value == -1)
        {
            dropdown_Player_4.interactable = false;
            dropdown_Player_5.interactable = false;
            dropdown_Player_6.interactable = false;

            dropdown_Player_4.value = -1;
            dropdown_Player_5.value = -1;
            dropdown_Player_6.value = -1;

            dropdown_Player_4_value = -1;
            dropdown_Player_5_value = -1;
            dropdown_Player_6_value = -1;
        }
    }

    public void OnDropdownPlayer4Selection()
    {
        dropdown_Player_4_value = dropdown_Player_4.value - 1;

        if (dropdown_Player_4_value == -1)
        {
            dropdown_Player_5.interactable = false;
            dropdown_Player_6.interactable = false;

            dropdown_Player_5.value = -1;
            dropdown_Player_6.value = -1;

            dropdown_Player_5_value = -1;
            dropdown_Player_6_value = -1;
        }
    }

    public void OnDropdownPlayer5Selection()
    {
        dropdown_Player_5_value = dropdown_Player_5.value - 1;

        if (dropdown_Player_5_value == -1)
        {
            dropdown_Player_6.interactable = false;

            dropdown_Player_6.value = -1;

            dropdown_Player_6_value = -1;
        }
    }

    public void OnDropdownPlayer6Selection()
    {
        dropdown_Player_6_value = dropdown_Player_6.value - 1;

    }
}
