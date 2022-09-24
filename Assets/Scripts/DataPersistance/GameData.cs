using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // Recoding the last time played this campaign
    public long lastUpdated;

    // Name of the campaign
    public string campaignName;

    // Turn of the game
    public int gameTurn;

    // Villains information
    public List<string> list_VillainsIDs;
    public List<bool> list_VillainsBeingKilled;
    public List<string> list_VillainsCells;

    // Places visited by heroes
    public List<string> list_cellIniName;
    public List<string> list_cellEndName;

    // Option data in the main menu
    public bool showFlags;
    public bool showHex;
    public bool showHeroWalk;

    // Saving factions data from each cell
    public List<string> list_villainCells;
    public List<bool> list_factionDwarfCaos;
    public List<bool> list_factionLizardMen;
    public List<bool> list_factionCaos;
    public List<bool> list_factionNoDead;
    public List<bool> list_factionDarkElfs;
    public List<bool> list_factionSkavens;
    public List<bool> list_factionOrcAndGoblins;
    public List<bool> list_factionMonsters;

    // Record of cities converted and destroyed
    public List<string> list_citiesConverted;
    public List<string> list_citiesDestroyed;

    // Cities disabled this turn
    public List<string> list_CityDisabled;

    // Campaign Events already checked
    public List<string> list_IDCampaignEvents;

    // Dark secrets 
    public List<int> list_DarkSecretHeroes;
    public List<int> list_DarkSecretCardPosition;



    // The values defined in this constructor will be the default values
    // the game starts with when there is no data to load
    public GameData(string campaignNameCreated, List<int> darkSecretHeroeos, List<int> darkSecretCardPosition)
    {
        campaignName = campaignNameCreated;

        gameTurn = 0;

        list_VillainsIDs = new List<string>();
        list_VillainsBeingKilled = new List<bool>();
        list_VillainsCells = new List<string>();
        list_cellIniName = new List<string>();
        list_cellEndName = new List<string>();

        showFlags = true;
        showHex = true;
        showHeroWalk = false;

        // Saving factions data from each cell
        list_villainCells = new List<string>();
        list_factionDwarfCaos = new List<bool>();
        list_factionLizardMen = new List<bool>();
        list_factionCaos = new List<bool>();
        list_factionNoDead = new List<bool>();
        list_factionDarkElfs = new List<bool>();
        list_factionSkavens = new List<bool>();
        list_factionOrcAndGoblins = new List<bool>();
        list_factionMonsters = new List<bool>();

        list_citiesConverted = new List<string>();
        list_citiesDestroyed = new List<string>();

        list_CityDisabled = new List<string>();

        list_IDCampaignEvents = new List<string>();

        list_DarkSecretHeroes = darkSecretHeroeos;
        list_DarkSecretCardPosition = darkSecretCardPosition;

}

    public int GethowManyVillainsHasBeingKilled()
    {
        int howManyVillainsKilled = 0;

        foreach (bool hasBeingKilled in list_VillainsBeingKilled)
        {
            if (hasBeingKilled == true)
            {
                howManyVillainsKilled++;
            }
        }

        return howManyVillainsKilled;
    }
}
