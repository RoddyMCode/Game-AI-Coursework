using System.Data;
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

  

    protected override void InitialiseFromAwake()
	{

      
		 gameObject.AddComponent<SeekToMouse>();
        //  followPosition = leader.position − leader.forward* followDistance;
        CreateNewLeader();

    }
    private void Update()
    {
        if(allyRole == AllyRole.baseTroop)
		{

		}
		else if (allyRole == AllyRole.leader)
		{

		}
	}

    protected override void CooperativeArbitration()
	{
		base.CooperativeArbitration();

       



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
			var seekToMouse = GetComponent<SeekToMouse>();
			seekToMouse.enabled = !seekToMouse.enabled;
		}
	}

	protected override void UpdateDirection()
	{
		if (GetComponent<SeekToMouse>().enabled)
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
