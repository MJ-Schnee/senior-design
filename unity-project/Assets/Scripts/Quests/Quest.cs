using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using TMPro;
public abstract class Quest : MonoBehaviour
{
    private TMP_Text ProgressText;

    public void Awake() 
    {
        ProgressText = GameObject.Find("QuestText").GetComponent<TMP_Text>();
    }
    public abstract void StartQuest();
    public abstract string ProgressMessage();
    protected void UpdateProgress()
    {
        string message = this.ProgressMessage();
        ProgressText.text = message;
    }
    public abstract void CheckProgress();

    public abstract void EndQuest();

}

