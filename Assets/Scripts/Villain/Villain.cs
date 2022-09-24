using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Villain : MonoBehaviour, IDataPersistence
{
    [Header("Data needed if was city before")]
    // Each monster cell has a record if the villain has been killed
    [SerializeField] private GameObject cityBefore;

    // Each monster cell has a record if the villain has been killed
    public bool hasVillainBeingKilled;

    // The flag of the villain
    public GameObject villainFlag;

    // Variable to change the texture of the flag of the villain
    public GameObject villainFlagTexture;

    // factions that can appear in the cell
    public bool factionDwarfCaos;
    public bool factionLizardMen;
    public bool factionCaos;
    public bool factionNoDead;
    public bool factionDarkElfs;
    public bool factionSkavens;
    public bool factionOrcAndGoblins;
    public bool factionMonsters;

    ////////////////////////////////////////////
    ///          Load and Save function      ///
    ////////////////////////////////////////////
    void IDataPersistence.LoadData(GameData data)
    {
        int counter = 0;
        foreach (string cellName in data.list_villainCells)
        { 
            if (cellName == this.gameObject.name)
            {
                factionDwarfCaos = data.list_factionDwarfCaos[counter];
                factionLizardMen = data.list_factionLizardMen[counter];
                factionCaos = data.list_factionCaos[counter];
                factionNoDead = data.list_factionNoDead[counter];
                factionDarkElfs = data.list_factionDarkElfs[counter];
                factionSkavens = data.list_factionSkavens[counter];
                factionOrcAndGoblins = data.list_factionOrcAndGoblins[counter];
                factionMonsters = data.list_factionMonsters[counter];
            }

            counter++;
        }
    }

    void IDataPersistence.SaveData(ref GameData data)
    {
        data.list_villainCells.Add(this.gameObject.name);
        data.list_factionDwarfCaos.Add(factionDwarfCaos);
        data.list_factionLizardMen.Add(factionLizardMen);
        data.list_factionCaos.Add(factionCaos);
        data.list_factionNoDead.Add(factionNoDead);
        data.list_factionDarkElfs.Add(factionDarkElfs);
        data.list_factionSkavens.Add(factionSkavens);
        data.list_factionOrcAndGoblins.Add(factionOrcAndGoblins);
        data.list_factionMonsters.Add(factionMonsters);
    }

    public void PopUpMenu()
    {
        GameObject gameObjectMonsterMenu =  GameObject.Find("CanvasMonsterMenu");
        Canvas canvasMonsterMenu;
        if (gameObjectMonsterMenu != null)
        { 
            canvasMonsterMenu = gameObjectMonsterMenu.GetComponent<Canvas>();

            

            // Activating or disabling menu
            // It will need to be on the popup menu and the button that activates de popup
            if (canvasMonsterMenu.enabled == true)
            {
                canvasMonsterMenu.enabled = false;
            }
            else if (canvasMonsterMenu.enabled == false){
                canvasMonsterMenu.enabled = true;

                PersistentVariables.Instance.UpdateCurrentCell(this.gameObject.name);
            }

            // Activate recovery city button if cell was previously a city
            if (cityBefore != null)
            {
                // Updating Name of place
                string namePlace = cityBefore.name + " \n(Aventura)";
                this.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = namePlace;
                canvasMonsterMenu.GetComponent<VillainMenu>().SetNamePlace(namePlace);
                PersistentVariables.Instance.SetActivateRecoverCityButton(true);
            }
            else
            {
                // Updating Name of place
                canvasMonsterMenu.GetComponent<VillainMenu>().SetNamePlace(this.gameObject.GetComponentInChildren<TextMeshProUGUI>().text);
                PersistentVariables.Instance.SetActivateRecoverCityButton(false);
            }

            if (hasVillainBeingKilled == false)
            {
                PersistentVariables.Instance.SetInteractableGenerateVillainButton(true);
            }
            else
            {
                PersistentVariables.Instance.SetInteractableGenerateVillainButton(false);
            }
            

            // Activate kill button only if villain is assign, heroes are in that cell and villain has not been killed
            if (VillainsGameData.Instance.GetVillainInThatHexagon(this.gameObject.name) != "" && PersistentVariables.Instance.GetHeroesCellCurrentPosName() == this.gameObject.name && hasVillainBeingKilled == false)
            {
                PersistentVariables.Instance.SetInteractableKillVillainButton(true);
            }
            else
            {
                PersistentVariables.Instance.SetInteractableKillVillainButton(false);
            }
            
        }
    }

    public void VillainHasBeingKilled()
    {
        //hasVillainBeingKilled = true;

        // Villains can be only killed if the heroes are in the same cell
        if (PersistentVariables.Instance.GetHeroesCellCurrentPosName() == this.gameObject.name)
        {
            hasVillainBeingKilled = true;
        }
    }

    // This function is just in case the user wants to change the living status of a villain
    public void SetVillainHasBeingKilled(bool beingKilled)
    {
        hasVillainBeingKilled = beingKilled;
    }

    public void SetFlagVisibility(bool showFlag)
    {

        if(PersistentVariables.Instance.GetActivationOfFlags() == true)
            villainFlag.SetActive(showFlag);
    }

    // Putting the correct flag for the villain
    public void SetFlagMaterial(string villainID)
    {
        SetFlagVisibility(true);

        Material villainFlagMaterial;

        if (hasVillainBeingKilled == false)
        {
            villainFlagMaterial = Resources.Load<Material>("FactionFlags/mat_alive/" + VillainsGameData.Instance.GetVillainFactionName(villainID));
        }
        else
        {
            villainFlagMaterial = Resources.Load<Material>("FactionFlags/mat_dead/" + VillainsGameData.Instance.GetVillainFactionName(villainID));
        }

        villainFlagTexture.GetComponent<Renderer>().material = villainFlagMaterial;
    }

    public List<string> GetFactionsAllowed()
    {
        List<string> factions = new List<string>();

        if (this.factionDwarfCaos == true)
            factions.Add("ENANOS DEL CAOS");

        if (this.factionLizardMen == true)
            factions.Add("HOMBRES LAGARTO");
        
        if (this.factionCaos == true)
            factions.Add("CAOS");
        
        if (this.factionNoDead == true)
            factions.Add("NO MUERTOS");
        
        if (this.factionDarkElfs == true)
            factions.Add("ELFOS OSCUROS");
        
        if (this.factionSkavens == true)
            factions.Add("SKAVEN");
        
        if (this.factionOrcAndGoblins == true)
            factions.Add("ORCOS Y GOBLINS");

        if(this.factionMonsters == true)
            factions.Add("MONSTRUOS"); 

        return factions;
    }


    public void RecoverCity()
    {
        cityBefore.GetComponent<City>().RecoverCity();
    }

}
