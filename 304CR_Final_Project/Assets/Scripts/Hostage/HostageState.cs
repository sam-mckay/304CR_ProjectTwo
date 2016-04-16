using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HostageState 
{

    public HostageController hostage;
    public GameObject player;

    public int nextPatrolPoint;
    public SqaureGrid grid;
    public AStar pathfinder;
    public LinkedList<Location> route;
    public LinkedListNode<Location> routePos;
    public Vector3 previousPos;
    public float distance = 0;

    public HostageState(HostageController hostageController)
    {
        hostage = hostageController;
        grid = GameObject.FindGameObjectWithTag(Tags.World).GetComponent<World>().grid;
        player = GameObject.FindGameObjectWithTag(Tags.Player);
    }

    public virtual void updateState() { }
    

    public virtual void toCapturedState()
    {
        Debug.Log("Cannot Transition to this state");
    }

    public virtual void toReleasedState()
    {
        Debug.Log("Cannot Transition to this state");
    }


    public virtual void move()
    {
        //Debug.Log("ROUTE POS X" + routePos.Value.x);
        Vector3 targetPos = new Vector3(routePos.Value.x, hostage.transform.position.y, routePos.Value.y);
        Vector3 velocity = Vector3.zero;

        hostage.transform.position = Vector3.Lerp(previousPos, targetPos, distance);
        hostage.transform.LookAt(targetPos);
        if (distance >= 1)
        {
            if (routePos == route.Last)
            {
                Debug.Log("GAME WON");
            }
            else
            {
                routePos = routePos.Next;
                distance = 0;
                previousPos = targetPos;
            }
        }
    }
       

    //triggers
    public virtual void OnTriggerEnter(Collider other)
    {
        //
    }
}
