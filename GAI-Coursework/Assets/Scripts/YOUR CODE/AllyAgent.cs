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
    public WanderBehaviour wanderScript;
    public AvoidanceBehaviour avoidScript;

  

    protected override void InitialiseFromAwake()
    {
        allyRole = AllyRole.baseTroop;

        seekScript = GetComponent<SeekBehaviour>();
        if (wanderScript == null)
        {
            wanderScript = gameObject.AddComponent<WanderBehaviour>();
        }
        if (avoidScript == null)
        {
            avoidScript = gameObject.AddComponent<AvoidanceBehaviour>();
        }
        if (seekScript == null)
        {
            seekScript = gameObject.AddComponent<SeekBehaviour>();
        }

        seekScript.enabled = false;
        wanderScript.enabled = false;
        avoidScript.enabled = true;
                

        CreateNewLeader();

    }

    protected override void CooperativeArbitration()
    {
        base.CooperativeArbitration();


        CreateNewLeader();
        UpdateAllBaseTroopsTarget();


        if (allyRole == AllyRole.baseTroop)
        {
            // Follow the leader
            seekScript.Target = AllyAgentGroupManager.leader.transform;
            seekScript.enabled = true;

            // Only use avoidance if necessary
            avoidScript.enabled = true;

            // Disable wander for baseTroops
            wanderScript.enabled = false;
        }
        else if (allyRole == AllyRole.leader)
        {
            // Leader moves around randomly or along path
            wanderScript.enabled = true;
            seekScript.enabled = false;
            avoidScript.enabled = false;
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
            if (attackType == Attack.AttackType.Rocket && GameData.Instance.AllyRocketsAvailable <= 0)
            {
                attackType = Attack.AttackType.AllyGun;
            }

            AttackWith(attackType);
        }
        if (Input.GetMouseButtonDown(1))
        {
            SteeringVelocity = Vector3.zero;
            CurrentVelocity = Vector3.zero;

              
        }
    }

    protected override void UpdateDirection()
    {
       // var seekToMouse = GetComponent<SeekToMouse>();

            base.UpdateDirection();
       
    }
    void CreateNewLeader()
    {
        // If there is no leader or the leader is inactive
        if (AllyAgentGroupManager.leader == null || !AllyAgentGroupManager.leader.gameObject.activeInHierarchy)
        {
            if (allyRole != AllyRole.leader)
            {
                AllyAgentGroupManager.leader = this;
                allyRole = AllyRole.leader;


                // Enable leader behaviors
                gameObject.AddComponent<SeekToMouse>();
                wanderScript.enabled = true;
                seekScript.enabled = false;
                avoidScript.enabled = false;
            }
        }
        else
        {
            // Only set to baseTroop if you are not the leader
            if (allyRole != AllyRole.leader)
            {
                allyRole = AllyRole.baseTroop;
                wanderScript.enabled = false;
                seekScript.enabled = true;
                avoidScript.enabled = true;
               
              
            }
        }
    }
    void UpdateAllBaseTroopsTarget()
    {
        foreach (var ally in AllyAgentGroupManager.allAllies)
        {
            if (ally != null && ally != this && ally.allyRole == AllyRole.baseTroop)
            {
                ally.seekScript.Target = this.transform;
            }
        }
    }

}
