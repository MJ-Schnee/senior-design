using System.Collections.Generic;
using UnityEngine;

public class TileGridManager : MonoBehaviour
{
    public static TileGridManager Instance;

    [SerializeField]
    private GameObject tilePrefab;

    [SerializeField]
    private int roomLength = 20;
    
    [SerializeField]
    private int roomWidth = 20;
    
    // Bi-Directional searching for tile by object or coordinate
    public Dictionary<Tile, Vector2Int> TileToCoordinate = new();
    public Dictionary<Vector2Int, Tile> CoordinateToTile = new();
    
    private List<Tile> highlightedTiles = new();

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
        CreateStartingRoom();
    }

    /// <summary>
    /// Creates the starting room
    /// </summary>
    void CreateStartingRoom()
    {
        Vector2Int[] startingDoorSides = { Vector2Int.up, Vector2Int.left, Vector2Int.right };
        CreateRoom(20, 20, false, startingDoorSides);
    }

    /// <summary>
    /// Instantiates a new tile at a location if one doesn't exist there already
    /// </summary>
    private void CreateTile(int x, int z)
    {
        Vector2Int newTileGridLoc = new(x, z);
        if (!CoordinateToTile.ContainsKey(newTileGridLoc))
        {
            GameObject tileGO = Instantiate(tilePrefab, new Vector3(x, 0, z), Quaternion.identity, transform);
            Tile tile = tileGO.GetComponent<Tile>();
            TileToCoordinate.Add(tile, newTileGridLoc);
            CoordinateToTile.Add(newTileGridLoc, tile);
        }
    }

    // Function to get a tile at specific coordinates
    public Tile GetTileAtLocation(int x, int z)
    {
        if (CoordinateToTile.TryGetValue(new Vector2Int(x, z), out Tile tile))
        {
            return tile;
        }
        return null;
    }

    /// <summary>
    /// Highlights all walkable tiles within range of player's speed centered at player
    /// </summary>
    public void HighlightReachableTiles(int x_cen, int y_cen, int range)
    {
        List<Tile> reachableTiles = FindTilesInRange(GetTileAtLocation(x_cen, y_cen), range);
        foreach (Tile reachableTile in reachableTiles)
        {
            if (reachableTile.IsWalkable)
            {
                reachableTile.ToggleHighlight(true);
                highlightedTiles.Add(reachableTile);
            }
        }
    }
    
    /// <summary>
    /// Unhighlights all tiles that were marked as highlighted
    /// </summary>
    public void UnhighlightAllTiles()
    {
        foreach (Tile tile in highlightedTiles)
        {
            tile.ToggleHighlight(false);
        }
        highlightedTiles.Clear();
    }

    /// <summary>
    /// Preform A* search given start tile coordinates and end tile coordinates
    /// </summary>
    public List<Tile> AStarSearch(Vector2Int start, Vector2Int goal)
    {
        PriorityQueue<Vector2Int> frontier = new();
        frontier.Enqueue(start, 0);

        Dictionary<Vector2Int, Vector2Int?> cameFrom = new();
        Dictionary<Vector2Int, float> costSoFar = new();

        cameFrom[start] = null;
        costSoFar[start] = 0;

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();

            if (current == goal)
            {
                break;
            }

            foreach (Vector2Int next in GetNeighborsPos(current))
            {
                CoordinateToTile.TryGetValue(next, out Tile tile);
                // Only VALID tiles (or a non-valid goal tile)
                if (!(next == goal) && (tile == null || !tile.TryGetComponent(out Tile tileComponent) || !tileComponent.IsWalkable))
                {
                    continue;
                }
                
                float newCost = costSoFar[current] + 1; // Uniform movement cost (for now)
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    float heuristicCost = Mathf.Abs(next.x - goal.x) + Mathf.Abs(next.y - goal.y);
                    float priority = newCost + heuristicCost;
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        return ReconstructPath(cameFrom, start, goal);
    }

    /// <summary>
    /// Given a tile position, this will return the positions of its neighbor tiles
    /// </summary>
    private List<Vector2Int> GetNeighborsPos(Vector2Int tilePos)
    {
        List<Vector2Int> neighbors = new();
        Vector2Int[] directions = {
            Vector2Int.right, Vector2Int.left,
            Vector2Int.up, Vector2Int.down,
            Vector2Int.one, new(-1, -1),    // Upleft, downleft
            new(1, -1), new(-1, 1)          // Upright, downright
        };

        foreach (var dir in directions)
        {
            Vector2Int neighbor = new(tilePos.x + dir.x, tilePos.y + dir.y);
            if (CoordinateToTile.ContainsKey(neighbor))
            {
                neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }

    /// <summary>
    /// Helper method for A* to reconstruct finished path
    /// </summary>
    private List<Tile> ReconstructPath(Dictionary<Vector2Int, Vector2Int?> cameFrom, Vector2Int start, Vector2Int goal)
    {
        List<Tile> path = new();
        Vector2Int? current = goal;

        while (current != null && current != start)
        {
            Tile tile = CoordinateToTile[current.Value];
            // Remove added tiles that aren't walkable (player goal tile)
            if (tile.IsWalkable)
            {
                path.Add(tile);
            }
            current = cameFrom[current.Value];
        }
        path.Reverse();
        return path;
    }

    /// <summary>
    /// Creates a room given its top right corner, enemy spawn, and sides of doors
    /// </summary>
    public void CreateRoom(int roomTopRightX, int roomTopRightZ, bool doesSpawnEnemy = false, params Vector2Int[] doorSides)
    {
        // Start with blank template
        CreateBlankRoom(roomTopRightX, roomTopRightZ);

        // Create a door on each specified side
        foreach (Vector2Int side in doorSides)
        {
            CreateDoor(roomTopRightX, roomTopRightZ, side);
        }

        // Spawn enemy or room event
        if (doesSpawnEnemy)
        {
            // TODO: Make random tile enemy spawn tile, not just center
            GetTileAtLocation(roomTopRightX - roomWidth / 2, roomTopRightZ - roomLength / 2).toggleEnemy(true);
            GameManager.Instance.GenerateRandomEnemy(roomTopRightX - roomWidth / 2, roomTopRightZ - roomLength / 2);
        }
        else
        {
            // TODO: Implement room events
        }
    }

    /// <summary>
    /// Creates a door on a given side of a room given the top right corner's position
    /// </summary>
    public void CreateDoor(int roomTopRightX, int roomTopRightZ, Vector2Int side)
    {
        int doorSize = 4;
        int halfDoor = doorSize / 2;
        
        // Using the room's dimensions:
        // Top-right: (roomTopRightX, roomTopRightZ)
        // Top-left: (roomTopRightX - RoomWidth, roomTopRightZ)
        // Bottom-right: (roomTopRightX, roomTopRightZ - RoomLength)
        // Bottom-left: (roomTopRightX - RoomWidth, roomTopRightZ - RoomLength)

        if (side == Vector2Int.up) // North wall (up wall)
        {
            int centerX = roomTopRightX - roomWidth / 2;
            int startX = centerX - halfDoor;
            int endX = centerX + halfDoor;
            for (int x = startX; x <= endX; x++)
            {
                Tile doorTile = GetTileAtLocation(x, roomTopRightZ);
                doorTile.toggleWall(false);
                doorTile.toggleDoor(true);
            }
        }
        else if (side == Vector2Int.down) // South wall (down wall)
        {
            int centerX = roomTopRightX - roomWidth / 2;
            int startX = centerX - halfDoor;
            int endX = centerX + halfDoor;
            int doorZ = roomTopRightZ - roomLength;
            for (int x = startX; x <= endX; x++)
            {
                Tile doorTile = GetTileAtLocation(x, doorZ);
                doorTile.toggleWall(false);
                doorTile.toggleDoor(true);
            }
        }
        else if (side == Vector2Int.right) // East wall (right wall)
        {
            int centerZ = roomTopRightZ - roomLength / 2;
            int startZ = centerZ - halfDoor;
            int endZ = centerZ + halfDoor;
            for (int z = startZ; z <= endZ; z++)
            {
                Tile doorTile = GetTileAtLocation(roomTopRightX, z);
                doorTile.toggleWall(false);
                doorTile.toggleDoor(true);
            }
        }
        else if (side == Vector2Int.left) // West wall (left wall)
        {
            int centerZ = roomTopRightZ - roomLength / 2;
            int startZ = centerZ - halfDoor;
            int endZ = centerZ + halfDoor;
            int doorX = roomTopRightX - roomWidth;
            for (int z = startZ; z <= endZ; z++)
            {
                Tile doorTile = GetTileAtLocation(doorX, z);
                doorTile.toggleWall(false);
                doorTile.toggleDoor(true);
            }
        }
    }

    /// <summary>
    /// Creates a random, new room. Parameters are top right corner of new room
    /// </summary>
    public void CreateRandomRoom(int roomTopRightX, int roomTopRightZ)
    {   
        // Create a list of all possible door sides
        List<Vector2Int> possibleSides = new()
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        // Range allows for no doors or all sides as doors
        int doorCount = Random.Range(0, 5);

        // Pick door sides at random from the list
        Vector2Int[] doorSides = new Vector2Int[doorCount];
        for (int i = 0; i < doorCount; i++)
        {
            int randomIndex = Random.Range(0, possibleSides.Count);
            doorSides[i] = possibleSides[randomIndex];

            // Ensure we don't pick the same side twice
            possibleSides.RemoveAt(randomIndex);
        }

        // Percent chance to spawn an enemy
        bool spawnEnemy = Random.Range(0, 100) <= 75;

        CreateRoom(roomTopRightX, roomTopRightZ, spawnEnemy, doorSides);
    }

    /// <summary>
    /// Makes a room of roomLength x roomHeight tiles with edges set to wall tiles.
    /// Input is top right corner of room.
    /// </summary>
    public void CreateBlankRoom(int x, int z)
    {
        for (int i = x; i >= (x - roomWidth); i--)
        {
            for (int j = z; j >= (z - roomLength); j--)
            {
                CreateTile(i, j);
            }
        }
        
        // Changes certain tiles into wall tiles
        for (int i = x; i >= (x - roomWidth); i--)
        {
            for (int j = z; j >= (z - roomLength); j--)
            {
                if(GetEdgeDirection(i,j) != Vector2Int.zero)
                {
                    GetTileAtLocation(i,j).toggleWall(true);
                }
            }
        }
    }

    /// <summary>
    /// Given tile coordinate, returns direction of edge or zero if not edge
    /// </summary>
    public Vector2Int GetEdgeDirection(int x, int z)
    {
        Vector2Int tile = new(x, z);
        Vector2Int[] cardinals = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
        foreach (Vector2Int cardinal in cardinals)
        {
            Vector2Int neighbor = tile + cardinal;
            if (!CoordinateToTile.ContainsKey(neighbor))
            {
                return cardinal;
            }
        }

        return Vector2Int.zero;
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

        Vector2Int startPos = TileToCoordinate[centerTile];

        Queue<Vector2Int> frontier = new();
        Dictionary<Vector2Int, int> distance = new();
        frontier.Enqueue(startPos);
        distance[startPos] = 0;

        // Breadth-First Search
        while (frontier.Count > 0)
        {
            // Dequeue a tile
            Vector2Int currentPos = frontier.Dequeue();
            int currentDist = distance[currentPos];

            Tile currTile = GetTileAtLocation(currentPos.x, currentPos.y);
            tilesInRange.Add(currTile);

            // Don't enqueue neighbors beyond range
            if (currentDist >= range)
            {
                continue;
            }

            // Check neighbors
            foreach (var neighborPos in GetNeighborsPos(currentPos))
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

    public int GetRoomWidth()
    {
        return roomWidth;
    }

    public int GetRoomLength()
    {
        return roomLength;
    }
}

/// <summary>
/// Helper class for transforming positions to tile grid coordinates
/// </summary>
public static class TileHelper
{
    public static Vector2Int PositionToCoordinate(Vector3 position)
    {
        return new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
    }
}