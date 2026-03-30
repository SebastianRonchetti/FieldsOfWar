using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

/* 
Remove references to deprecated Scripts 
    - GameManager
*/

public class NavigationAgent : MonoBehaviour
{
    public NavMeshAgent agent;
    Unit myUnit;
    public Transform[] waypoints;
    UIManager ins;
    [SerializeField] int currentWaypointIndex = 0;
    [SerializeField]private int lastcommand = -1, command, requiredUnits = 20;
    [SerializeField] float stoppingDistance;
    /* private static bool timerRunning = false;
    private static float timerCountdown = 0f, controlDuration = 30f, proximityThreshold = 1.5f; */
    Rigidbody rb;

    void Awake()
    {
        TryGetComponent(out rb);
    }
    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
        myUnit = GetComponent<Unit>();
        agent.stoppingDistance = stoppingDistance;
        agent.speed = myUnit.speed;
      
    }

    public void setWaypoints(List<Transform> _waypoints)
    {
        waypoints = _waypoints.ToArray();
    }

    void Update()
    {
        if (!agent.isOnNavMesh) return;

        command = myUnit.command;
        //if (myUnit.stoppedAtGarrison == true) return;
        if (command != lastcommand)
        {
            HandleCommand(command);
            lastcommand = command;
        }

        if(myUnit.getTarget() != null)
        {
            if (command == 3 && myUnit.getTarget().TryGetComponent<Unit>(out _) && agent.destination != myUnit.getTarget().position)
            {
                pursueTarget();
            }    
        } 

        CheckIfReachedDestination();
        //Whats this for? V - Sb
        /* if (timerRunning)
        {
            timerCountdown -= Time.deltaTime;
            UIManager.Instance.UpdateTimerDisplay(timerCountdown);

            if (timerCountdown <= 0f)
            {
                timerRunning = false;
                // win / lose screen
            }
        } */

    }

    public void setTarget(Transform enemyPosition)
    {
        if (!enemyPosition) return;
        agent.SetDestination(enemyPosition.position);
    }

    public void HandleCommand(int command)
    {
        switch (command) //0 = retreat, 1 = defend, 2 = attack
        {
            case 0: // Retreat 
                MoveToPreviousWaypoint();
                break;
            case 1: // Defend - hold position
                StopMoving();
                break;
            case 2: // attack
                MoveToNextWaypoint();
                break;
            case 3: //go after enemy
                pursueTarget();
                break;
        }
    }

    void pursueTarget()
    {
        agent.SetDestination(myUnit.getTarget().position);
    }

    private void MoveToNextWaypoint()
    {
        if (waypoints.Length == 0) return;

        if (currentWaypointIndex < waypoints.Length - 1)
        {
            currentWaypointIndex++;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
        else
        {
            StopMoving();
        }
    }

    private void MoveToPreviousWaypoint()
    {
        if (waypoints.Length == 0) return;

        rb.isKinematic = false;
        currentWaypointIndex = 0;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }


    private void StopMoving()
    {
        rb.isKinematic = true;
        agent.ResetPath();
    }

    private void CheckIfReachedDestination()
    {
        if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance)
        {
            if (myUnit.command == 2)
            {
                MoveToNextWaypoint();
            }
            else if(myUnit.command == 3)
            {
                pursueTarget();
            }
        }
    }
    

    //These look like herd navigation AI patterns but I'm unsure of how and when to use them V - Sb
    /* private void MonitorGroupAtWaypointZero()
    {
        if (waypoints.Length == 0 || timerRunning) return;

        NavigationAgent[] allAgents = FindObjectsOfType<NavigationAgent>();
        int count = 0;

        foreach (var unit in allAgents)
        {
            if (unit.CompareTag("Player"))
            {
                float dist = Vector3.Distance(unit.transform.position, waypoints[0].position);
                if (dist < proximityThreshold)
                {
                    count++;
                }
            }
        }

        if (count == requiredUnits)
        {
            timerRunning = true;
            timerCountdown = controlDuration;
        }
    }

    private void RunGroupTimer()
    {
        if (timerRunning)
        {
            timerCountdown -= Time.deltaTime;
            if (timerCountdown <= 0f)
            {
                timerRunning = false;
                //lose/win screen 
            }
        }
    } */

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, agent.stoppingDistance);
    }
}