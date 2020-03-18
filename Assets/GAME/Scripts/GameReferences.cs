using System.Collections.Generic;
using TMPro;
using UnityEngine;

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


    public Dictionary<int, string> Units = new Dictionary<int, string>() { 
        { 1, "Civilian" },
        { 2, "Archer" },
    };

    public void ShowInfo(int unitId)
    {
        selectionTitle.text = Units[unitId];
    }
}
