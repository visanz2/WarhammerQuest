using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForDarkSecret : MonoBehaviour
{
    public void OnButtonClosedClicked() {
        Debug.Log("PRESSED");

        PersistentVariables.Instance.isDarkSecretHasBeenRead = true;
    } // these methods will cause WaitForAction to break
}
