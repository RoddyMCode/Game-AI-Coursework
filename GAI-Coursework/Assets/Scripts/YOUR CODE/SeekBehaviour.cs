using UnityEngine;

public class SeekBehaviour : SteeringBehaviour
{
    public Transform Target;

   
    private bool hasTargetPositionOverride = false;
    private Vector3 targetPositionOverride;

    public Vector3 TargetPositionOverride
    {
        get => targetPositionOverride;
        set
        {
            targetPositionOverride = value;
            hasTargetPositionOverride = true;
        }
    }

    public void ClearTargetPositionOverride()
    {
        hasTargetPositionOverride = false;
    }
 
    public override Vector3 UpdateBehaviour(SteeringAgent agent)
    {
        Vector3 targetPos;

        // priortises a enemy target over random position or leader
        if (hasTargetPositionOverride)
        {
            targetPos = TargetPositionOverride;
        }
        else
        {
            if (Target == null)     // targert is leader or random position dependin troop type
            {
                steeringVelocity = Vector3.zero;
                return steeringVelocity;
            }

            targetPos = Target.position;
        }

        
        Vector3 direction = (targetPos - transform.position);
        direction.z = 0f;

        float maxSpeed = SteeringAgent.GetMaxSpeedAllowed(agent);
        desiredVelocity = direction.normalized * maxSpeed;

        
        steeringVelocity = desiredVelocity - agent.CurrentVelocity;

        // this avoid the agents slowing when traveling on diagonals
        if (Mathf.Abs(direction.x) > 0.1f && Mathf.Abs(direction.y) > 0.1f)
        {
            desiredVelocity *= 1.4f;
        }

        return steeringVelocity;
    }
}
