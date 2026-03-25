using System;
using UnityEngine;

public class CommunicationEvents : ScriptableObject
{
    public static Action<string, float> GatherResource;
    public static Action<string, Unit, bool> AddUnitToFactionList; //faction, unit, isOnTopTrack
    public static Action<GatheringUnit, bool> SetGathererInfo;
    public static Action<Unit> RemoveUnitFromLists;
    public static Action<int, int> updateUI, updateUnitCount; //currentUnitCount, maxUnitCount
    public static Action<string> onFactionDefeated;
    public static Action<float> updateFunds;
    public static Action<float, string> updateTimer; 
    public static Action<int, bool, string> setUnitOrders; //Command(0 = retreat, 1 = hold, 2 = advance, 3 (Unit defined, not broadcasted) = pursue target), isOnTop, faction
    static bool modeIsTimer;
    public static void setMode(bool mode) { modeIsTimer = mode; }
    public static bool getMode() { return modeIsTimer; }
    public static Action onReloadLevel, onLoadMainMenu, onLoadNextLevel;
}