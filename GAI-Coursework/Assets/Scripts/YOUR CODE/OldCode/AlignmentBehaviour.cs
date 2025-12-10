using UnityEngine;

public class AlignmentBehaviour : SteeringBehaviour
{
    public float neighborRadius = 1.6f;
    public float alignmentForce = 12f;

    public override Vector3 UpdateBehaviour(SteeringAgent agent)
    {
        Vector3 avgDir = Vector3.zero;
        int count = 0;

        foreach (var ally in AllyAgentGroupManager.allAllies)
        {
            if (ally == null || ally == agent) continue;



            float dist = Vector3.Distance(agent.transform.position, ally.transform.position);
            if (dist < neighborRadius)
            {
                Vector3 heading = ally.transform.up;   // <-- THIS WORKS EVERY TIME
                avgDir += heading;
                count++;
            }
        }

        if (count > 0)
        {
            avgDir /= count;
            avgDir.Normalize();

            Vector3 desired = avgDir * SteeringAgent.GetMaxSpeedAllowed(agent);

            steeringVelocity = (desired - agent.CurrentVelocity) * alignmentForce;

            return steeringVelocity;
        }

        return Vector3.zero;
    }
    public static Vector3 ComputeHeading(SteeringAgent agent, float radius = 3f)
    {
        Vector3 heading = Vector3.zero;
        int count = 0;

        foreach (var ally in AllyAgentGroupManager.allAllies)
        {
            if (ally == null || ally == agent) continue;

            float dist = Vector3.Distance(agent.transform.position, ally.transform.position);
            if (dist < radius)
            {
                heading += ally.transform.up;
                count++;
            }
        }

        if (count == 0) return Vector3.zero;

        return heading.normalized;
    }

}
