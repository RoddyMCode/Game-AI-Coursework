using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public enum AllyRole            // leaders will seek to random location using a* whilst base troops just seek to leader
{
    leader,
    baseTroop,
}
    public enum AgentState      // tells troops to change state
    {
        Normal,
        Combat
    }
public class AllyAgent : SteeringAgent
{
    private Attack.AttackType basicShoot = Attack.AttackType.AllyGun;

    public AllyRole allyRole;

    public float attackRange = 7f;
    public SteeringAgent currentTarget;


    public float detectionRange = 10f;

    private SeekToMouse seekToMouse;
    private SeekBehaviour seekScript;       // used to steer to targets

    private AvoidanceBehaviour avoidanceScript;         // used to avoid overlapping allys
    public SeparationBehaviour separationScript;        // used to seperate while traversing the map
  

    private AllyPathfinding pathfinder;             // A* pathfinding used to find fasted path for the leader
    private List<int> currentPath;
    private int currentWaypoint = 0;
    private int attemptsAtRandomNode = 20; //number of tries a leader gets before dying to find a node with A*



    public AgentState currentState = AgentState.Normal;
        
    protected override void InitialiseFromAwake()
    {
        allyRole = AllyRole.baseTroop;

       
        pathfinder = gameObject.AddComponent<AllyPathfinding>();
        pathfinder.Initialise(GameData.Instance.Map);

        if (seekScript == null)
        {
            seekScript = gameObject.AddComponent<SeekBehaviour>();
        }

        if (separationScript == null)
        {
            separationScript = gameObject.AddComponent<SeparationBehaviour>();
        }
        if (avoidanceScript == null)
        {
            avoidanceScript = gameObject.AddComponent<AvoidanceBehaviour>();
        }


        CreateNewLeader();
    }

        
    protected override void CooperativeArbitration()
    {
        base.CooperativeArbitration();

        CreateNewLeader();
        UpdateAllBaseTroopsTarget();
        UpdateCombatStateMachine();

        switch (currentState)
        {
            case AgentState.Combat:
                CombatState();
                return;

            case AgentState.Normal:
            default:
                NormalState();
                break;
        }
              
    }

    void UpdateCombatStateMachine()
    {
      
        if (currentTarget != null)
        {
            float distanceFromEnemy = Vector3.Distance(transform.position, currentTarget.transform.position);

            if (!currentTarget.gameObject.activeInHierarchy ||
                GameData.Instance.GetAgentHealth(currentTarget) <= 0 ||
                distanceFromEnemy > detectionRange)   
            {
                currentTarget = null;
            }
        }


        if (currentTarget == null)
        {
            currentTarget = FindEnemyInDetectionRadius();

        }

        if (currentTarget != null)
        {
            currentState = AgentState.Combat;
        }
        else
        {
            currentState = AgentState.Normal;
        }

    }
    void CombatState()
    {
        if (currentTarget == null)
        {
            currentState = AgentState.Normal;
            return;
        }

        StopPathing();
        seekScript.ClearTargetPositionOverride();

        // aim at enemy first enemy in range
        seekScript.enabled = true;
        seekScript.Target = currentTarget.transform;

        // stops the allys from seperating in combat relys on avoidance to avoid rockets
        separationScript.enabled = false;
     

        AttackWith(basicShoot);
    }

   
    void NormalState()
    {
        if (allyRole == AllyRole.baseTroop)
        {
           
            seekScript.enabled = true;
            seekScript.Target = AllyAgentGroupManager.leader.transform;

            separationScript.enabled = true;
            avoidanceScript.enabled = true;

            return;

        }
               
        

        FollowPath();

        if (currentPath == null || currentWaypoint >= currentPath.Count)
        {
            PickNewRandomDestination();
        }
    }

   
    SteeringAgent FindEnemyInDetectionRadius()
    {
        foreach (var enemy in GameData.Instance.enemies)
        {
            if (enemy == null) continue;
            if (!enemy.gameObject.activeInHierarchy) continue;
            if (GameData.Instance.GetAgentHealth(enemy) <= 0) continue;

            float distanceAway = Vector3.Distance(transform.position, enemy.transform.position);

            if (distanceAway <= detectionRange)
                return enemy;  // first enemy found in detection range
        }

        return null;
    }
    void StopPathing()
    {
        currentPath = null;
        currentWaypoint = 0;
    }

    void FollowPath()
    {
        if (currentPath == null || currentWaypoint >= currentPath.Count)
            return;

        Vector3 waypointLocation = PathNodeVector(currentPath[currentWaypoint]);
        seekScript.TargetPositionOverride = waypointLocation;

        float distanceFromWaypoint = Vector3.Distance(transform.position, waypointLocation);

        if (distanceFromWaypoint < 0.75f)
        {
           
            currentWaypoint++;
        }
    }


    void PickNewRandomDestination()
    {
        Map map = GameData.Instance.Map;
        bool foundValidPath = false;
        int start = GetStartingVector(transform.position);      // this ally agent position



        for (int i = 0; i < attemptsAtRandomNode; i++)
        {
            int rx = Random.Range(0, Map.MapWidth);    // random Y and X indexes
            int ry = Random.Range(0, Map.MapHeight);

            if (!map.IsNavigatable(rx, ry))     // moves to nest attempt if not navigable
                continue;
                int target = map.MapIndex(rx, ry);

                currentPath = pathfinder.FindPath(start, target);
                currentWaypoint = 0;


                if (currentPath != null && currentPath.Count > 0)
                {
                    foundValidPath = true;
                    break;

                }
                
            
        }
        if (!foundValidPath)
        {
            this.gameObject.SetActive(false);
            Debug.Log(this.gameObject.name + "Died Couldn't pathe");
            //Anti pathfinding infinite loop check

        }
      

    }

    Vector3 PathNodeVector(int index)
    {
        int x = GameData.Instance.Map.MapIndexToX(index);
        int y = GameData.Instance.Map.MapIndexToY(index);
        return new Vector3(x, y, 0f);
    }

    int GetStartingVector(Vector3 pos)
    {
        int x = Mathf.RoundToInt(pos.x);
        int y = Mathf.RoundToInt(pos.y);
        return GameData.Instance.Map.MapIndex(x, y);
    }

    void CreateNewLeader()
    {
        if (AllyAgentGroupManager.leader == null ||
            !AllyAgentGroupManager.leader.gameObject.activeInHierarchy)
        {
            AllyAgentGroupManager.leader = this;        
            allyRole = AllyRole.leader;
            seekScript.enabled = true;
            separationScript.enabled = false;
            avoidanceScript.enabled = false;
             

        }
    }

    void UpdateAllBaseTroopsTarget()
    {

        Transform leaderTransform = AllyAgentGroupManager.leader.transform;

        foreach (var ally in AllyAgentGroupManager.allAllies)
        {
            if (ally.allyRole == AllyRole.baseTroop)
            {
                ally.seekScript.Target = leaderTransform; 
            }
        }
    }
  
    }
