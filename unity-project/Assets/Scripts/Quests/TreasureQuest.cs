using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using TMPro;
public class TreasureQuest: Quest
{
    public override void CheckProgress()
    {
        this.UpdateProgress();
        if (GameManager.Instance.TreasureRoom == true)
        {
            GameManager.Instance.GameWin();
        }
    }
    public override void StartQuest()
    {
        GameManager.Instance.TreasureEvent.AddListener(this.CheckProgress);
    }
    public override string ProgressMessage()
    {
        return $"Quest: Find the Treasure Room!";
    }
}