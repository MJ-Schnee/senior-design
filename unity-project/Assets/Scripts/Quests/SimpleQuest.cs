using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using TMPro;
public class SimpleQuest: Quest
{
    public override void CheckProgress()
    {
        this.UpdateProgress();
        if (GameManager.Instance.KillCount >= 3)
        {
            GameManager.Instance.GameWin();
        }
    }
    public override void StartQuest()
    {
        GameManager.Instance.KillEvent.AddListener(this.CheckProgress);
    }
    public override string ProgressMessage()
    {
        return $"Quest: Defeat {GameManager.Instance.KillCount}/3 Enemies";
    }
}