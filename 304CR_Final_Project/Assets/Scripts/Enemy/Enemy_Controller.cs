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
    public SqaureGrid grid;
    bool isDone;
    int width, height;

    bool isPatrolForward=true;
    public GameObject bullet;
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
    [HideInInspector] public Vector3 guardPosition;
    [HideInInspector] public Quaternion guardRotation;

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
        grid = GameObject.FindGameObjectWithTag(Tags.World).GetComponent<World>().grid;
        width = world.GetComponent<World>().width;
        height = world.GetComponent<World>().height;
        
        isDone = false;
        weaponRange = 8;
        damage = 10;
        
        

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

        

        if (patrolPoints.Length == 0)
        {
            guardPosition = new Vector3(Mathf.FloorToInt(transform.position.x),0, Mathf.FloorToInt(transform.position.z));
            guardRotation = this.transform.rotation;
            currentState = guardState;
        }
        else
        {
            currentState = patrolState;
        }
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
        //Debug.Log("ENEMY: FIRED BULLET");
        Vector3 fixedAxisDir = ray.direction;
        fixedAxisDir.y = 0;
        GameObject newBullet = (GameObject)Instantiate(bullet, ray.origin, Quaternion.LookRotation(fixedAxisDir));
        newBullet.GetComponent<Bullet>().initBullet(weaponRange, damage, 0.1f);
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

    public bool isValidDestination(Location destination)
    {
        if (destination.x < 0 || destination.y < 0 || destination.x > width || destination.y > height)
        {
            return false;
        }
        else if (grid.walls.Contains(destination))
        {
            return false;
        }
        return true;
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
