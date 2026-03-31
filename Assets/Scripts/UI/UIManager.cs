using JetBrains.Annotations;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    //playerManagerReference PMR; //This is a stand in for a class being worked on, final names won't necesarily reflect what's written here.
    //storeManagerReference SMR; //This is a stand in for a class being worked on, final names won't necesarily reflect what's written here.
    public TMP_Text numberOfUnitsText;
    public TMP_Text goldAmountText;
    //public GameManager gameManager;
    public static UIManager Instance;
    AudioSource audioSource;
    AudioClip music;
    bool paused = false;
    LevelManager levelManager;
    //
    private bool isTopTrackSelected = true;
    public CommandsGroup commandsGroup;
    private int topFloorUnitCount = 0, bottomFloorUnitCount = 0, maxCount = 16;
    public TMP_Text timerText;
    
    void instantiate()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Awake()
    {
        instantiate();
        audioSource = GetComponent<AudioSource>();
        levelManager = LevelManager.Instance;
        CommunicationEvents.updateFunds += UpdateCurrentGoldAmount;
        CommunicationEvents.updateUnitCount += UpdateCurrentNumOfUnits;
        CommunicationEvents.updateTimer += UpdateTimerDisplay;
        audioSource.loop = true;
        audioSource.clip = music;
        audioSource.Play();
    }

    void Start()
    {
        //PMR = getComponent<playerManagerReference>();
        //SMR = getComponent<storeManagerReference>();
        //subscribe UpdateCurrentGoldAmount and UpdateCurrentNumberOfUnits to event that will tell them what to display
    }

    private void UpdateCurrentGoldAmount(float currentGoldAmount)
    {
        string currentGoldAmountStr = currentGoldAmount.ToString();
        goldAmountText.text = currentGoldAmountStr;
    }
    private void UpdateCurrentNumOfUnits(int current, int max)
    {
            numberOfUnitsText.text = current.ToString("00") + "/" + max.ToString("00");
    }

    // public void issueCommand() // 0: Advance; 1: Halt; 2: Retreat;
    // {
    //send out an event to all subscriptors with the selected command
    // }

    //0 = retreat, 1 = defend, 2 = attack
    public void AttackCommand()
    {
        //GameManager.currrentAction = 2;
       // CommunicationEvents.setUnitOrders?.Invoke(2, true, "Player");
       CommunicationEvents.setUnitOrders?.Invoke(2, isTopTrackSelected, "Player");
    }
    public void DefendCommand()
    {
        //  GameManager.currrentAction = 1;

        //CommunicationEvents.setUnitOrders?.Invoke(1, true, "Player");

        CommunicationEvents.setUnitOrders?.Invoke(1, isTopTrackSelected, "Player");

    }

    public void RetreatCommand()
    {
       // GameManager.currrentAction = 0;
      //  CommunicationEvents.setUnitOrders?.Invoke(0, true, "Player");
        CommunicationEvents.setUnitOrders?.Invoke(0, isTopTrackSelected, "Player");

    }

    public void alternateSpawnPoint()
    {
        //PMR.alternateSpawn();
    }

    public void purchaseUnit(int placeInList)
    {
        //
    }

    public void UpdateTimerDisplay(float timeToVictoryLeft, string controllingFaction)
    {
        int min = Mathf.FloorToInt(timeToVictoryLeft / 60);
        int sec = Mathf.FloorToInt(timeToVictoryLeft % 60);
        string timerTextContent = string.Format("{0:0}:{01:00}", min, sec);

        switch (controllingFaction)
        {
            case "Neutral":
            timerText.color = new Color32(224, 131, 54, 255);
            timerText.text = "Neutral";
                break;
            case "Enemy":
            timerText.color = new Color32(191, 27, 6, 255);
            timerText.text = $"{timerTextContent} until Enemy forces win";
                break;
            case "Player":
            timerText.color = new Color32(9, 128, 23, 255);
            timerText.text = $"{timerTextContent} until Player forces win";
                break;
        }
    }

    public void pause()
    {
        if (paused)
        {
            Time.timeScale = 1f;
            return;
        }
        Time.timeScale = 0f;
    }

    public void retryLevel()
    {
        
    }

    public void goToNextLevel()
    {

    }

    public void gotToMainMenu()
    {
        
    }
    
    //
    private void RefreshUnitDisplay()
    {
        int currentCount = isTopTrackSelected ? topFloorUnitCount : bottomFloorUnitCount;

        numberOfUnitsText.text = currentCount.ToString("00") + "/" + maxCount.ToString("00");
    }
    public void SelectTopTrack()
    {
        isTopTrackSelected = true; 
        commandsGroup.ResetCommandsVisual();
        RefreshUnitDisplay();

    }

    public void SelectBottomTrack()
    {
        isTopTrackSelected = false;
        commandsGroup.ResetCommandsVisual();
        RefreshUnitDisplay();
        
    }
    //


    public void AddUnitToTopFloor()
    {
        topFloorUnitCount++;
        RefreshUnitDisplay();
    }

    public void AddUnitToBottomFloor()
    {
        bottomFloorUnitCount++;
        RefreshUnitDisplay();
    }

    public void removeUnitFromList(Unit unit)
    {
        if (unit.isOnTopTrack)
        {
            RemoveUnitFromTopFloor();
        } else
        {
            RemoveUnitFromBottomFloor();
        }
    }

    void RemoveUnitFromTopFloor()
    {
        topFloorUnitCount --;
        RefreshUnitDisplay();
    }

    void RemoveUnitFromBottomFloor()
    {
        bottomFloorUnitCount --;
        RefreshUnitDisplay();
    }
}
