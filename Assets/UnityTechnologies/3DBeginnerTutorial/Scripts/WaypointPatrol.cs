using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointPatrol : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public Transform[] waypoints;
    public float minWaitTime = 1f; // Minimum wait time before moving to the next waypoint
    public float maxWaitTime = 3f; // Maximum wait time before moving to the next waypoint
    public float wanderRadius = 10f; // Radius in which the ghost can randomly wander
    
    private int m_CurrentWaypointIndex;
    private bool isPatrolling = true;

    void Start ()
    {
        // Start the patrol process with a random waypoint.
        SetNextDestination();
    }

    void Update ()
    {
        if (isPatrolling)
        {
            // If the agent has reached the destination, wait for a random time before choosing a new target.
            if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
            {
                StartCoroutine(WaitAndPatrol());
            }
        }
    }

    void SetNextDestination()
    {
        // Randomly select the next waypoint or wander randomly
        if (Random.Range(0f, 1f) > 0.5f) // 50% chance to either move to a waypoint or wander randomly
        {
            // Randomly choose a waypoint
            m_CurrentWaypointIndex = Random.Range(0, waypoints.Length);
            navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
        }
        else
        {
            // Wander to a random point within a certain radius
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;

            // Ensure the point is on the NavMesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
            {
                navMeshAgent.SetDestination(hit.position);
            }
        }
    }

    IEnumerator WaitAndPatrol()
    {
        isPatrolling = false;

        // Wait for a random amount of time between minWaitTime and maxWaitTime
        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitTime);

        // After waiting, set a new destination (random waypoint or random wander)
        SetNextDestination();

        isPatrolling = true;
    }
}