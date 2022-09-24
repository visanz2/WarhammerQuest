using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileID = "";

    [Header("Content")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;

    [SerializeField] private TextMeshProUGUI campaignName;
    [SerializeField] private TextMeshProUGUI howManyVillainsKilled;

    private Button saveSlotButton;

    private void Awake()
    {
        saveSlotButton = this.GetComponent<Button>();
    }

    public void SetData(GameData data)
    {
        // There is not data for this profileID
        if (data == null)
        {
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
        }
        else // There is data for this profileID
        {
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);

            this.campaignName.text = data.campaignName;
            this.howManyVillainsKilled.text = "Villanos asesinados: " + data.GethowManyVillainsHasBeingKilled().ToString();
        }
    }

    public void SetProfileID(string nameCampaign)
    {
        this.profileID = nameCampaign;
    }

    public string GetProfileID()
    {
        return this.profileID;
    }

    public void SetInteractable(bool isInteractable)
    {
        saveSlotButton.interactable = isInteractable;
    }
}
