using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatrolState : IEnemyState
{
    private Enemy_Controller enemy;

    private int nextPatrolPoint;
    SqaureGrid grid;
    AStar pathfinder;
    LinkedList<Location> route;
    LinkedListNode<Location> routePos;
    Vector3 previousPos;
    float distance = 0;
    float speed = 1;

    public PatrolState(Enemy_Controller enemyController)
    {
        grid = GameObject.FindGameObjectWithTag(Tags.World).GetComponent<World>().grid;
        enemy = enemyController;
        patrol();
    }

    public void updateState()
    {
        distance += speed * Time.deltaTime;
        move();
    }

    public void onTriggerEnter(Collider other)
    {

    }

    public void toPatrolState()
    {
        Debug.Log("Cannot Transition to this state");
    }

    public void toGuardState()
    {
        
    }

    public void toSearchState()
    {
        Debug.Log("Cannot Transition to this state");
    }

    public void toAttackState()
    {
        Debug.Log("Cannot Transition to this state");
    }

    public void toChaseState()
    {
        Debug.Log("Cannot Transition to this state");
    }

    public void toFleeState()
    {
        Debug.Log("Cannot Transition to this state");
    }

    void move()
    {
        Vector3 targetPos = new Vector3(routePos.Value.x, enemy.transform.position.y, routePos.Value.y);
        Vector3 velocity = Vector3.zero;

        enemy.transform.position = Vector3.Lerp(previousPos, targetPos, distance);
        enemy.transform.LookAt(targetPos);
        if (distance <= 1)
        {
            if (routePos == route.Last)
            {
                nextPatrolPoint++;
                if(nextPatrolPoint > enemy.patrolPoints.Length)
                {
                    nextPatrolPoint = 0;
                }
                patrol();
            }
            routePos = routePos.Next;
            distance = 0;
            previousPos = targetPos;
        }
    }

    void patrol()
    {
        //set locations up
        int startX, startY, endX, endY;
        startX = Mathf.FloorToInt(enemy.transform.position.x);
        startY = Mathf.FloorToInt(enemy.transform.position.z);

        endX = Mathf.FloorToInt(enemy.patrolPoints[nextPatrolPoint].position.x);
        endY = Mathf.FloorToInt(enemy.patrolPoints[nextPatrolPoint].position.z);
        Location start = new Location(startX, startY);
        Location end = new Location(endX, endY);
        //set route
        pathfinder = new AStar(grid, start, end);
        route = pathfinder.createRoute(grid, pathfinder, end, start);
        routePos = route.First;
        distance = 0.8f;
    }
}
