using UnityEngine;

public class ServerReferences : MonoBehaviour
{
    [HideInInspector]
    public static ServerReferences Instance;

    private void Awake()
    {
        Instance = this;
    }

    //[SerializeField] GameLiftServerController GameLiftServer;

    public Plane raycastPlane = new Plane(Vector3.up, Vector3.zero);

    public Transform spawnA;
    public Transform spawnB;
}
