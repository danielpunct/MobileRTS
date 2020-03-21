using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameReferences : MonoBehaviour
{
    [HideInInspector]
    public static GameReferences Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Plane raycastPlane = new Plane(Vector3.up, Vector3.zero);
    public Transform selection;

    public TMP_Text selectionTitle;
    public Button buildButton;

    public bool pendingBuild = false;


    public Dictionary<int, string> Units = new Dictionary<int, string>() { 
        { 1, "un om" },
        { 2, "Archer" },
    };

    public void ShowInfo(int unitId)
    {
        buildButton.gameObject.SetActive(unitId == 1);
        selectionTitle.text = Units[unitId];
    }

    public void Clear()
    {
        buildButton.gameObject.SetActive(false);
        selectionTitle.text = "";
    }

    public void OnBuildClick()
    {
        pendingBuild = true;
    }

    internal void ConsumeBuildEvent()
    {
        pendingBuild = false;
    }
}
