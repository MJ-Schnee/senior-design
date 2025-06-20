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
    private bool IsDot;
    private bool IsPit;
    private bool IsTrap;
    private bool IsTreasure;

    public bool IsWalkable = true;

    [SerializeField]
    private List<Renderer> renderers;

    [SerializeField]
    private Color highlightColor = Color.white;

    [SerializeField]
    private Color doorC = new Color(150.0f/255.0f,75.0f/255.0f,0.0f,1.0f);

    [SerializeField]
    private Color wallC = Color.gray;

    [SerializeField]
    private Color enemyC = Color.green;

    [SerializeField]
    private Color pitC = Color.red;
    private Color treasureC = Color.yellow;

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
    public bool getTrap()
    {
        return IsTrap;
    }
    public bool getDot()
    {
        return IsDot;
    }
    public bool getPit()
    {
        return IsPit;
    }
    public bool getTreasure()
    {
        return IsTreasure;
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
    public void setTreasure(bool isTreasure)
    {
        IsTreasure = isTreasure;
        if(isTreasure)
        {
            foreach(var material in materials)
            {
                material.SetColor("_Color", treasureC);
            }
        }
    }
    public void setPit(bool isPit)
    {
        IsPit = isPit;
        if(isPit)
        {
            IsWalkable = false;
            foreach (var material in materials)
            {
                //set the color
                material.SetColor("_Color", pitC);
            }
        }
        else
        {
            IsWalkable = true;
        }
    }
    public void setTrap(bool isTrap)
    {
        IsTrap = isTrap;
    }
    public void setDot(bool isDot)
    {
        IsDot = isDot;
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
                material.SetColor("_EmissionColor", highlightColor * highlightColor.a);
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