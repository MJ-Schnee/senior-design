using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerClickHandler
{
    public bool IsHighlighted;

    // Varibales so we can check what type of tile we have and assign correct properties
    private bool IsDoor;
    private bool IsWall;
    private bool IsEnemySpawn;

    public bool IsWalkable = true;

    [SerializeField]
    private List<Renderer> renderers;

    [SerializeField]
    private Color baseC = Color.blue;
    private Color color = Color.white;
    private Color doorC = Color.red;
    private Color wallC = Color.black;
    private Color enemyC = Color.green;

    //helper list to cache all the materials of this object
    private List<Material> materials;

    //Gets all the materials from each renderer
    private void Awake()
    {
        IsHighlighted = false;

        materials = new List<Material>();
        foreach (var renderer in renderers)
        {
            //A single child-object might have mutliple materials on it
            //that is why we need to all materials with "s"
            materials.AddRange(new List<Material>(renderer.materials));
        }
    }

    // Getter functions, good practice to make these variables private and use these to get their current value
    public bool getDoor()
    {
        return IsDoor;
    }
    public bool getWall()
    {
        return IsWall;
    }
    public bool getEnemySpawn()
    {
        return IsEnemySpawn;
    }

    // Toggle funcitons, all basically turn the tile into another tile,
    // IE exampleTile.toggleDoor(true) turns exampleTile into a Door tile by changing its color
    // Since a tile can have IsDoor and IsWall true at the same time these are toggles so for example
    // I want to change a wall tile to a door tile I run toggleWall(false) and toggleDoor(true)
    public void toggleDoor(bool isDoor)
    {
        IsDoor = isDoor;
        if (isDoor)
        {
            foreach (var material in materials)
            {
                //set the color
                material.SetColor("_Color", doorC);
            }
        }
    }
    public void toggleWall(bool isWall)
    {
        IsWall = isWall;
        if (isWall)
        {
            IsWalkable = false;
            foreach (var material in materials)
            {
                //set the color
                material.SetColor("_Color", wallC);
            }
        }
        else
        {
            IsWalkable = true;
        }
    }
    public void toggleEnemy(bool isEnemySpawn)
    {
        IsEnemySpawn = isEnemySpawn;
        if (isEnemySpawn)
        {
            foreach (var material in materials)
            {
                //set the color
                material.SetColor("_Color", enemyC);
            }
        }
    }

    public void ToggleHighlight(bool isHighlighted)
    {
        IsHighlighted = isHighlighted;

        if (isHighlighted)
        {
            foreach (var material in materials)
            {
                //We need to enable the EMISSION
                material.EnableKeyword("_EMISSION");
                //before we can set the color
                material.SetColor("_EmissionColor", color);
            }
        }
        else
        {
            foreach (var material in materials)
            {
                //we can just disable the EMISSION
                //if we don't use emission color anywhere else
                material.DisableKeyword("_EMISSION");
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StartCoroutine(UiManager.Instance.HandleTileClick(this));
    }
}