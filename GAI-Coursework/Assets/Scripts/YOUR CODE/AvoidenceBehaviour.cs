using UnityEngine;

public class AvoidanceBehaviour : SteeringBehaviour
{
    public float avoidDistance = 1.5f;
    public float avoidForce = 15f;
    public float agentAvoidRadius = 1.2f;

    private Map map;
    
    private void Awake()
    {
        map = GameData.Instance.Map;

    }

    public override Vector3 UpdateBehaviour(SteeringAgent agent)
    {
        Vector3 force = Vector3.zero;
             
           bool avoiding = false;


        foreach (var other in AllyAgentGroupManager.allAllies)
        {
       
            if (other == agent) continue;

            Vector3 distanceToOther = agent.transform.position - other.transform.position;
            float distance = distanceToOther.magnitude;

            if (distance < agentAvoidRadius)
            {
                // closer the agents are to one another increases force to avoid each other
                force += distanceToOther.normalized * (avoidForce / distance);
                avoiding = true;
            }
        }

       
        if (!avoiding)
        {
            return Vector3.zero;
        }

        return force;
    }
}
