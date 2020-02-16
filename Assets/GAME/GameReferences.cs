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
}
