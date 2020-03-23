using Gamelogic.Extensions;
using UnityEngine;

public class MapInteractions : Singleton<MapInteractions>
{
    [SerializeField] Transform buildingPreview;

    Vector3? pendingInput;

    private void Start()
    {
        buildingPreview.gameObject.SetActive(false);
    }

    public bool GetPendingInput(out Vector3 buildInput)
    {
        bool hasInput = false;
        buildInput = Vector3.zero;

        if (pendingInput != null)
        {
            buildInput = pendingInput.Value;
            hasInput = true;
        }

        pendingInput = null;
        return hasInput;
    }

    public void UpdateBuildingPreview(bool isVisible, Ray? fingerRay = null, bool sendBuildInput = false)
    {
        if (isVisible)
        {
            var ray = fingerRay.Value;
            if (GameReferences.Instance.raycastPlane.Raycast(ray, out var dist))
            {
                var p = ray.GetPoint(dist);

                buildingPreview.gameObject.SetActive(true);
                buildingPreview.position = p;
            }
        }

        else
        {
            buildingPreview.gameObject.SetActive(false);

            if (sendBuildInput)
            {
                var ray = fingerRay.Value;
                if (GameReferences.Instance.raycastPlane.Raycast(ray, out var dist))
                {
                    pendingInput = ray.GetPoint(dist);
                }
            }
        }

    }
}
