using NUnit.Framework;
using UnityEngine;

public class SeparationBehaviour : SteeringBehaviour
{
    public float separationRadius = 2f;   // Distance for separation to activate
    public float separationForce = 5f; 
    public float straglerVanishDistance = 40f;  // how far untill a troop dies

    private Map map; 

    public override Vector3 UpdateBehaviour(SteeringAgent agent)
    {
        
        if (map == null)
            map = GameData.Instance.Map;

        Vector3 totalPushDirection = Vector3.zero;
        int nearAgentCount = 0;

        foreach (var otherAgent in GameData.Instance.allies)
        {
            if (otherAgent == agent) continue;  // makes sure the agent isnt itself that it exists and not the only agent
            if (otherAgent == null) continue;
            if (!otherAgent.gameObject.activeInHierarchy) continue;

            float distance = Vector3.Distance(agent.transform.position, otherAgent.transform.position);

            
            if (distance < separationRadius && distance > 0f)
            {
                Vector3 awayDirection = (agent.transform.position - otherAgent.transform.position).normalized;
                totalPushDirection += awayDirection * (separationForce / distance);
                nearAgentCount++;
            }
                                    //else if (distance > straglerVanishDistance)          // anti stragler fail safe 
                                 //{ 
        //ignore this please    //    gameObject.SetActive(false);                      // ran into issue when groups split into 2 alot would die
                                 //    Debug.Log(agent.name + " straggled");             
                                  //}       
        }       
        if (nearAgentCount > 0)
        {
            // calculates avrage combined force 
            totalPushDirection = totalPushDirection / nearAgentCount; 
        }  

        return totalPushDirection;

    }
}
