using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour
{
    public int width, height;
    public SqaureGrid grid;
    public List<Vector2> walls;
    public List<Vector2> forests;
    public List<Vector2> roads;
    void Start()
    {
        grid = new SqaureGrid(width, height);
    }

    void assembleWalls(SqaureGrid grid)
    {
        foreach (Vector2 wall in walls)
        {
            grid.walls.Add(new Location((int)wall.x, (int)wall.y));
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
}
