using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class TileGridManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public Dictionary<int,Dictionary<int,GameObject>> tileGrid = new Dictionary<int,Dictionary<int,GameObject>>();
    private int min_x = -30;
    private int max_x = 30;
    private int min_y = -30;
    private int max_y = 30;
    private List<GameObject> highlightedTiles = new List<GameObject>();

    public void highlightTilesRadius(int x_cen, int y_cen, int radius)
    {
        for (int i = Mathf.Max(min_x, x_cen - radius); (i <= max_x) && (i < (x_cen + radius)); i++)
        {
            for (int j = Mathf.Max(min_y, y_cen - radius); (j <= max_y) && (j < y_cen + radius); j++)
            {
                float dist = Vector3.Distance(new Vector3(x_cen,0,y_cen), new Vector3(i,0,j));
                if (dist < radius) {
                    tileGrid[i][j].GetComponent<Highlight>()?.ToggleHighlight(true);
                    highlightedTiles.Add(tileGrid[i][j]);
                }
            }
        }
    }
    public void unhighlightAllTiles()
    {
        foreach (GameObject tile in highlightedTiles)
        {
            tile.GetComponent<Highlight>()?.ToggleHighlight(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Instantiate tiles around 0,0,0 in a 15x15 grid
        for (int i = min_x; i <= max_x; i++)
        {
            tileGrid.Add(i, new Dictionary<int, GameObject>());

            for (int j = min_y; j <= max_y; j++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(i,0,j), Quaternion.identity);
                tileGrid[i].Add(j, tile);
            }
        }
        this.highlightTilesRadius(10,0,8);
    }

    
    // CODE FOR DEMONSTRATION, REMOVE THIS AND USE "highlightTilesRadius and unhighlightALLTiles eslewhere"
    Ray ray;
    RaycastHit hit;

    void Update() {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit))
        {
            this.unhighlightAllTiles();
            Vector3 point = hit.collider.transform.position;
            this.highlightTilesRadius((int)point.x,(int)point.z,8);
        }
    }
}
