using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler 
{
    // Where the file is saved
    private string dataDirPath = "";

    // Name of file saved or loaded
    private string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load(string profileID)
    {
        // base case - if the profileID is null, return right away
        if (profileID == null)
        {
            return null;
        }


        // Using Path.Combine to account for different OS's having different path separators
        string fullPath = Path.Combine(dataDirPath, profileID, dataFileName);

        // Data is loaded in this variable
        GameData loadedData = null;

        // Before loading check if file exists
        if (File.Exists(fullPath))
        {
            try
            {
                // Load the serialized data from the file
                string dataToLoad = "";

                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        // Load everything in one tring, necessary to deseralized
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // Deserialize the data from Json back into the C# object
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);


            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to save data to file." + fullPath + "\n" + e);
            }
    }

        return loadedData;
    }

    public void Save(GameData data, string profileID)
    {
        // base case - if the profileID is null, return right away
        if (profileID == null)
        {
            return;
        }

        Debug.Log("Saved on:" + dataDirPath);
        // Using Path.Combine to account for different OS's having different path separators
        string fullPath = Path.Combine(dataDirPath, profileID, dataFileName);

        try
        {
            // Create the directory the file will be written to if it does not already exist
            // It is possible to usie dataDirPath but it is ok as well using fullPath
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // Serialize the C# fame data object into Json
            string dataToStore = JsonUtility.ToJson(data, true);

            // Write the serialized data to the file
            // function "using" controls to close the file and use it only inside the block
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }

        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file." + fullPath + "\n" + e);
        }
    }

    public Dictionary<string, GameData> LoadAllProfiles()
    {
        Dictionary<string, GameData> DictionaryProfile = new Dictionary<string, GameData>();

        // Loop over all directory names in the data directory path
        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();

        foreach (DirectoryInfo dirInfo in dirInfos)
        {
            string profileID = dirInfo.Name;

            // Check if the data file exist in the folder
            // If it does not, then this folder is not a profile and should be skipped
            string fullPath = Path.Combine(dataDirPath, profileID, dataFileName);
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning("Skipping directory when loading all profiles becaise it does not contain data: " + profileID);
                continue;
            }

            // Load the game data for this profile and put it in the dictionary
            GameData profileData = Load(profileID);

            // Ensure the profile data is not null, because if it is
            // then something went worng and we should let ourselves know
            if (profileData != null)
            {
                DictionaryProfile.Add(profileID, profileData);
            }
            else
            {
                Debug.LogError("Tried to load profile but something went worng. ProfileID: " + profileID);
            }
        }

        return DictionaryProfile;
    }

    public string GetMostRecentlyUpdatedProfileId()
    {
        string mostRecentProfileId = null;

        Dictionary<string, GameData> profilesGameData = LoadAllProfiles();
        foreach (KeyValuePair<string, GameData> pair in profilesGameData)
        {
            string profileId = pair.Key;
            GameData gameData = pair.Value;

            // skip this entry if the gamedata is null
            if (gameData == null)
            {
                continue;
            }

            // if this is the first data we've come across that exists, it's the most recent so far
            if (mostRecentProfileId == null)
            {
                mostRecentProfileId = profileId;
            }
            // otherwise, compare to see which date is the most recent
            else
            {
                DateTime mostRecentDateTime = DateTime.FromBinary(profilesGameData[mostRecentProfileId].lastUpdated);
                DateTime newDateTime = DateTime.FromBinary(gameData.lastUpdated);
                // the greatest DateTime value is the most recent
                if (newDateTime > mostRecentDateTime)
                {
                    mostRecentProfileId = profileId;
                }
            }
        }
        return mostRecentProfileId;
    }
}
