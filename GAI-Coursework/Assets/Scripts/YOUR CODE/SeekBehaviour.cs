using UnityEngine;

public class SeekBehaviour : MonoBehaviour
{
    public Transform Target;
    public float speed = 5f;

    public Vector3 GetSteering()
    {
        if (Target == null)
            return Vector3.zero;

        Vector3 dir = (Target.position - transform.position).normalized;
        return dir * speed;
    }
}