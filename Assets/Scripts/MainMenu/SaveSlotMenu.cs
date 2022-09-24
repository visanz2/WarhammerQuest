using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotMenu : MonoBehaviour
{
    [Header("Menu Navigation")]
    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private NewCampaign newCampaignMenu;
    


    [Header("Menu Buttons")]
    [SerializeField] private Button buttonBack;

    private SaveSlot[] saveSlots;

    private bool isLoadingGame = false;

    private void Awake()
    {
        saveSlots = this.GetComponentsInChildren<SaveSlot>();
    }

    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        // Disable all buttons
        DisableMenuButtons();

        if (this.isLoadingGame == false)
        {
            Debug.Log("SaveSlot campaign" + saveSlot.GetProfileID().ToString());
            newCampaignMenu.SetprofileID(saveSlot.GetProfileID());

            // Move to panel NewCampaign
            newCampaignMenu.ActivateMenu();
            this.DeactivateMenu();
        }
        else
        {
            // Update the selected profile id to be used for data persistence
            DataPersistenceManager.Instance.ChangeSelectedProfileID(saveSlot.GetProfileID());
            // Load the scene which will in turn save the game because of OnSceneUnLoaded() in the DataPersitenceManager
            SceneManager.LoadSceneAsync("SampleScene");
        }
        
        



        /*

        if (!isLoadingGame)
        {
            // if a new game
            //GameData profileData = null;
            //profilesGameData.TryGetValue(saveSlot.GetProfileID(), out profileData);

            string campaignName = "default";
            // create a new game which will initialize our data to a clean slate
            DataPersistenceManager.Instance.NewGame(campaignName);
        }*/

        
    }

    public void OnBackClicked()
    {
        mainMenu.ActivateMenu();
        this.DeactivateMenu();
    }

    public void ActivateMenu(bool isLoadingGame)
    {
        // Set this menu to be active
        this.gameObject.SetActive(true);

        // Set mode
        this.isLoadingGame = isLoadingGame;

        // Load all profiles that exist
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.Instance.GetAllProfilesGameData();

        // Loop through each save slot in the UI and set the content appropiately
        foreach (SaveSlot saveSlot in saveSlots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileID(), out profileData);

            saveSlot.SetData(profileData);

            if (profileData == null && isLoadingGame)
            {
                saveSlot.SetInteractable(false);
            }
            else
            {
                saveSlot.SetInteractable(true);
            }
        }
    }

    public void DeactivateMenu ()
    {
        this.gameObject.SetActive(false); 
    }

    public void DisableMenuButtons()
    {
        foreach (SaveSlot saveSlot in saveSlots)
        {
            saveSlot.SetInteractable(false);
        }

        buttonBack.interactable = false;
    }
}
