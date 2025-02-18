using System.Collections.Generic;
using UnityEngine;

public class TileGridManager : MonoBehaviour
{
    public static TileGridManager Instance;
    public GameObject tilePrefab;
    
    // Bi-Directional searching for tile by object or coordinate
    public Dictionary<GameObject, (int, int)> TileGrid_tile = new();
    public Dictionary<(int, int), GameObject> TileGrid_coord = new();

    public int gridHeight, gridWidth;
    
    private List<GameObject> highlightedTiles = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    void Start()
    {
        // Instantiate tiles with center as origin
        for (int i = -gridWidth/2; i <= gridWidth/2; i++)
        {
            for (int j = -gridHeight/2; j <= gridHeight/2; j++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(i, 0, j), Quaternion.identity, transform);
                TileGrid_tile.Add(tile, (i, j));
                TileGrid_coord.Add((i, j), tile);
            }
        }
        // Change certain tiles into wall
        for (int i = -gridWidth/2; i <= gridWidth/2; i++)
        {
            for (int j = -gridHeight/2; j <= gridHeight/2; j++)
            {
                if(checkEdge(i,j) != 0)
                {
                    getTile(i,j).toggleWall(true);
                }
            }
        }
        // Change certain tiles into door
        for(int k = gridWidth/2; k <= (gridWidth+6)/2; k++)
        {
            Tile tileD = getTile(gridWidth/2, k-12);
            tileD.toggleWall(false);
            tileD.toggleDoor(true);
        }
        for(int l = gridHeight/2; l <= (gridHeight+6)/2; l++)
        {
            Tile tileD = getTile(l-12, gridHeight/2);
            tileD.toggleWall(false);
            tileD.toggleDoor(true);
        }
    }

    // Function to get a tile at specific coordinates
    public Tile getTile(int x, int y)
    {
        TileGrid_coord.TryGetValue((x,y), out GameObject tile);
        tile.TryGetComponent(out Tile tileComponenet);
        return tileComponenet;
    }

    public void HighlightReachableTiles(int x_cen, int y_cen, int range)
    {
        if (range == 0)
        {
            return;
        }

        for (int i = x_cen - range; i <= x_cen + range; i++)
        {
            for (int j = y_cen - range; j <= y_cen + range; j++)
            {
                TileGrid_coord.TryGetValue((i, j), out GameObject tile);
                if (tile != null && tile.TryGetComponent(out Tile tileComponent) && tileComponent.IsWalkable)
                {
                    tileComponent.ToggleHighlight(true);
                    highlightedTiles.Add(tile);
                }
            }
        }
    }
    
    public void UnhighlightAllTiles()
    {
        foreach (GameObject tile in highlightedTiles)
        {
            tile.TryGetComponent(out Tile tileComponent);
            tileComponent.ToggleHighlight(false);
        }
        highlightedTiles.Clear();
    }

    public List<GameObject> FindRoute(GameObject startTile, GameObject endTile)
    {
        (int, int) startCoord = TileGrid_tile[startTile];
        (int, int) endCoord = TileGrid_tile[endTile];

        List<GameObject> path = FindRoute(startCoord, endCoord);
        return path;
    }

    public List<GameObject> FindRoute((int, int) startCoord, (int, int) endCoord)
    {
        List<GameObject> path = AStarSearch(startCoord, endCoord);
        return path;
    }

    private List<GameObject> AStarSearch((int, int) start, (int, int) goal)
    {
        PriorityQueue<(int, int)> frontier = new();
        frontier.Enqueue(start, 0);

        Dictionary<(int, int), (int, int)?> cameFrom = new();
        Dictionary<(int, int), float> costSoFar = new();

        cameFrom[start] = null;
        costSoFar[start] = 0;

        while (frontier.Count > 0)
        {
            (int, int) current = frontier.Dequeue();

            if (current == goal)
            {
                break;
            }

            foreach ((int, int) next in GetNeighbors(current))
            {
                TileGrid_coord.TryGetValue(next, out GameObject tile);
                if (tile == null || !tile.TryGetComponent(out Tile tileComponent) || !tileComponent.IsWalkable)
                {
                    continue;
                }
                
                float newCost = costSoFar[current] + 1; // Uniform movement cost (for now)
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    float heuristicCost = Mathf.Abs(next.Item1 - goal.Item1) + Mathf.Abs(next.Item2 - goal.Item2);
                    float priority = newCost + heuristicCost;
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        return ReconstructPath(cameFrom, start, goal);
    }

    private List<(int, int)> GetNeighbors((int, int) tile)
    {
        List<(int, int)> neighbors = new();
        (int, int)[] directions = { (1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (-1, -1), (1, -1), (-1, 1) };

        foreach (var dir in directions)
        {
            (int, int) neighbor = (tile.Item1 + dir.Item1, tile.Item2 + dir.Item2);
            if (TileGrid_coord.ContainsKey(neighbor))
            {
                neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }

    private List<GameObject> ReconstructPath(Dictionary<(int, int), (int, int)?> cameFrom, (int, int) start, (int, int) goal)
    {
        List<GameObject> path = new();
        (int, int)? current = goal;

        while (current != null && current != start)
        {
            path.Add(TileGrid_coord[current.Value]);
            current = cameFrom.GetValueOrDefault(current.Value, null);
        }
        path.Reverse();
        return path;
    }

    // Function creates a new Room, input is the coordinates of the top corner of the new room
    public void newRoom(int x, int z)
    {   
        for (int i = x; i > (x-gridWidth); i--)
        {
            for (int j = z; j > (z-gridHeight); j--)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(i, 0, j), Quaternion.identity, transform);
                TileGrid_tile.Add(tile, (i, j));
                TileGrid_coord.Add((i, j), tile);
            }
        }
        // Changes certain tiles into wall tiles
        for (int i = x; i > (x-gridWidth); i--)
        {
            for (int j = z; j > (z-gridHeight); j--)
            {
                if(checkEdge(i,j) != 0)
                {
                    getTile(i,j).toggleWall(true);
                }
            }
        }
        // For demo purposes creating a tile for enemy spawn
        getTile((x - gridWidth/2),(z - gridHeight/2)).toggleEnemy(true);
        GameManager.Instance.GenerateRandomEnemy(x - gridWidth/2,z - gridHeight/2);
    }

    // Function that checks if this is an edge tile, input is coordinates of the tile
    // Output is number dependent on which direction this is an edge tile for.
    // TODO make this work in the negative directions as well
    public int checkEdge(int x, int z)
    {
        (int, int) tile = (x,z);
        var XE = (0,1);
        var ZE = (1,0);
        (int, int) neighbor = (tile.Item1 + XE.Item1, tile.Item2 + XE.Item2);
        if (!(TileGrid_coord.ContainsKey(neighbor)))
        {
            return 2;
        }
        (int, int) neighbor2 = (tile.Item1 + ZE.Item1, tile.Item2 + ZE.Item2);
        if (!(TileGrid_coord.ContainsKey(neighbor2)))
        {
            return 1;
        }

        // For the negative directions
        // Editing note: maybe return 3 and 4 and make seperate cases?
        XE = (0,-1);
        ZE = (-1,0);
        (int, int) neighbor3 = (tile.Item1 + XE.Item1, tile.Item2 + XE.Item2);
        if (!(TileGrid_coord.ContainsKey(neighbor3)))
        {
            return 2;
        }
        (int, int) neighbor4 = (tile.Item1 + ZE.Item1, tile.Item2 + ZE.Item2);
        if (!(TileGrid_coord.ContainsKey(neighbor4)))
        {
            return 1;
        }
        return 0;
    }
}
