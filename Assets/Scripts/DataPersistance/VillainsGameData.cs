using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Text;
using System;
using UnityEngine.UI;
using UnityEngine.Analytics;

public class VillainInfoByLevels
{
    // Adding ID that will be used to record if the villain has being done by heroes before
    public List<int> list_ID = new List<int>();
    // Next value is used to select the villain in random mode
    // if monster selected randomly has been killed before, select another one.
    public List<bool> list_HasBeingKill = new List<bool>();
    public List<String> list_faction = new List<String>();
    public List<String> list_VillainName = new List<String>();
    public List<String> list_Book = new List<String>();
    public List<String> list_Page = new List<String>();
    public List<String> list_Levels = new List<String>();
    public List<String> list_HexagonID = new List<String>();
}

//[System.Serializable]
public class VillainsGameData : MonoBehaviour, IDataPersistence
{

    public static VillainsGameData Instance { get; private set; }


    private List<VillainInfoByLevels> list_VillainsByLevel = new List<VillainInfoByLevels>();

    private int maximumVillainLevel = 10;

    private void Awake()
    {
        // Creating persistance object
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Instance.LoadingAllVillains();
        }
        else
        {
            UnityEngine.Debug.Log("Destroying");
            Destroy(gameObject);
        }

    }
   
    ////////////////////////////////////////////
    ///          Load and Save function      ///
    ////////////////////////////////////////////
    public void LoadData(GameData data)
    {
        // Not sure why but this one executes before PersistenVariables LoadData
        
        // Removing current data and putting villain data to default
        LoadingAllVillains();

        int counter = 0;
        foreach (string villainID in data.list_VillainsIDs)
        {
            string[] villainIDInfo = villainID.Split('_');
            list_VillainsByLevel[int.Parse(villainIDInfo[0])].list_HasBeingKill[int.Parse(villainIDInfo[1])] = data.list_VillainsBeingKilled[counter];

            Villain[] list_AllVillainCellsInGame = Resources.FindObjectsOfTypeAll<Villain>();

            foreach (Villain villain in list_AllVillainCellsInGame)
            {
                if (villain.gameObject.name == data.list_VillainsCells[counter])
                { 
                    villain.SetVillainHasBeingKilled(data.list_VillainsBeingKilled[counter]);
                    villain.SetFlagMaterial(villainID);
                }
            }
            //GameObject hexagonCell = GameObject.Find(data.list_VillainsCells[counter]);

            //hexagonCell.GetComponent<Villain>().SetVillainHasBeingKilled(data.list_VillainsBeingKilled[counter]);
            //hexagonCell.GetComponent<Villain>().SetFlagMaterial(villainID);

            list_VillainsByLevel[int.Parse(villainIDInfo[0])].list_HexagonID[int.Parse(villainIDInfo[1])] = data.list_VillainsCells[counter];
            counter++;
        }
    }

    public void SaveData(ref GameData data)
    {
        int counter;
        
        // Removing previous Data to store the current state
        data.list_VillainsIDs.Clear();
        data.list_VillainsCells.Clear();
        data.list_VillainsBeingKilled.Clear();

        for (int i = 0; i < maximumVillainLevel; i++)
        {
            counter = 0;
            foreach (bool beingKilled in list_VillainsByLevel[i].list_HasBeingKill)
            {
                if (list_VillainsByLevel[i].list_HexagonID[counter] != "")
                {
                    // Stored level of monster _ position in the list
                    data.list_VillainsIDs.Add(i.ToString() + "_" + counter.ToString());
                    data.list_VillainsCells.Add(list_VillainsByLevel[i].list_HexagonID[counter]);
                    data.list_VillainsBeingKilled.Add(beingKilled);
                }

                counter++;
            }
        }
    }

    // This function is loading 10 excel files
    // Deprecated because now it is only 1 excel file
    // New function is after this one
    public void LoadingAllVillains10Files()
    {
        Debug.Log(Application.streamingAssetsPath);
        
        // There are 10 levels of villains
        for (int i = 1; i < maximumVillainLevel+1; i++)
        {
            string path = Application.streamingAssetsPath + "/ResourceData/nivel" + i.ToString() + ".txt";

            // Necessary to save the file in UTF8 encoding format
            List<string> fileLines = File.ReadAllLines(path, Encoding.UTF8).ToList();
            int countLine = 0;
            VillainInfoByLevels temporalVillainInfo = new VillainInfoByLevels();

            // Reading file
            foreach (string line in fileLines)
            {
                // Removing headers
                if (countLine == 0)
                {
                    countLine++;
                    continue;
                }


                string[] villainInfo = line.Split(';');

                
                // ID always starts with 1
                temporalVillainInfo.list_ID.Add(countLine);
                temporalVillainInfo.list_HasBeingKill.Add(false);
                temporalVillainInfo.list_faction.Add(villainInfo[0]);
                temporalVillainInfo.list_VillainName.Add(villainInfo[1]);
                temporalVillainInfo.list_Book.Add(villainInfo[2]);
                temporalVillainInfo.list_Page.Add(villainInfo[3]);
                temporalVillainInfo.list_Levels.Add(villainInfo[4]);
                temporalVillainInfo.list_HexagonID.Add("");

                countLine++;

            }
            list_VillainsByLevel.Add(temporalVillainInfo);
        }
    }

    public void LoadingAllVillains()
    {
        // Removing previous data of villains
        for (int i = 0; i < maximumVillainLevel; i++)
        {
            VillainInfoByLevels temporalVillainInfo = new VillainInfoByLevels();

            // Initializing list_VillainsByLevel
            temporalVillainInfo.list_ID = new List<int>();
            temporalVillainInfo.list_HasBeingKill = new List<bool>();
            temporalVillainInfo.list_faction = new List<string>();
            temporalVillainInfo.list_VillainName = new List<string>();
            temporalVillainInfo.list_Book = new List<string>();
            temporalVillainInfo.list_Page = new List<string>();
            temporalVillainInfo.list_Levels = new List<string>();
            temporalVillainInfo.list_HexagonID = new List<string>();

            Instance.list_VillainsByLevel.Add(temporalVillainInfo);
        }

        // There are 10 levels of villains
        //string path = Application.streamingAssetsPath + "/VillainInfo/Villains.txt";

        TextAsset file = Resources.Load("VillainInfo/Villains") as TextAsset;
        
        string[] fileLines = file.text.Split('\n');


        // Necessary to save the file in UTF8 encoding format
        //List<string> fileLines = File.ReadAllLines(path, Encoding.UTF8).ToList();

        int countLine = 0;

        // Reading file
        foreach (string line in fileLines)
        {
            // Removing headers
            if (countLine == 0)
            {
                countLine++;
                continue;
            }


            string[] villainInfo = line.Split(';');

            string[] villainLevels = villainInfo[4].Split('-');

            foreach (string levelString in villainLevels)
            {
                int level = int.Parse(levelString)-1;
                
                // ID always starts with 1
                list_VillainsByLevel[level].list_ID.Add(countLine);
                list_VillainsByLevel[level].list_HasBeingKill.Add(false);
                list_VillainsByLevel[level].list_Book.Add(villainInfo[0]);
                list_VillainsByLevel[level].list_VillainName.Add(villainInfo[1]);
                list_VillainsByLevel[level].list_faction.Add(villainInfo[2]);
                list_VillainsByLevel[level].list_Page.Add(villainInfo[3]);
                list_VillainsByLevel[level].list_Levels.Add(villainInfo[4]);
                list_VillainsByLevel[level].list_HexagonID.Add("");
                
            }
            countLine++;
        }
    }

   
    public string SearchingVillain(int level, List<string> factions)
    {
        string villainID = "";
        bool villainFounded = false;
        int limitCounter = -1;

        // Necessary to put a limit here in case that villain is not found after 100 tries
        while (villainFounded == false)
        {
            if (limitCounter > 99)
                break;

            limitCounter++;

            int randomInt = UnityEngine.Random.Range(0, list_VillainsByLevel[level].list_VillainName.Count);
            // Checking if villain is from a faction allowed in the cell
            bool correctFaction = false;
            foreach (string faction in factions)
            {
                if (list_VillainsByLevel[level].list_faction[randomInt] == faction)
                {
                    correctFaction = true;
                    break;
                }
            }
            // Villain is from a faction not alowed in the cell
            if (correctFaction == false)
                continue;

            // Villain has already being killed
            //if (list_VillainsByLevel[level].list_HasBeingKill[randomInt] == true)
            //  continue;

            // Villain is not designated in another hexagon
            if (list_VillainsByLevel[level].list_HexagonID[randomInt] != "")
                continue;


            // Obtaining villain ID
            villainID = level.ToString() + "_" + randomInt.ToString();

            // Finishing loop
            villainFounded = true;
        }

        return villainID;
    }


    

    //*********************************************//
    //              SET Functions                  //
    //              Only 2 can be modified         //
    //*********************************************//

    public void SetHexagonToVillain(string hexagonID, string villainID)
    {
        string[] villainIDInfo = villainID.Split('_');

        list_VillainsByLevel[int.Parse(villainIDInfo[0])].list_HexagonID[int.Parse(villainIDInfo[1])] = hexagonID;
    }

    public void SetVillainHasBeingKilled(bool hasBeingKill, string villainID)
    {
        string[] villainIDInfo = villainID.Split('_');

        list_VillainsByLevel[int.Parse(villainIDInfo[0])].list_HasBeingKill[int.Parse(villainIDInfo[1])] = hasBeingKill;

    }
    //*********************************************//


    //*********************************************//
    //              GET Functions                  //
    //*********************************************//
    public string GetVillainInThatHexagon(string hexagonID)
    {
        string villainID = "";
        int counter;

        // Searching hexagon
        for (int i = 0; i < maximumVillainLevel; i++)
        {
            counter = 0;
            foreach (string hexagonAttachVillain in list_VillainsByLevel[i].list_HexagonID)
            {
                if (hexagonAttachVillain == hexagonID)
                {
                    return i.ToString() + "_" + counter.ToString();
                }
                counter++;
            }

        }

        return villainID;
    }

    public List<string> GetAllHexagonWithVillains()
    {
        List<string> list_Hexagons = new List<string>();

        // Searching hexagon
        for (int i = 0; i < maximumVillainLevel; i++)
        {
            foreach (string hexagonAttachVillain in list_VillainsByLevel[i].list_HexagonID)
            {
                if (hexagonAttachVillain != "")
                {
                    list_Hexagons.Add(hexagonAttachVillain);
                }
            }

        }

        return list_Hexagons;
    }

    public List<string> GetVillainsKilled()
    {
        List<string> list_VillainKilled = new List<string>();
        int counter;

        // Searching hexagon
        for (int i = 0; i < maximumVillainLevel; i++)
        {
            counter = 0;
            foreach (bool hasBeingKilled in list_VillainsByLevel[i].list_HasBeingKill)
            {
                if (hasBeingKilled == true)
                {
                    list_VillainKilled.Add(i.ToString() + "_" + counter.ToString());
                }
                counter++;
            }

        }

        return list_VillainKilled;
    }

    public string GetVillainFactionName(string villainID)
    {
        string[] villainIDInfo = villainID.Split('_');

        return list_VillainsByLevel[int.Parse(villainIDInfo[0])].list_faction[int.Parse(villainIDInfo[1])];
    }

    public string GetVillainName(string villainID)
    {
        string[] villainIDInfo = villainID.Split('_');

        return list_VillainsByLevel[int.Parse(villainIDInfo[0])].list_VillainName[int.Parse(villainIDInfo[1])];
    }

    public string GetVillainBook(string villainID)
    {
        string[] villainIDInfo = villainID.Split('_');

        return list_VillainsByLevel[int.Parse(villainIDInfo[0])].list_Book[int.Parse(villainIDInfo[1])];
    }

    public string GetVillainPage(string villainID)
    {
        string[] villainIDInfo = villainID.Split('_');

        return list_VillainsByLevel[int.Parse(villainIDInfo[0])].list_Page[int.Parse(villainIDInfo[1])];
    }

    public string GetVillainHexagonID(string villainID)
    {
        string[] villainIDInfo = villainID.Split('_');

        return list_VillainsByLevel[int.Parse(villainIDInfo[0])].list_HexagonID[int.Parse(villainIDInfo[1])];
    }
    public bool GetVillainhasBeingKill(string villainID)
    {
        string[] villainIDInfo = villainID.Split('_');

        return list_VillainsByLevel[int.Parse(villainIDInfo[0])].list_HasBeingKill[int.Parse(villainIDInfo[1])];
    }

    //*********************************************//
}
