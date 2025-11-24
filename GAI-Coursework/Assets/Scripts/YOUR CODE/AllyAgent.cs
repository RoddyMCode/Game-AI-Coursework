using System.Data;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
public enum AllyRole
{
    leader,
    baseTroop,
    
}
public class AllyAgent : SteeringAgent
{
	private Attack.AttackType attackType = Attack.AttackType.AllyGun;

	public AllyRole allyRole;


    private SeekBehaviour seekScript;
    //public WanderBehaviour wanderScript;
    //  public AvoidanceBehaviour avoidScript;

    private SeekToMouse seekToMouse;

    protected override void InitialiseFromAwake()
	{
		allyRole = AllyRole.baseTroop;

        seekScript = GetComponent<SeekBehaviour>();
		if (seekScript == null)
        {
            seekScript = gameObject.AddComponent<SeekBehaviour>();
        }

        seekScript.enabled = false;

		gameObject.AddComponent<SeekToMouse>();

       
        if (seekToMouse == null)
        {
		seekToMouse = gameObject.AddComponent<SeekToMouse>();

        seekToMouse.enabled = false; 
		}
		//  followPosition = leader.position − leader.forward* followDistance;
    CreateNewLeader();

    }

    protected override void CooperativeArbitration()
	{
		base.CooperativeArbitration();


        SteeringVelocity = Vector3.zero;

        if (allyRole == AllyRole.baseTroop)
        {
            if (seekScript != null)
            {
                seekScript.Target = AllyAgentGroupManager.leader.transform;
                SteeringVelocity += seekScript.GetSteering();
            }
        }
    


        if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			attackType = Attack.AttackType.Melee;
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			attackType = Attack.AttackType.AllyGun;
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			attackType = Attack.AttackType.Rocket;
		}
		if (Input.GetKey(KeyCode.Space))
		{
			if(attackType == Attack.AttackType.Rocket && GameData.Instance.AllyRocketsAvailable <= 0)
			{
				attackType = Attack.AttackType.AllyGun;
			}

			AttackWith(attackType);
		}
		if(Input.GetMouseButtonDown(1))
		{
			SteeringVelocity = Vector3.zero;
			CurrentVelocity = Vector3.zero;

			if (seekToMouse != null)
			{
				seekToMouse.enabled = !seekToMouse.enabled;
			}
		}
	}

	protected override void UpdateDirection()
	{
        var seekToMouse = GetComponent<SeekToMouse>();

        if (GetComponent<SeekToMouse>().enabled && seekToMouse != null)
		{
			base.UpdateDirection();
		}
		else
		{
			var mouseInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mouseInWorld.z = 0.0f;
			transform.up = Vector3.Normalize(mouseInWorld - transform.position);
		}
	}
	void CreateNewLeader() 
	{
		
		if (AllyAgentGroupManager.leader == null) 
		{
			AllyAgentGroupManager.leader = this;
            allyRole = AllyRole.leader;		// made a static manager to handle who the leader also made tags so i can debug better
			}
			else
			{
            allyRole = AllyRole.baseTroop;
			}

		}

}
