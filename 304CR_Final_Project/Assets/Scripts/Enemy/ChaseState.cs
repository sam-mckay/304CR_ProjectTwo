using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChaseState : EnemyState
{
    bool isDone;

    public ChaseState(Enemy_Controller enemyController) : base(enemyController)
    {
        enemy = enemyController;
        grid = GameObject.FindGameObjectWithTag(Tags.World).GetComponent<World>().grid;
        player = GameObject.FindGameObjectWithTag(Tags.Player);
        previousPos = enemy.transform.position;
        isDone = true;
    }

    public override void updateState()
    {
        if (isDone)
        {
            chase();
        }
        //Debug.Log("PLAYER DIST: "+Vector3.Distance(player.transform.position, enemy.transform.position));
        if (Vector3.Distance(player.transform.position, enemy.transform.position) < 10 && isInLineOfSight())
        {
            toAttackState();
        }
        distance += speed * Time.deltaTime;
        move();
    }
    
    public override void toPatrolState()
    {
        enemy.patrolState.patrol();
        enemy.currentState = enemy.patrolState;
    }

    public override void toChaseState()
    {
        enemy.currentState = enemy.chaseState;
        isDone = true;
    }

    public override void toAttackState()
    {
        enemy.currentState = enemy.attackState;
    }

    public override void move()
    {
        //Debug.Log("ROUTE POS X" + routePos.Value.x);
        Vector3 targetPos = new Vector3(routePos.Value.x, enemy.transform.position.y, routePos.Value.y);
        Vector3 velocity = Vector3.zero;

        enemy.transform.position = Vector3.Lerp(previousPos, targetPos, distance);
        enemy.transform.LookAt(targetPos);
        if (distance >= 1)
        {
            if (routePos == route.Last)
            {
                //check if player visible
                if(isInLineOfSight())
                {
                    toChaseState();
                }
                toPatrolState();
            }
            routePos = routePos.Next;
            distance = 0;
            previousPos = targetPos;
        }
    }

    void chase()
    {
        Vector3 playerPos = player.transform.position;
        playerPos.x = Mathf.FloorToInt(playerPos.x);
        playerPos.z = Mathf.FloorToInt(playerPos.z);
        previousPos.x = Mathf.FloorToInt(enemy.transform.position.x);
        previousPos.z = Mathf.FloorToInt(enemy.transform.position.z);

        Location start = new Location((int)enemy.transform.position.x, (int)enemy.transform.position.z);
        Location destination = new Location((int)playerPos.x, (int)playerPos.z);
        if (!enemy.isValidDestination(destination))
        {
            return;
        }
        pathfinder = new AStar(grid, start, destination);
        route = pathfinder.createRoute(grid, pathfinder, start, destination);
        //route = pathfinder.optimiseRoute(grid, pathfinder, route);
        routePos = route.First.Next;

        distance = 0;
        isDone = false;
    }
}
