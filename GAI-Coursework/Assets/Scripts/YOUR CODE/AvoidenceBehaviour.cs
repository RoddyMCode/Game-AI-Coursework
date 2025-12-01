using UnityEngine;

public class AvoidanceBehaviour : SteeringBehaviour
{
    public float avoidRadius = 1.5f;
    public float avoidForce = 10f;

    public override Vector3 UpdateBehaviour(SteeringAgent agent)
    {
        Vector3 force = Vector3.zero;

        // Avoid other SteeringAgents
        foreach (var other in FindObjectsOfType<SteeringAgent>())
        {
            if (other == agent) continue;

            Vector3 toOther = agent.transform.position - other.transform.position;
            float dist = toOther.magnitude;

            if (dist < avoidRadius)
            {
                force += toOther.normalized * (avoidForce / dist);
            }
        }

        desiredVelocity = force;
        steeringVelocity = desiredVelocity;
        return steeringVelocity;
    }
}
