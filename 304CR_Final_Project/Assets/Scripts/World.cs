using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour
{
    public int width, height;
    public Transform wallNode;
    public SqaureGrid grid;
    public List<Vector2> walls;
    public List<Vector2> forests;
    public List<Vector2> roads;


    void Start()
    {
        drawBorder();
        grid = new SqaureGrid(width, height);
        assembleForests(grid);
        assembleWalls(grid);
        assembleRoads(grid);
    }

    void assembleWalls(SqaureGrid grid)
    {
        foreach (Vector2 wall in walls)
        {
            grid.walls.Add(new Location((int)wall.x, (int)wall.y));
        }
        GameObject[] worldWalls = GameObject.FindGameObjectsWithTag(Tags.Wall);
        foreach(GameObject wall in worldWalls)
        {
            int newX, newZ;
            newX = Mathf.FloorToInt(wall.transform.position.x);
            newZ = Mathf.FloorToInt(wall.transform.position.z);
            Location newWallPos = new Location(newX, newZ);
            grid.walls.Add(newWallPos);
            if(wall.name == "Cover")
            {
                Debug.Log("COVER POS: " + wall.transform.position);
            }
        }
    }

    void assembleForests(SqaureGrid grid)
    {
        foreach (Vector2 forest in forests)
        {
            Location currentLocation = new Location((int)forest.x, (int)forest.y);
            if (!grid.walls.Contains(currentLocation))
            {
                grid.forests.Add(new Location((int)forest.x, (int)forest.y));
            }
        }
    }

    void assembleRoads(SqaureGrid grid)
    {
        foreach (Vector2 road in roads)
        {
            Location currentLocation = new Location((int)road.x, (int)road.y);
            if (!grid.walls.Contains(currentLocation))
            {
                grid.roads.Add(new Location((int)road.x, (int)road.y));
            }
        }
    }

    void drawBorder()
    {
        Transform wallPart;

        for (int i = 0; i < width + 2; i++)
        {
            //bottom
            wallPart = (Transform)Instantiate(wallNode, new Vector3(-1 + i, 0.43f, -1), Quaternion.identity);
            wallPart.transform.parent = this.transform;
            wallPart.tag = "Untagged";

            wallPart = (Transform)Instantiate(wallNode, new Vector3(-1 + i, 0.43f, width), Quaternion.identity);
            wallPart.transform.parent = this.transform;
            wallPart.tag = "Untagged";

            wallPart = (Transform)Instantiate(wallNode, new Vector3(-1, 0.43f, -1 + i), Quaternion.identity);
            wallPart.transform.parent = this.transform;
            wallPart.tag = "Untagged";

            wallPart = (Transform)Instantiate(wallNode, new Vector3(height + 1, 0.43f, -1 + i), Quaternion.identity);
            wallPart.transform.parent = this.transform;
            wallPart.tag = "Untagged";

        }
    }



    public void healthTriggerReset(GameObject healthTrigger)
    {
        healthTrigger.SetActive(false);
        StartCoroutine(healthTriggerRespawn(healthTrigger));
    }

    IEnumerator healthTriggerRespawn(GameObject healthTrigger)
    {
        yield return new WaitForSeconds(5);
        healthTrigger.SetActive(true);
    }
}
