using UnityEngine;

public class WanderBehaviour : SteeringBehaviour
{
    public float circleDistance = 1.5f;
    public float circleRadius = 1.0f;
    public float jitter = 0.5f;

    private Vector3 wanderTarget = Vector3.zero;

    public override Vector3 UpdateBehaviour(SteeringAgent agent)
    {
        float maxSpeed = SteeringAgent.GetMaxSpeedAllowed(agent);

        // Add jitter to wander target
        wanderTarget += new Vector3(
            Random.Range(-1f, 1f) * jitter,
            Random.Range(-1f, 1f) * jitter,
            0f);

        wanderTarget = wanderTarget.normalized * circleRadius;

        Vector3 circleCenter = agent.CurrentVelocity.normalized * circleDistance;
        desiredVelocity = circleCenter + wanderTarget;

        // Scale to speed
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        steeringVelocity = desiredVelocity - agent.CurrentVelocity;
        return steeringVelocity;
    }
}
