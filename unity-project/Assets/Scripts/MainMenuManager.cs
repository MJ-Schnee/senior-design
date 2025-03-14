using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;

    public int[] CharacterList = {1,0,0,0};
    public List<Sprite> characterIcons;
    public List<GameObject> playerSelections;

    public Type QuestSelection = typeof(SimpleQuest);
    public void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }
    public void StartGame() 
    {
        SceneManager.LoadSceneAsync("GameplayDemo");
    }

    public void LoadCharacterP1()
    {
        GameObject panel = playerSelections[0];
        TMP_Dropdown dropdown = panel.GetComponentInChildren<TMP_Dropdown>();
        Image image = panel.transform.GetChild(2).gameObject.GetComponent<Image>();

        int val = dropdown.value + 1;
        CharacterList[0] = val;
        image.overrideSprite = characterIcons[val];
    }
    public void LoadCharacterP2()
    {
        GameObject panel = playerSelections[1];
        TMP_Dropdown dropdown = panel.GetComponentInChildren<TMP_Dropdown>();
        Image image = panel.transform.GetChild(2).gameObject.GetComponent<Image>();

        int val = dropdown.value;
        CharacterList[1] = val;
        image.overrideSprite = characterIcons[val];
    }
    public void LoadCharacterP3()
    {
        GameObject panel = playerSelections[2];
        TMP_Dropdown dropdown = panel.GetComponentInChildren<TMP_Dropdown>();
        Image image = panel.transform.GetChild(2).gameObject.GetComponent<Image>();

        int val = dropdown.value;
        CharacterList[2] = val;
        image.overrideSprite = characterIcons[val];
    }
    public void LoadCharacterP4()
    {
        GameObject panel = playerSelections[3];
        TMP_Dropdown dropdown = panel.GetComponentInChildren<TMP_Dropdown>();
        Image image = panel.transform.GetChild(2).gameObject.GetComponent<Image>();

        int val = dropdown.value;
        CharacterList[3] = val;
        image.overrideSprite = characterIcons[val];
    }
}