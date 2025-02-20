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
        // Instantiate tiles
        addBaseRoom(20,20);
        // Change certain tiles into door
        for(int k = gridWidth; k <= (gridWidth+3); k++)
        {
            Tile tileD = GetTileAtLoc(gridWidth, k-12);
            tileD.toggleWall(false);
            tileD.toggleDoor(true);
        }
        for(int l = gridHeight; l <= (gridHeight+3); l++)
        {
            Tile tileD = GetTileAtLoc(l-12, gridHeight);
            tileD.toggleWall(false);
            tileD.toggleDoor(true);
        }
    }

    // Function to get a tile at specific coordinates
    public Tile GetTileAtLoc(int x, int z)
    {
        GameObject tile = TileGrid_coord[(x, z)];
        Tile tileComponent = tile.GetComponent<Tile>();
        return tileComponent;
    }

    /// <summary>
    /// Highlights all walkable tiles within range of player's speed centered at player
    /// </summary>
    public void HighlightReachableTiles(int x_cen, int y_cen, int range)
    {
        List<Tile> reachableTiles = FindTilesInRange(GetTileAtLoc(x_cen, y_cen), range);
        foreach (Tile reachableTile in reachableTiles)
        {
            if (reachableTile.IsWalkable)
            {
                reachableTile.ToggleHighlight(true);
                highlightedTiles.Add(reachableTile.gameObject);
            }
        }
    }
    
    public void UnhighlightAllTiles()
    {
        foreach (GameObject tile in highlightedTiles)
        {
            Tile tileComponent = tile.GetComponent<Tile>();
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
                // Only VALID tiles (or a non-valid goal tile)
                if (!(next == goal) && (tile == null || !tile.TryGetComponent(out Tile tileComponent) || !tileComponent.IsWalkable))
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
            GameObject tileObj = TileGrid_coord[current.Value];
            // Remove added tiles that aren't walkable (player goal tile)
            if (tileObj.GetComponent<Tile>().IsWalkable)
            {
                path.Add(tileObj);
            }
            current = cameFrom[current.Value];
        }
        path.Reverse();
        return path;
    }

    // Function creates a new Room, input is the coordinates of the top corner of the new room
    public void newRoom(int x, int z)
    {   
        // Adds basic layout every room will have
        addBaseRoom(x,z);
        // Randomly chooses between several different room options
        int kRoom = Random.Range(1,5);
        switch(kRoom)
        {
            case 1:
                twoRoom(x,z);
                break;
            case 2:
                twoRoomv2(x,z);
                break;
            case 3:
                twoRoomv3(x,z);
                break;
            case 4:
                twoRoomv4(x,z);
                break;
            default:
                break;
        }
    }

    // Creates an enemy in the center of the room
    public void spawnEnemy(int x, int z)
    {
        GetTileAtLoc((x - gridWidth/2),(z - gridHeight/2)).toggleEnemy(true);
        GameManager.Instance.GenerateRandomEnemy(x - gridWidth/2,z - gridHeight/2);
    }

    // List of rooms being created
    // the first part of the name of the function is the number of doors each room has

    public void zeroRoom(int x, int z)
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
                    GetTileAtLoc(i,j).toggleWall(true);
                }
            }
        }
    }
    public void oneRoom(int x, int z)
    {
        for (int i = (x-(gridWidth/2) + 2); i > (x-gridWidth/2 -2); i--)
        {
            GetTileAtLoc(i,z).toggleWall(false);
            GetTileAtLoc(i,z).toggleDoor(true);
        }
    }
    public void oneRoomv2(int x, int z)
    {
        for (int i = (z-(gridHeight/2) + 2); i > (z-gridHeight/2 - 2); i--)
        {
            GetTileAtLoc(x,i).toggleWall(false);
            GetTileAtLoc(x,i).toggleDoor(true);
        }
    }
    public void twoRoom(int x, int z)
    {

        for (int i = (z-(gridHeight/2) + 2); i > (z-gridHeight/2 - 2); i--)
        {
            GetTileAtLoc(x-20,i).toggleWall(false);
            GetTileAtLoc(x-20,i).toggleDoor(true);
        }
        for (int i = (x-(gridWidth/2) + 2); i > (x-gridWidth/2 -2); i--)
        {
            GetTileAtLoc(i,z-20).toggleWall(false);
            GetTileAtLoc(i,z-20).toggleDoor(true);
        }
        spawnEnemy(x,z);
    }
    public void twoRoomv2(int x, int z)
    {

        for (int i = (z-(gridHeight/2) + 2); i > (z-gridHeight/2 - 2); i--)
        {
            GetTileAtLoc(x,i).toggleWall(false);
            GetTileAtLoc(x,i).toggleDoor(true);
        }
        for (int i = (x-(gridWidth/2) + 2); i > (x-gridWidth/2 -2); i--)
        {
            GetTileAtLoc(i,z).toggleWall(false);
            GetTileAtLoc(i,z).toggleDoor(true);
        }
        spawnEnemy(x,z);
    }
    public void twoRoomv3(int x, int z)
    {

        for (int i = (z-(gridHeight/2) + 2); i > (z-gridHeight/2 - 2); i--)
        {
            GetTileAtLoc(x-20,i).toggleWall(false);
            GetTileAtLoc(x-20,i).toggleDoor(true);
        }
        for (int i = (x-(gridWidth/2) + 2); i > (x-gridWidth/2 -2); i--)
        {
            GetTileAtLoc(i,z).toggleWall(false);
            GetTileAtLoc(i,z).toggleDoor(true);
        }
        spawnEnemy(x,z);
    }
    public void twoRoomv4(int x, int z)
    {

        for (int i = (z-(gridHeight/2) + 2); i > (z-gridHeight/2 - 2); i--)
        {
            GetTileAtLoc(x,i).toggleWall(false);
            GetTileAtLoc(x,i).toggleDoor(true);
        }
        for (int i = (x-(gridWidth/2) + 2); i > (x-gridWidth/2 -2); i--)
        {
            GetTileAtLoc(i,z-gridHeight).toggleWall(false);
            GetTileAtLoc(i,z-gridHeight).toggleDoor(true);
        }
        spawnEnemy(x,z);
    }

    // makes the gridWidth x gridHeight number of tiles and changes any edge tiles to wall tiles
    public void addBaseRoom(int x, int z)
    {
        for (int i = x; i >= (x-gridWidth); i--)
        {
            for (int j = z; j >= (z-gridHeight); j--)
            {
                if (!checkExists(i,j))
                {
                    GameObject tile = Instantiate(tilePrefab, new Vector3(i, 0, j), Quaternion.identity, transform);
                    TileGrid_tile.Add(tile, (i, j));
                    TileGrid_coord.Add((i, j), tile);
                }
            }
        }
        // Changes certain tiles into wall tiles
        for (int i = x; i >= (x-gridWidth); i--)
        {
            for (int j = z; j >= (z-gridHeight); j--)
            {
                if(checkEdge(i,j) != 0)
                {
                    GetTileAtLoc(i,j).toggleWall(true);
                }
            }
        }
    }

    // checks if a tile already exists at set coordinates
    public bool checkExists(int x, int z)
    {
        (int, int) tile = (x,z);
        return TileGrid_coord.ContainsKey(tile);
    }

    // Function that checks if this is an edge tile, input is coordinates of the tile
    // Output is number dependent on which direction this is an edge tile for.
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
        XE = (0,-1);
        ZE = (-1,0);
        (int, int) neighbor3 = (tile.Item1 + XE.Item1, tile.Item2 + XE.Item2);
        if (!(TileGrid_coord.ContainsKey(neighbor3)))
        {
            return 4;
        }
        (int, int) neighbor4 = (tile.Item1 + ZE.Item1, tile.Item2 + ZE.Item2);
        if (!(TileGrid_coord.ContainsKey(neighbor4)))
        {
            return 3;
        }
        return 0;
    }

    /// <summary>
    /// Given a tile, it will return all tiles within a range
    /// </summary>
    public List<Tile> FindTilesInRange(Tile centerTile, int range)
    {
        List<Tile> tilesInRange = new();

        if (range == 0)
        {
            return tilesInRange;
        }

        (int, int) startPos = (
            Mathf.RoundToInt(centerTile.gameObject.transform.position.x),
            Mathf.RoundToInt(centerTile.gameObject.transform.position.z)
        );

        Queue<(int, int)> frontier = new();
        Dictionary<(int, int), int> distance = new();
        frontier.Enqueue(startPos);
        distance[startPos] = 0;

        // Breadth-First Search
        while (frontier.Count > 0)
        {
            // Dequeue a tile
            (int, int) currentPos = frontier.Dequeue();
            int currentDist = distance[currentPos];

            Tile currTile = GetTileAtLoc(currentPos.Item1, currentPos.Item2);
            tilesInRange.Add(currTile);

            // Don't enqueue neighbors beyond range
            if (currentDist >= range)
            {
                continue;
            }

            // Check neighbors
            foreach (var neighborPos in GetNeighbors(currentPos))
            {
                // Already visited?
                if (distance.ContainsKey(neighborPos))
                {
                    continue;
                }

                // Mark distance and enqueue
                distance[neighborPos] = currentDist + 1;
                frontier.Enqueue(neighborPos);
            }
        }

        return tilesInRange;
    }
}
