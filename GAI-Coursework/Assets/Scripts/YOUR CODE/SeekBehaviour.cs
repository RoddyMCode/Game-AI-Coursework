using UnityEngine;

public class SeekBehaviour : SteeringBehaviour
{
    public Transform Target;

    public override Vector3 UpdateBehaviour(SteeringAgent agent)
    {
        if (Target == null)
        {
            steeringVelocity = Vector3.zero;
            return steeringVelocity;
        }


        // Compute desired velocity
        Vector3 direction = (Target.position - transform.position);
        direction.z = 0f;

        float maxSpeed = SteeringAgent.GetMaxSpeedAllowed(agent);
        desiredVelocity = direction.normalized * maxSpeed;

        // Compute steering velocity
        steeringVelocity = desiredVelocity - agent.CurrentVelocity;

        return steeringVelocity;
    }
}
