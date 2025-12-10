using System.Collections.Generic;
using UnityEngine;

public static class AllyAgentGroupManager
{

    public static AllyAgent leader;
    public static List<AllyAgent> allAllies = new List<AllyAgent>();

}

//this holds a list of the active troops on the scene and a leader troop managed by the enum in Ally Agent script