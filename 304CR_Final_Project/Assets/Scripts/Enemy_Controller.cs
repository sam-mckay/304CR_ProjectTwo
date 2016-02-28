using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy_Controller : MonoBehaviour
{
    //TEMP DEBUG
    public Transform pathNode;
    public Transform wallNode;
    //A* vars
    GameObject player;
    GameObject world;
    SqaureGrid grid;
    LinkedList<Location> route;
    LinkedListNode<Location> routePos;
    Vector3 previousPos;
    float distance = 0;
    bool isDone;
    int width, height;

    float spottingTimer;
    //AI Attributes
    int ID;
    float health;
    int status; //0=normal, 1=alert, 2=combat
    int task; //0=patrolling, 1=guarding, 2=chasing, 3=attacking, 4=fleeing
    static float weaponRange;
    public float speed;
	// Use this for initialization
	void Start ()
    {
        status = 0;
        task = 1;
        player = GameObject.FindGameObjectWithTag("Player");
        world = GameObject.FindGameObjectWithTag("World");
        grid = world.gameObject.GetComponent<World>().grid;
        width = world.GetComponent<World>().width;
        height = world.GetComponent<World>().height;
        drawBorder();
        isDone = false;

      
    }
	
	// Update is called once per frame
	void Update ()
    {
       
	}

    void FixedUpdate()
    {
        if (task == 2 && !isDone)
        {
            distance += speed * Time.deltaTime;
            Move();
        }
        else if(task == 2 && isInLineOfSight())
        {
            chase();
        }
        else if(task == 2)
        {
            task = 1;
            status = 0;
        }
    }

    void Move()
    {
        Vector3 targetPos = new Vector3(routePos.Value.x, transform.position.y, routePos.Value.y);
        Vector3 velocity = Vector3.zero;

        transform.position = Vector3.Lerp(previousPos, targetPos, distance);
        transform.LookAt(targetPos);
        if (distance >= 1)
        {
            if(routePos == route.Last)
            {
                isDone = true;
            }
            routePos = routePos.Next;
            distance = 0;
            previousPos = targetPos;
        }
    }

    void guard()
    {

    }

    void chase()
    {
        Vector3 playerPos = player.transform.position;
        playerPos.x = Mathf.FloorToInt(playerPos.x);
        playerPos.z = Mathf.FloorToInt(playerPos.z);

        Location start = new Location((int)this.transform.position.x, (int)this.transform.position.z);
        Location destination = new Location((int)playerPos.x, (int)playerPos.z);
        AStar pathfinder = new AStar(grid, start, destination);
        route = pathfinder.createRoute(grid, pathfinder, start, destination);
        routePos = route.First;

        task = 2;
        distance = 1.2f;
        isDone = false;

        drawGrid(grid, pathfinder, route);
    }

    public void takeDamage(int damage)
    {
        health -= damage;
        if(health<0)
        {
            Destroy(this);
        }
    }

    void playerSpotted()
    {
        Debug.Log("PLAYER_SPOTTED");
        if (status != 1)
        {
            status = 1;//alert
            chase();
        }
    }

    bool isInLineOfSight()
    {
        Ray LOS = new Ray();
        LOS.origin = this.transform.position + (this.transform.forward/2.0f);
        LOS.direction = player.transform.position - this.transform.position;
        float playerDistance = Vector3.Distance(this.transform.position, player.transform.position);
        RaycastHit hit;
        if(Physics.Raycast(LOS, out hit, playerDistance))
        {
            Debug.Log("HIT! "+hit.collider.name);
            Debug.DrawLine(LOS.origin, hit.point, Color.red,1.0f,false);
            if(hit.collider.tag != "Player")
            {
                Debug.Log("HIT OTHER OBJECT");
                return false;
            }
            else
            {
                return true;
            }
        }
        return false;
    }

    void resetTimer()
    {
        spottingTimer = 0.0f;
        Debug.Log("TIMER RESET");
    }

    void OnTriggerEnter(Collider other)
    {
        if(other == player.GetComponent<Collider>())
        {
            resetTimer();
        }   
    }

    void OnTriggerStay(Collider other)
    {
        //update timer as long as player is in view
        if(other == player.GetComponent<Collider>() && isInLineOfSight())
        {
            Debug.Log("IN LOS");
            
            spottingTimer += Time.deltaTime;
            if (spottingTimer >= 2.0f)
            {
                playerSpotted();
            }
        }
        else if(other == player.GetComponent<Collider>())
        {
            resetTimer();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other == player.GetComponent<Collider>())
        {
            resetTimer();
        }
    }





    //TEMP DEBUG
    void drawBorder()
    {
        Transform wallPart;
        
        for (int i = 0; i < width+2; i++)
        {
            //bottom
            wallPart = (Transform)Instantiate(wallNode, new Vector3(-1 + i, 0, -1), Quaternion.identity);
            wallPart.transform.parent = this.transform.parent;

            wallPart = (Transform)Instantiate(wallNode, new Vector3(-1 + i, 0, width), Quaternion.identity);
            wallPart.transform.parent = this.transform.parent;

            wallPart = (Transform)Instantiate(wallNode, new Vector3(-1, 0, -1 + i), Quaternion.identity);
            wallPart.transform.parent = this.transform.parent;

            wallPart = (Transform)Instantiate(wallNode, new Vector3(height + 1, 0, -1 + i), Quaternion.identity);
            wallPart.transform.parent = this.transform.parent;

        }
    }

    void drawGrid(SqaureGrid grid, AStar astar, LinkedList<Location> route)
    {
        for (var y = 0; y <= width; y++)
        {
            for (var x = 0; x <= height; x++)
            {
                Location currentLocation = new Location(x, y);
                Location locationPtr = currentLocation;
                if (!astar.cameFrom.TryGetValue(currentLocation, out locationPtr))
                {
                    locationPtr = currentLocation;
                }
                //show path
                if (route.Contains(currentLocation))
                {
                    Transform wallPart = (Transform)Instantiate(pathNode, new Vector3(x, 0, y), Quaternion.identity);
                    wallPart.parent = this.transform.parent;
                }

                else if (grid.walls.Contains(currentLocation))
                {
                    //show wall at current pos
                    Transform wallPart = (Transform)Instantiate(wallNode, new Vector3(x, 0, y), Quaternion.identity);
                    wallPart.parent = this.transform.parent;
                }
            }
        }
    }
}
