using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;

    public int[] CharacterList = {1,0,0,0};
    public TMP_Dropdown PlayerOneSelection;
    public TMP_Dropdown PlayerTwoSelection;
    public TMP_Dropdown PlayerThreeSelection;
    public TMP_Dropdown PlayerFourSelection;
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
        CharacterList[0] = PlayerOneSelection.value + 1;
    }
    public void LoadCharacterP2()
    {
        CharacterList[1] = PlayerTwoSelection.value;
    }
    public void LoadCharacterP3()
    {
        CharacterList[2] = PlayerThreeSelection.value;
    }
    public void LoadCharacterP4()
    {
        CharacterList[3] = PlayerFourSelection.value;
    }
}