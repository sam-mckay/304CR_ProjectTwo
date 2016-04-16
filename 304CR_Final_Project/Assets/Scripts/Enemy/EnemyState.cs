using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyState
{
    public Enemy_Controller enemy;
    public GameObject player;

    public int nextPatrolPoint;
    public SqaureGrid grid;
    public AStar pathfinder;
    public LinkedList<Location> route;
    public LinkedListNode<Location> routePos;
    public Vector3 previousPos;
    public float distance = 0;
    public float speed = 1;
    public float spottingTimer;

    public EnemyState(Enemy_Controller enemyController)
    {
        enemy = enemyController;
        grid = GameObject.FindGameObjectWithTag(Tags.World).GetComponent<World>().grid;
        player = GameObject.FindGameObjectWithTag(Tags.Player);
    }

    public virtual void updateState() { }
    
    public virtual void toPatrolState()
    {
        Debug.Log("Cannot Transition to this state");
    }

    public virtual void toGuardState()
    {
        Debug.Log("Cannot Transition to this state");
    }

    public virtual void toSearchState()
    {
        Debug.Log("Cannot Transition to this state");
    }

    public virtual void toAttackState()
    {
        enemy.currentState = enemy.attackState;
    }

    public virtual void toChaseState()
    {
        Debug.Log("Cannot Transition to this state");
    }

    public virtual void toFleeState()
    {
        Debug.Log("Cannot Transition to this state");
    }

    

    public virtual void move()
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
                nextPatrolPoint++;
                if (nextPatrolPoint > enemy.patrolPoints.Length)
                {
                    nextPatrolPoint = 0;
                }
                if (enemy.patrolPoints.Length > 0)
                {
                    patrol();
                }
                else
                {
                    enemy.guardState.moveToGuardPoint();
                    enemy.currentState = enemy.guardState;
                }
            }
            else
            {
                routePos = routePos.Next;
                distance = 0;
                previousPos = targetPos;
            }
        }
    }

    public virtual void attacked()
    {
        enemy.transform.rotation = Quaternion.LookRotation(player.transform.position - enemy.transform.position);
        toAttackState();
    }

    public bool isInLineOfSight()
    {
        Ray LOS = new Ray();
        LOS.origin = enemy.transform.position + (enemy.transform.forward / 2.0f);
        LOS.direction = player.transform.position - enemy.transform.position;
        float playerDistance = Vector3.Distance(enemy.transform.position, player.transform.position);
        RaycastHit hit;
        if (Physics.Raycast(LOS, out hit, playerDistance))
        {
            //Debug.Log("HIT! " + hit.collider.name);
            Debug.DrawLine(LOS.origin, hit.point, Color.red, 1.0f, false);
            if (hit.collider.tag != "Player")
            {
               // Debug.Log("HIT OTHER OBJECT");
                return false;
            }
            else
            {
                return true;
            }
        }
        return false;
    }

    public void resetTimer()
    {
        spottingTimer = 0.0f;
        //Debug.Log("TIMER RESET");
    }

    public void patrol()
    {
        //set locations up
        int startX, startY, endX, endY;
        startX = Mathf.FloorToInt(enemy.transform.position.x);
        startY = Mathf.FloorToInt(enemy.transform.position.z);

        endX = Mathf.FloorToInt(enemy.patrolPoints[nextPatrolPoint].position.x);
        endY = Mathf.FloorToInt(enemy.patrolPoints[nextPatrolPoint].position.z);
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
        previousPos = enemy.transform.position;
        distance = 0;
        //Debug.Log("ROUTE CREATED: " + route.Count);
    }

    void playerSpotted()
    {
        toChaseState();
    }

    public virtual void playerheard()
    {
        toChaseState();
    }

    //triggers
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other == player.GetComponent<Collider>())
        {
            resetTimer();
        }
    }

    public virtual void OnTriggerStay(Collider other)
    {
        //update timer as long as player is in view
        if (other == player.GetComponent<Collider>() && isInLineOfSight())
        {
            //Debug.Log("IN LOS");

            spottingTimer += Time.deltaTime;
            if (spottingTimer >= 2.0f)
            {
                playerSpotted();
            }
        }
        else if (other == player.GetComponent<Collider>())
        {
            resetTimer();
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (other == player.GetComponent<Collider>())
        {
            resetTimer();
        }
    }
}
