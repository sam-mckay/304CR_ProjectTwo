using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public interface WeightedGraph<L>
{
    int cost(Location A, Location B);
    IEnumerable<Location> Neighbours(Location currentLocation);
}

public struct Location
{
    public readonly int x;
    public readonly int y;
    public Location(int w, int h)
    {
        this.x = w;
        this.y = h;
    }
}

//To Represent the map
public class SqaureGrid : WeightedGraph<Location>
{
    //used to locate neighbours
    public static readonly Location[] DIRS = new[]
    {
        new Location(1,0),
        new Location(0,-1),
        new Location(-1,0),
        new Location(0,1)
    };

    int width;
    int height;
    int forestCost;
    int roadCost; 
    //for weighting nodes
    public HashSet<Location> walls = new HashSet<Location>();
    public HashSet<Location> forests = new HashSet<Location>();
    public HashSet<Location> roads = new HashSet<Location>();

    //construtor for map grid
    public SqaureGrid(int w, int h)
    {
        //init vars
        width = w;
        height = h;
        forestCost = PlayerPrefs.GetInt(SaveManager.forestCost);
        roadCost = PlayerPrefs.GetInt(SaveManager.roadCost);
    }

    //check whether the Location is on the grid
    // useful for checkin edge cases as map does not support looping
    bool inBounds(Location currentLocation)
    {
        if(0 <= currentLocation.x && currentLocation.y < width &&
            0 <= currentLocation.y && currentLocation.y < height)
        {
            return true;
        }
        return false;
    }

    //check if terrain can be travelled over 
    //could be expanded to support more than just walls
    //i.e. lava, holes 
    bool passable(Location currentLocation)
    {
        if(walls.Contains(currentLocation))
        {
            return false;
        }
        return true;
    }

    //returns the cost of a node dependent on its weight
    public int cost(Location A, Location B)
    {
        if(forests.Contains(B) || forests.Contains(A))
        {
            Debug.Log("RETURNING FOREST:"+ PlayerPrefs.GetInt(SaveManager.forestCost));
            return forestCost;
        }
        if (roads.Contains(B) || roads.Contains(A))
        {
            Debug.Log("RETURNING ROAD:" + PlayerPrefs.GetInt(SaveManager.roadCost));
            return roadCost;
        }
        //standard cost
        return 5;
    }

    //returns each valid neighbour
    public IEnumerable<Location> Neighbours(Location currentLocation)
    {
        foreach (var dir in DIRS)
        {
            Location next = new Location(currentLocation.x + dir.x, currentLocation.y + dir.y);
            //if next is on the map and is a walkable i.e. not a wall
            if (inBounds(next) && passable(next))
            {
                yield return next;
            }
        }
    }
}

public class AStar : MonoBehaviour
{
    public Dictionary<Location, Location> cameFrom = new Dictionary<Location, Location>();
    public Dictionary<Location, int> costSoFar = new Dictionary<Location, int>();

    static public int distCalc(Location A, Location B)
    {
        return Math.Abs(A.x - B.x) + Math.Abs(A.y - B.y);
    }

       
    public AStar(SqaureGrid grid, Location startPos, Location destination)
    {
        //Debug.Log("START A STAR");
        //setup variables
        PriorityQueue<int, Location> frontier = new PriorityQueue<int, Location>();
        frontier.Enqueue(startPos, 0);
        cameFrom[startPos] = startPos;
        costSoFar[startPos] = 0;

        //main loop
        //while frontier is not empty
        //Debug.Log("START A STAR LOOP");
        while (frontier.count > 0)
        {
            //get first Node in queue
            Location current = frontier.Dequeue();
            //check if current Node is the Destination Node
            if (current.Equals(destination))
            {
                Debug.Log("A*: DONE");
                break;//Destination reached stop searching
            }

            foreach (var next in grid.Neighbours(current))
            {
                // calculate cost to neighbour
                int newCost = costSoFar[current] + grid.cost(current, next);
                // if its cheaper to get here from this node than previous routes or 
                //  we haven't checked this neighbour
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    //update costSoFar
                    costSoFar[next] = newCost;
                    //calculate priority
                    int priority = distCalc(next, destination) + newCost;
                    //add to frontier
                    frontier.Enqueue(next, priority);
                    //sent current node as the previous node
                    //allows us to work our way back through cameFrom to find the route 
                    cameFrom[next] = current;
                }
            }
        }
    }

    public LinkedList<Location> createRoute(SqaureGrid grid, AStar astar, Location start, Location destination)
    {
        LinkedList<Location> path = new LinkedList<Location>();
        Location current = destination;
        path.AddFirst(current);
        while (!current.Equals(start))
        {
            current = astar.cameFrom[current];
            path.AddFirst(current);
        }
        return path;
    }

    public LinkedList<Location> optimiseRoute(SqaureGrid grid, AStar astar, LinkedList<Location> path)
    {
        LinkedList<Location> optimisedPath = new LinkedList<Location>();

        LinkedListNode<Location> currentNode = path.First;
        bool isNodeRemoved = true;
        while(currentNode != path.Last)
        {
            LinkedListNode<Location> nextNode = currentNode.Next;
            if (nextNode != null)
            {
                if (!grid.forests.Contains(currentNode.Value) && isInLineOfSight(currentNode.Value, nextNode.Value))
                {
                    path.Remove(nextNode);
                    Debug.Log("REMOVING NODE");
                    isNodeRemoved = true;
                }
                if (!isNodeRemoved)
                {
                    currentNode = currentNode.Next;
                    isNodeRemoved = false;
                }
            }
            else
            {
                break;
            }
        }
        return optimisedPath;
    }

    bool isInLineOfSight(Location currentNode, Location nextNode)
    {
        Ray LOS = new Ray();
        LOS.origin = new Vector3(currentNode.x + (currentNode.x / 2.0f), 0, currentNode.y + (currentNode.y / 2.0f));
        LOS.direction = new Vector3(nextNode.x, 0, nextNode.y) - new Vector3(currentNode.x, 0, currentNode.y);
        float playerDistance = Vector3.Distance(new Vector3(currentNode.x, 0, currentNode.y), new Vector3(nextNode.x, 0, nextNode.y));
        RaycastHit hit;
        if (Physics.Raycast(LOS, out hit, playerDistance))
        {
            Debug.Log("HIT! " + hit.collider.name);
            Debug.DrawLine(LOS.origin, hit.point, Color.red, 1.0f, false);
            if (hit.collider.tag == "Wall")
            {
                Debug.Log("HIT WALL");
                return false;
            }
            else
            {
                return true;
            }
        }
        return true;
    }
}
