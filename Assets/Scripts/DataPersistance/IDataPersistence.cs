using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// It is an interface
public interface IDataPersistence
{
    void LoadData(GameData data);

    void SaveData(ref GameData data); 
}
