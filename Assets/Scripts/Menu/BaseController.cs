using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    [SerializeField] protected string faction;
    protected StoreManager storeManager;
    [SerializeField] protected GameObject topTrack, bottomTrack, selectedTrack, activeResourceNode, spawnTop, spawnBottom;
    [SerializeField] protected bool selectedTopTrack = true;
    [SerializeField] protected ResourceNode[] nodes;
    [SerializeField] public int maxNumberOfTroops = 16;
    [SerializeField] protected List<Unit> ActiveUnits;
    [SerializeField] protected List<Transform> waypointsTop, waypointsBottom;

    public virtual void Awake()
    {
        storeManager = gameObject.GetComponent<StoreManager>();//get store component on own GameObject
        if (ActiveUnits == null)
        {
            ActiveUnits = new List<Unit>();
        }
        else
        {
            foreach (Unit unit in ActiveUnits)
            {
                if (unit.gameObject.TryGetComponent<GatheringUnit>(out _))
                {
                    addGathererToUnitList((GatheringUnit)unit, unit.isOnTopTrack);
                }
                else
                {
                    addUnitToUnitList(faction, unit, unit.isOnTopTrack);

                }
            }
        }
        CommunicationEvents.AddUnitToFactionList += addUnitToUnitList;
        CommunicationEvents.RemoveUnitFromLists += removeUnitFromList;
        CommunicationEvents.SetGathererInfo += addGathererToUnitList;
    }

    public virtual void Start()
    {
        faction = gameObject.tag;
        selectedTrack = topTrack;
        selectedTopTrack = false;
        alternateSelectedTrack();
        storeManager = gameObject.GetComponent<StoreManager>();
    }

    public void alternateSelectedTrack()
    {
        if (!selectedTopTrack)
        {
            activeResourceNode = nodes[0].gameObject;
            selectedTrack = topTrack;
            selectedTopTrack = true;
        }
        else
        {
            selectedTrack = bottomTrack;
            activeResourceNode = nodes[1].gameObject;
            selectedTopTrack = false;
        }
    }

    public virtual void addUnitToUnitList(string _faction, Unit unit, bool isOnTopTrack)
    {
        if (unit.faction != faction) return;
        if (!ActiveUnits.Contains(unit))
        {
            ActiveUnits.Add(unit);
        }
        else
        {
            unit.Spawn(faction, isOnTopTrack, gameObject);
        }

        if (isOnTopTrack)
        {
            unit.gameObject.GetComponent<NavigationAgent>().setWaypoints(waypointsTop);
        }
        else
        {
            unit.gameObject.GetComponent<NavigationAgent>().setWaypoints(waypointsBottom);
        }
        
        if (faction != "Player") return;
        CommunicationEvents.updateUI?.Invoke(ActiveUnits.Count, maxNumberOfTroops);
    }

    public void addGathererToUnitList(GatheringUnit unit, bool isOnTopTrack)
    {
        addUnitToUnitList(unit.faction, unit, isOnTopTrack);
        if (unit.faction != faction) return;
        if (isOnTopTrack)
        {
            unit.setCastleAndGatherNode(spawnTop, nodes[0].gameObject);
        }
        else
        {
            unit.setCastleAndGatherNode(spawnBottom, nodes[1].gameObject);
        }
    }

    public virtual void removeUnitFromList(Unit unit)
    {
        ActiveUnits.Remove(unit);
    }

    public void purchaseUnit(int _orderInList)
    {
        if(ActiveUnits.Count < maxNumberOfTroops)
        {
            bool r = storeManager.trySpawnUnit(_orderInList, selectedTopTrack);
        }
    }
    

}