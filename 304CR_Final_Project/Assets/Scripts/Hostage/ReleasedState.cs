using UnityEngine;
using System.Collections;

public class ReleasedState : HostageState
{
    Vector3 escapeLocation;

    public ReleasedState(HostageController hostageController)  : base(hostageController)
    {
        hostage = hostageController;
        escapeLocation = GameObject.FindGameObjectWithTag(Tags.Player).transform.position;
    }

    public override void updateState()
    {
        //Debug.Log("Released");
        distance += hostage.speed * Time.deltaTime;
        move();
    }

    public override void toCapturedState()
    {
        hostage.currentState = hostage.capturedState;
    }

    public void escape()
    {
        int startX, startY, endX, endY;
        startX = Mathf.FloorToInt(hostage.transform.position.x);
        startY = Mathf.FloorToInt(hostage.transform.position.z);

        endX = Mathf.FloorToInt(escapeLocation.x);
        endY = Mathf.FloorToInt(escapeLocation.z);
        Location start = new Location(startX, startY);
        Location end = new Location(endX, endY);
        /*
        Debug.Log("START X,Y: " + startX + "," + startY);
        Debug.Log("END X,Y: " + endX + "," + endY);
        Debug.Log("nextPatrolPoint POS: " + enemy.patrolPoints[nextPatrolPoint].position.x + "," + enemy.patrolPoints[nextPatrolPoint].position.z);
        Debug.Log("nextPatrolPoint Var: " + nextPatrolPoint);
        */
        //set route
        pathfinder = new AStar(grid, start, end);
        route = pathfinder.createRoute(grid, pathfinder, start, end);
        routePos = route.First.Next;
        previousPos = hostage.transform.position;
        distance = 0;
        //Debug.Log("ROUTE CREATED: " + route.Count);
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.tag == Tags.Enemy && other.GetType().ToString() != "UnityEngine.SphereCollider")
        {
            toCapturedState();
        }
    }
}
