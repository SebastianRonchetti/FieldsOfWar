using UnityEngine;
using UnityEngine.UI;
public class FlagZone : MonoBehaviour
{
    /// <summary>
    /// DEPRECATED 
    /// See scripts/buildings/pond for timer and capture the flag logic
    /// </summary>
    float timerForDomination, playerFactionCountdown, enemyFactionCountdown;
    bool occupiedByEnemy, occupiedByPlayer, playerFactionCountdownOn, enemyFactionCountdownOn;
    Sprite countdownBar;
    Collider zone;

    void Update()
    {
        if (playerFactionCountdown > 0 && playerFactionCountdownOn)
        {
            playerFactionCountdown -= Time.deltaTime;
        }


        if (enemyFactionCountdown > 0 && enemyFactionCountdownOn)
        {
            enemyFactionCountdown -= Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        string advancingFaction = other.gameObject.tag;

        if (advancingFaction == "Player")
        {
            if (enemyFactionCountdownOn)
            {
                enemyFactionCountdownOn = false;
                playerFactionCountdownOn = true;

                playerFactionCountdown = timerForDomination;    
            }
        }

        if (advancingFaction == "Enemy")
        {
            if (playerFactionCountdownOn)
            {
                enemyFactionCountdownOn = true;
                playerFactionCountdownOn = false;

                enemyFactionCountdown = timerForDomination;    
            }
        }

        if (!occupiedByPlayer) occupiedByPlayer = true;
    }
}