using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Navigation")]
    [SerializeField] private SaveSlotMenu saveSlotMenu;

    [Header("Menu Buttons")]
    [SerializeField] private Button buttonNewGame;
    [SerializeField] private Button buttonContinueGame;
    [SerializeField] private Button buttonLoadGame;

    public void Start()
    {
        // Do not allow the player to load data if there is not data saved previously
        if (!DataPersistenceManager.Instance.HasGameData())
        {
            buttonContinueGame.interactable = false;
            buttonLoadGame.interactable = false;
        }
    }
    public void OnNewGameClick()
    {
        /*if (campaingName.text == "")
        {
            campaigNameError.enabled = true;
        }
        else
        {
            campaigNameError.enabled = false;
        }*/

        saveSlotMenu.ActivateMenu(false);
        this.DeactivateMenu();

        // Old Method 
        /*DisableMenuButtons();
        // Create a new game - which will initialize our game data
        DataPersistenceManager.Instance.NewGame();

        // Load the gameplay scene - which will in turn save the game because of 
        // OnSceneUnloaded() in the DataPersistenceManager
        SceneManager.LoadSceneAsync("SampleScene"); 
        */ 
    }


    public void OnLoadGameClick()
    {
        saveSlotMenu.ActivateMenu(true);
        this.DeactivateMenu();
    }

    public void OnContinueGameClick()
    {
        DisableMenuButtons();
        // Load the next scene - which will n turn load the game because of
        // OnSceneLoaded() in the DataPersistenceManager
        SceneManager.LoadSceneAsync("SampleScene");
    }

    private void DisableMenuButtons()
    {
        buttonNewGame.interactable = false;
        buttonLoadGame.interactable = false;
    }

    public void ActivateMenu()
    {
        this.gameObject.SetActive(true);
    }

    public void DeactivateMenu()
    {
        this.gameObject.SetActive(false);
    }
}
