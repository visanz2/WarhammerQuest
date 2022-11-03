using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CityMenu : MonoBehaviour
{
    [Header("Name of place in the map")]
    [SerializeField] private TextMeshProUGUI text_NamePlace;

    [Header("Reputation Texts")]
    [SerializeField] private TextMeshProUGUI text_PermanentReputation;
    [SerializeField] private TextMeshProUGUI text_TemporalReputation;
    [SerializeField] private TextMeshProUGUI text_TotalReputation;

    [Header("Dark Secrets Images and Texts")]
    [SerializeField] private TextMeshProUGUI text_Player1;
    [SerializeField] private TextMeshProUGUI text_Player2;
    [SerializeField] private TextMeshProUGUI text_Player3;
    [SerializeField] private GameObject image_Player1;
    [SerializeField] private GameObject image_Player2;
    [SerializeField] private GameObject image_Player3;

    private int temporalReputation = 0;
    private int permanentReputation = 0;
    private bool readingDarkSecret = false;

    public void SetNamePlace(string namePlace)
    {
        text_NamePlace.text = namePlace;
    }

    public void GetReputation()
    {
        string hexagonID = PersistentVariables.Instance.GetCurrentCell();

        GameObject hexagonCell = GameObject.Find(hexagonID);

        permanentReputation = hexagonCell.GetComponent<City>().GetReputation();
        text_PermanentReputation.text = permanentReputation.ToString();

        temporalReputation = 0;
        text_TemporalReputation.text = temporalReputation.ToString();
        
        SetTotalReputation();
    }

    // Always is increased by 1
    public void OnClickPlusButton()
    {
        temporalReputation++;
        text_TemporalReputation.text = temporalReputation.ToString();
        SetTotalReputation();
    }

    // Always is decreased by 1
    public void OnClickMinusButton()
    {
        if ((permanentReputation - temporalReputation) == 0)
            return;

        temporalReputation--;
        text_TemporalReputation.text = temporalReputation.ToString();
        SetTotalReputation();
    }

    public void SetTotalReputation()
    {
        text_TotalReputation.text = (permanentReputation + temporalReputation).ToString();
    }

    public void OnClickDestroyCity()
    {
        string hexagonID = PersistentVariables.Instance.GetCurrentCell();

        GameObject hexagonCell = GameObject.Find(hexagonID);

        hexagonCell.GetComponent<City>().DestroyCity();
    }

    public void OnClickConvertedCity()
    {
        string hexagonID = PersistentVariables.Instance.GetCurrentCell();

        GameObject hexagonCell = GameObject.Find(hexagonID);

        hexagonCell.GetComponent<City>().ConvertCityToVillain();
    }

    public void OnClickRecoverCity()
    {
        string hexagonID = PersistentVariables.Instance.GetCurrentCell();

        GameObject hexagonCell = GameObject.Find(hexagonID);

        hexagonCell.GetComponent<City>().RecoverCity();
    }

    public void OnClickDarkSecrets()
    {
        string hexagonID = PersistentVariables.Instance.GetCurrentCell();

        GameObject hexagonCell = GameObject.Find(hexagonID);

        int player1 = hexagonCell.GetComponent<City>().GetPlayer1();
        int player2 = hexagonCell.GetComponent<City>().GetPlayer2();
        int player3 = hexagonCell.GetComponent<City>().GetPlayer3();

        int playerDarkSecret = DataPersistenceManager.Instance.GetPlayerDarkSecret(player1);
        text_Player1.text = DataPersistenceManager.Instance.list_AventurerType[playerDarkSecret];
        Sprite sprite = Resources.Load<Sprite>("DarkSecrets/Icons/" + DataPersistenceManager.Instance.list_AventurerTypeIcon[playerDarkSecret]);
        image_Player1.GetComponent<Image>().sprite = sprite;


        if (player2 != -1)
        {
            playerDarkSecret = DataPersistenceManager.Instance.GetPlayerDarkSecret(player2);
            text_Player2.text = DataPersistenceManager.Instance.list_AventurerType[playerDarkSecret];
            image_Player2.GetComponent<Image>().sprite = Resources.Load<Sprite>("DarkSecrets/Icons/" + DataPersistenceManager.Instance.list_AventurerTypeIcon[playerDarkSecret]);
            GameObject.Find("PanelPlayer2").SetActive(true);
        }
        else
            GameObject.Find("PanelPlayer2").SetActive(false);

        if (player3 != -1)
        {
            playerDarkSecret = DataPersistenceManager.Instance.GetPlayerDarkSecret(player3);
            text_Player3.text = DataPersistenceManager.Instance.list_AventurerType[playerDarkSecret];
            image_Player3.GetComponent<Image>().sprite = Resources.Load<Sprite>("DarkSecrets/Icons/" + DataPersistenceManager.Instance.list_AventurerTypeIcon[playerDarkSecret]);
            GameObject.Find("PanelPlayer3").SetActive(true);
        }
        else
            GameObject.Find("PanelPlayer3").SetActive(false);
    }

    public void HideOrActiveMenu()
    {
        if (readingDarkSecret == true)
        {
            this.gameObject.SetActive(true);
            readingDarkSecret = false;
        }
    }

    public void OnClickShowDarkSecret1()
    {
        string hexagonID = PersistentVariables.Instance.GetCurrentCell();

        GameObject hexagonCell = GameObject.Find(hexagonID);
        int player = hexagonCell.GetComponent<City>().GetPlayer1();
        PersistentVariables.Instance.ShowingDarkSecret(player, false);

        readingDarkSecret = true;
        this.gameObject.SetActive(false);

    }

    public void OnClickShowDarkSecret2()
    {
        string hexagonID = PersistentVariables.Instance.GetCurrentCell();

        GameObject hexagonCell = GameObject.Find(hexagonID);
        int player = hexagonCell.GetComponent<City>().GetPlayer2();
        PersistentVariables.Instance.ShowingDarkSecret(player, false);

        readingDarkSecret = true;
        this.gameObject.SetActive(false);
    }

    public void OnClickShowDarkSecret3()
    {
        string hexagonID = PersistentVariables.Instance.GetCurrentCell();

        GameObject hexagonCell = GameObject.Find(hexagonID);
        int player = hexagonCell.GetComponent<City>().GetPlayer3();
        PersistentVariables.Instance.ShowingDarkSecret(player, false);

        readingDarkSecret = true;
        this.gameObject.SetActive(false);
    }

    public void OnClickDoneDarkSecret1()
    {
        string hexagonID = PersistentVariables.Instance.GetCurrentCell();

        GameObject hexagonCell = GameObject.Find(hexagonID);
        int player = hexagonCell.GetComponent<City>().GetPlayer1();
        DataPersistenceManager.Instance.MovePlayerToNextCityDarkSecret(player);

        DataPersistenceManager.Instance.UpdateDarkSecrets();
    }

    public void OnClickDoneDarkSecret2()
    {
        string hexagonID = PersistentVariables.Instance.GetCurrentCell();

        GameObject hexagonCell = GameObject.Find(hexagonID);
        int player = hexagonCell.GetComponent<City>().GetPlayer2();
        DataPersistenceManager.Instance.MovePlayerToNextCityDarkSecret(player);

        DataPersistenceManager.Instance.UpdateDarkSecrets();
    }

    public void OnClickDoneDarkSecret3()
    {
        string hexagonID = PersistentVariables.Instance.GetCurrentCell();

        GameObject hexagonCell = GameObject.Find(hexagonID);
        int player = hexagonCell.GetComponent<City>().GetPlayer3();
        DataPersistenceManager.Instance.MovePlayerToNextCityDarkSecret(player);

        DataPersistenceManager.Instance.UpdateDarkSecrets();
    }
}
