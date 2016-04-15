using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy_Controller : MonoBehaviour
{
    //TEMP DEBUG
    
    public Transform pathNode;
    public Transform wallNode;
    public GameObject bullet;
    //A* vars
    GameObject player;
    GameObject world;
    SqaureGrid grid;
    AStar pathfinder;
    LinkedList<Location> route;
    LinkedListNode<Location> routePos;
    Vector3 previousPos;
    float distance = 0;
    bool isDone;
    int width, height;

    bool isPatrolForward=true;

    float spottingTimer;
    //AI Attributes
    int ID;
    public float health;
    int status; //0=normal, 1=alert, 2=combat
    public int task; //0=patrolling, 1=guarding, 2=chasing, 3=attacking, 4=fleeing
    public float weaponRange;
    public float damage;
    public float speed;

    //
    public Transform[] patrolPoints;
    public Vector2 VECpatrolStart, VECpatrolEnd;
    
    public Location patrolStart, patrolEnd;

    //AI State Machine
    [HideInInspector] public EnemyState currentState;
    [HideInInspector] public ChaseState chaseState;
    [HideInInspector] public AttackState attackState;
    [HideInInspector] public FleeState fleeState;
    [HideInInspector] public PatrolState patrolState;
    [HideInInspector] public GuardState guardState;
    [HideInInspector] public SearchState searchState;
    // Use this for initialization
    void Start ()
    {
        status = 0;
        
        player = GameObject.FindGameObjectWithTag(Tags.Player);
        world = GameObject.FindGameObjectWithTag(Tags.World);
        grid = world.gameObject.GetComponent<World>().grid;
        width = world.GetComponent<World>().width;
        height = world.GetComponent<World>().height;
        
        isDone = false;
        weaponRange = 8;
        damage = 10;

        //convert vector to location
        patrolStart = new Location((int)VECpatrolStart.x, (int)VECpatrolStart.y);
        patrolEnd = new Location((int)VECpatrolEnd.x, (int)VECpatrolEnd.y);

        /*
        if (task == 0)
        {
            patrol(patrolStart, patrolEnd);
        }*/

        //set states
        currentState = new EnemyState(this);
        chaseState = new ChaseState(this);
        attackState = new AttackState(this);
        fleeState = new FleeState(this);
        patrolState = new PatrolState(this);
        guardState = new GuardState(this);
        searchState = new SearchState(this);

        currentState = patrolState;
    }
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    void FixedUpdate()
    {
        //shiny new state machine here:
        currentState.updateState();
    }

    public void fireBullet(Ray ray, RaycastHit hit)
    {
        Debug.Log("ENEMY: FIRED BULLET");
        Vector3 fixedAxisDir = ray.direction;
        fixedAxisDir.y = 0;
        GameObject newBullet = (GameObject)Instantiate(bullet, ray.origin, Quaternion.LookRotation(fixedAxisDir));
        newBullet.GetComponent<Bullet>().initBullet(weaponRange, damage, 0.1f);
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

    void patrol(Location start, Location end)
    {
        task = 0;
        if (isPatrolForward)
        {
            pathfinder = new AStar(grid, start, end);
            route = pathfinder.createRoute(grid, pathfinder, start, end);
            isPatrolForward = false;
        }
        else if (!isPatrolForward)
        {
            pathfinder = new AStar(grid, end, start);
            route = pathfinder.createRoute(grid, pathfinder, end, start);
            isPatrolForward = true;
        }
        //route = pathfinder.optimiseRoute(grid, pathfinder, route);
        routePos = route.First;
        distance = 1.2f;
        //drawGrid(grid, pathfinder, route);
        
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
        if (!isValidDestination(destination))
        {
            status = 0;
            return;
        }
        pathfinder = new AStar(grid, start, destination);
        route = pathfinder.createRoute(grid, pathfinder, start, destination);
        //route = pathfinder.optimiseRoute(grid, pathfinder, route);
        routePos = route.First;

        task = 2;
        distance = 1.2f;
        isDone = false;

        //drawGrid(grid, pathfinder, route);
    }

    public bool isValidDestination(Location destination)
    {
        if (destination.x < 0 || destination.y < 0 || destination.x > width || destination.y > height)
        {
            return false;
        }
        else if(grid.walls.Contains(destination))
        {
            return false;
        }
        return true;
    }

    public void takeDamage(float hitDamage)
    {
        currentState.attacked();
        health -= hitDamage;
        Debug.Log("ENEMY: TOOK DAMAGE: " + hitDamage + " CURRENT HEALTH: " + health);
        if (health < 0.0f)
        {
            Destroy(this.gameObject);
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

    public void playerheard()
    {
        currentState.playerheard();
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
            //Debug.Log("HIT! "+hit.collider.name);
            Debug.DrawLine(LOS.origin, hit.point, Color.red,1.0f,false);
            if(hit.collider.tag != "Player")
            {
                //Debug.Log("HIT OTHER OBJECT");
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
        //Debug.Log("TIMER RESET");
    }

    void OnTriggerEnter(Collider other)
    {
        currentState.OnTriggerEnter(other);  
    }

    void OnTriggerStay(Collider other)
    {
        currentState.OnTriggerStay(other);
    }

    void OnTriggerExit(Collider other)
    {
        currentState.OnTriggerExit(other);
    }
    
    //TEMP DEBUG
    

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
                    //Transform wallPart = (Transform)Instantiate(wallNode, new Vector3(x, 0, y), Quaternion.identity);
                    //wallPart.parent = this.transform.parent;
                }
            }
        }
    }
}
