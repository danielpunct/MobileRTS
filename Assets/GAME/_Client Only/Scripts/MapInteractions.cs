using Gamelogic.Extensions;
using UnityEngine;

public class MapInteractions : Singleton<MapInteractions>
{
    [SerializeField] Transform buildingPreview;

    private void Start()
    {
        buildingPreview.gameObject.SetActive(false);
    }

    public void UpdateBuildingPreview(bool isVisible, Ray? fingerRay = null)
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
    }
}
