using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
public class LocalLevelManager : MonoBehaviour
{
    [SerializeField] GameObject victoryBanner, defeatBanner, gameEndUI, timer, nextLevel;
    TextMeshPro timerText;
    bool suddenDeath = false;
    [SerializeField] float timeToDefeat;
    [SerializeField] int unitsThatAreAboutToDie, totalUnitsBottomCount = 0;

    void Awake()
    {
        timerText = timer.GetComponent<TextMeshPro>();
        CommunicationEvents.onFactionDefeated += onFactionDefeated;
    }

    void onFactionDefeated(string faction)
    {
        if (faction == "Player")
        {
            gameEndUI.SetActive(true);
            defeatBanner.SetActive(true);
            victoryBanner.SetActive(false);
            nextLevel.SetActive(false);
        }
        else
        {
            gameEndUI.SetActive(true);
            nextLevel.SetActive(true);
            victoryBanner.SetActive(true);
            defeatBanner.SetActive(false);
        }
    }

    public void onNextLevelPressed()
    {
        CommunicationEvents.onLoadNextLevel?.Invoke();
    }

    public void onBackToMainMenuPressed()
    {
        CommunicationEvents.onLoadMainMenu?.Invoke();
    }

    public void onRetryPressed()
    {
        CommunicationEvents.onReloadLevel?.Invoke();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void FixedUpdate()
    {
        if (suddenDeath)
        {
            timeToDefeat -= Time.deltaTime;
            updateTimer();
        }

        if (timeToDefeat <= 0)
        {
            gameOver("Player");
        }
    }

    void updateTimer()
    {
        string mins = (timeToDefeat / 60).ToString();
        string sec = (timeToDefeat % 60).ToString();
        timerText.text = $"{mins}:{sec}";
    }

    void unitsThatAreToDie(bool increase)
    {
        if (increase)
        {
            totalUnitsBottomCount++;
            if (totalUnitsBottomCount >= unitsThatAreAboutToDie)
            {
                suddenDeath = true;
            }
        }
        else
        {
            totalUnitsBottomCount--;
        }
    }

    void gameOver(string _faction) // _faction = the defeated faction
    {
        gameEndUI.SetActive(true);
        Time.timeScale = 0f;
        if (_faction == "Player")
        {
            defeatBanner.SetActive(true);
            victoryBanner.SetActive(false);
        }
        else
        {
            victoryBanner.SetActive(true);
            defeatBanner.SetActive(false);
        }
    }
}
