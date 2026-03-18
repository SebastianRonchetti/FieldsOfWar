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
    [SerializeField] int resourceYield;
    List<Unit> unitsInRange;
    bool contestedTerritory, neutralCountdownOn = false, countdownToVictoryRunning = false;

    public void sendResourcesToCurrentlyOccupyingFaction()
    {
        if(contestedTerritory) return;
    }

    void changePondRuler(string faction)
    {
        currentlyOccupyingFaction = faction;
    }

    void OnTriggerEnter(Collider other)
    {
        Unit _unit;
        if(other.TryGetComponent(out _unit))unitsInRange.Add(_unit);
        if(other.gameObject.tag == currentlyOccupyingFaction) return;
        if(currentlyOccupyingFaction == "Neutral") 
        {
            currentlyOccupyingFaction = other.gameObject.tag;
            return;
        }
    }

    void OnTriggerExit(Collider other)
    {
        Unit _unit;
        if(other.TryGetComponent(out _unit))removeFromList(_unit);
    }

    void FixedUpdate()
    {
        if (countdownToVictoryRunning)
        {
            countdownToWin_Progress -= Time.deltaTime;
            if(countdownToWin_Progress <= 0)
            {
                //currently occupying faction wins
            }
        }
        if (neutralCountdownOn)
        {
            countdownToNeutral_Progress -= Time.deltaTime;
            if(countdownToWin_Progress <= 0)
            {
                previouslyOccupyingFaction = "Neutral";
                currentlyOccupyingFaction = "Neutral";
            }
        }
    }

    void alterControl()
    {
        if(!isPondInConflict() && unitsInRange.Count > 0)
        {
            
        }
    }

    void alterCountdowns()
    {
        if(currentlyOccupyingFaction != previouslyOccupyingFaction)
        {
            countdownToWin_Progress = countdownToWin_Minutes * 60;
        }

        if (!isPondInConflict())
        {
            countdownToVictoryRunning = true;
        }

        if (countdownToVictoryRunning)
        {
            countdownToNeutral_Progress = countdownToNeutral_Seconds;
            neutralCountdownOn = false;
        }
    }

    void removeFromList(Unit _unit)
    {
        unitsInRange.Remove(_unit);
    }

    bool isPondInConflict()
    {
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

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(this.transform.position, range);
    }
}
