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
    }

    public void HighlightTilesRadius(int x_cen, int y_cen, int radius)
    {
        for (int i = x_cen - radius; i < x_cen + radius; i++)
        {
            for (int j = y_cen - radius; j < y_cen + radius; j++)
            {
                float dist = Vector3.Distance(new Vector3(x_cen, 0, y_cen), new Vector3(i, 0, j));
                if (dist < radius) {
                    TileGrid_coord.TryGetValue((i, j), out GameObject tile);
                    tile.TryGetComponent(out Tile tileComponent);
                    if (tileComponent.IsWalkable)
                    {
                        tileComponent.ToggleHighlight(true);
                        highlightedTiles.Add(tile);
                    }
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
            current = cameFrom[current.Value];
        }
        path.Reverse();
        return path;
    }
}
