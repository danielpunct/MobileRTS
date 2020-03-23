using Lean.Touch;
using UnityEngine;
using UnityEngine.UI;

public class BuildingCardHolder : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] float yLimit = 200;

    Vector3 position;

    Vector3 dragStartPosition;
    bool isLocked;
    

    private void Start()
    {
        position = transform.position;

        LeanTouch.OnFingerDown += LeanTouch_OnFingerDown;
        LeanTouch.OnFingerSet += LeanTouch_OnFingerSet;
        LeanTouch.OnFingerUp += LeanTouch_OnFingerUp;
    }


    private void LeanTouch_OnFingerUp(LeanFinger obj)
    {
        if (isLocked)
        {
            MapInteractions.Instance.UpdateBuildingPreview(false, obj.GetRay(), true); // must swith layer !obj.IsOverGui);
        }

        isLocked = false;
        transform.position = position;
        var c = image.color;
        c.a = 1;
        image.color = c;

    }

    private void LeanTouch_OnFingerSet(LeanFinger obj)
    {
        if (isLocked)
        {
            transform.position += new Vector3(obj.ScreenDelta.x, obj.ScreenDelta.y, 0);

            if(transform.position.y > yLimit)
            {
                var c =image.color;
                c.a = 0;
                image.color = c;

                MapInteractions.Instance.UpdateBuildingPreview(true, obj.GetRay());
            }
            else
            {
                var c =image.color;
                c.a = 1;
                image.color = c;

                MapInteractions.Instance.UpdateBuildingPreview(false);
            }
        }
    }

    private void LeanTouch_OnFingerDown(LeanFinger obj)
    {
        if (!obj.IsOverGui) return;

        var element = LeanTouch.RaycastGui(obj.ScreenPosition)[0];

        if (element.gameObject == gameObject)
        {
            isLocked = true;
            dragStartPosition = obj.ScreenPosition;
        }
    }
}
