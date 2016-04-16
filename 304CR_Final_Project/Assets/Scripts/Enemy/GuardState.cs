using UnityEngine;
using System.Collections;

public class GuardState : EnemyState
{
    
    public GuardState(Enemy_Controller enemyController) : base(enemyController)
    {
        enemy = enemyController;
    }

    public override void updateState()
    {
        Debug.Log("GUARDING");
        if(distance < 1)
        {
            //Debug.Log("MOVING");
            distance += speed * Time.deltaTime;
            move();
        }
        else
        {
            enemy.transform.rotation = enemy.guardRotation;
        }
    }

    public void moveToGuardPoint()
    {
        int startX, startY, endX, endY;
        startX = Mathf.FloorToInt(enemy.transform.position.x);
        startY = Mathf.FloorToInt(enemy.transform.position.z);

        endX = Mathf.FloorToInt(enemy.guardPosition.x);
        endY = Mathf.FloorToInt(enemy.guardPosition.z);

        Location start = new Location(startX, startY);
        Location end = new Location(endX, endY);
        //
        Debug.Log("START X,Y: " + startX + "," + startY);
        Debug.Log("END X,Y: " + endX + "," + endY);
        //set route
        pathfinder = new AStar(grid, start, end);
        route = pathfinder.createRoute(grid, pathfinder, start, end);
        routePos = route.First.Next;
        previousPos = enemy.transform.position;
        distance = 0;
    }

    public override void toChaseState()
    {
        enemy.currentState = enemy.chaseState;
    }
}
