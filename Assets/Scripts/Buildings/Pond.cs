using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
This script needs to keep track of two distinct timers. One timer should be able to pause, resume and reset
depending on which faction is controlling it.
when there are no units of any faction within range of the pond and after a short while
it will reset the timer to victory and set the controlling faction to "Neutral"
*/

public class Pond : ResourceNode
{
    [SerializeField] float countdownToWin_Minutes, countdownToNeutral_Seconds, range;
    float countdownToWin_Progress, countdownToNeutral_Progress;
    [SerializeField] string currentlyOccupyingFaction, previouslyOccupyingFaction;
    [SerializeField] List<Unit> unitsInRange;
    bool contestedTerritory, neutralCountdownOn = false, countdownToVictoryRunning = false;

    void Awake()
    {
        unitsInRange = new List<Unit>();
        CommunicationEvents.RemoveUnitFromLists += removeFromList;
        currentlyOccupyingFaction = "Neutral";
    }

    public void sendResourcesToCurrentlyOccupyingFaction()
    {
        if(contestedTerritory) return;
    }

    void OnTriggerEnter(Collider other)
    {

        //adds units that come in range to a list.
        //if the incoming unit isnt of the same faction as the currently controlling faction
        //then it moves to alter control function
        Unit _unit;
        if(other.TryGetComponent(out _unit))
        {
            unitsInRange.Add(_unit);
            _unit.onDeath += removeFromList;
        }
        if(other.gameObject.tag == currentlyOccupyingFaction) return;
        
        alterControl();
    }

    void OnTriggerExit(Collider other)
    {
        //removes units from list when they exit the trigger area
        Unit _unit;
        if(other.TryGetComponent(out _unit))removeFromList(_unit);
    }

    void FixedUpdate()
    {
        if(countdownToVictoryRunning && neutralCountdownOn)
        {
            throw new System.Exception("There are two countdowns on!");
        }

        if (countdownToVictoryRunning)
        {
            countdownToWin_Progress -= Time.deltaTime;
            if(countdownToWin_Progress <= 0)
            {
                switch (currentlyOccupyingFaction)
                {
                    case "Player":
                    CommunicationEvents.onFactionDefeated("Enemy");
                        break;
                    case "Enemy":
                    CommunicationEvents.onFactionDefeated("Player");
                        break;
                }
            }
        }
        else if (neutralCountdownOn)
        {
            countdownToNeutral_Progress -= Time.deltaTime;
            if(countdownToWin_Progress <= 0)
            {
                previouslyOccupyingFaction = "Neutral";
                currentlyOccupyingFaction = "Neutral";
            }
        }
        CommunicationEvents.updateTimer?.Invoke(countdownToWin_Progress, currentlyOccupyingFaction);
    }

    void alterControl()
    {
        if(!isPondInConflict() || currentlyOccupyingFaction == "Neutral")
        {
            if(unitsInRange.Count > 0)
            {
                currentlyOccupyingFaction = unitsInRange[0].gameObject.tag;
            } 
            
            if(neutralCountdownOn) 
            {
                alterNeutralCountdown();
                return;
            }
        }

        alterTovictoryCountdown(isPondInConflict());
        
    }

    void alterNeutralCountdown()
    {
        if(!neutralCountdownOn) countdownToNeutral_Progress = countdownToNeutral_Seconds;
        neutralCountdownOn = !neutralCountdownOn;
    }

    void alterTovictoryCountdown(bool conflict)
    {
        if(currentlyOccupyingFaction != previouslyOccupyingFaction || currentlyOccupyingFaction == "Neutral")
        {
            countdownToVictoryRunning = false;
            countdownToWin_Progress = countdownToWin_Minutes * 60;
        }

        if (!conflict)
        {
            countdownToVictoryRunning = true;
        } 
        else
        {
            countdownToVictoryRunning = false;
            previouslyOccupyingFaction = currentlyOccupyingFaction;
        }

        if (countdownToVictoryRunning)
        {
            previouslyOccupyingFaction = "Neutral";
            countdownToNeutral_Progress = countdownToNeutral_Seconds;
            neutralCountdownOn = false;
        }
    }

    void removeFromList(Unit _unit)
    {
        unitsInRange.Remove(_unit);
        alterControl();
    }

    bool isPondInConflict()
    {
        if(areAllUnitsSameFaction()) return false;
        foreach(Unit unit in unitsInRange)
        {
            if (unit.gameObject.tag != currentlyOccupyingFaction)
            {
                previouslyOccupyingFaction = currentlyOccupyingFaction;
                return true;
            }
        }
        return false;
    }

    bool areAllUnitsSameFaction()
    {
        if(unitsInRange.Count == 1) return true;
        for(int i = 1; i < unitsInRange.Count; i++)
        {
            if(unitsInRange[i].gameObject.tag != unitsInRange[0].gameObject.tag)
            {
                return false;
            }
        }
        return true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(this.transform.position, range);
    }

    #region Testing
    void switchVictoryCountdownForPlayer()
    {
        
    }

    
    void switchVictoryCountdownForEnemy()
    {
        
    }

    void switchNeutralCountdown()
    {
        
    }
    #endregion
}
