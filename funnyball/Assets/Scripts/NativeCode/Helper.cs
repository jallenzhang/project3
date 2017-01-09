using UnityEngine;
using System.Collections;

public class Helper {
    public static Vector3 PerspectivePosToOrthographicPos(Camera perspectiveCamera, Camera OrthographicCamera, Vector3 perspectivePos)
    {
        Vector3 ret = Vector3.zero;

        Vector3 screenPos = perspectiveCamera.WorldToScreenPoint(perspectivePos);
        ret = OrthographicCamera.ScreenToWorldPoint(screenPos);
        ret = new Vector3(ret.x, ret.y, 0f);

        return ret;
    }

    public static Vector3 OrthographicPosToPerspectivePos(Camera perspectiveCamera, Camera OrthographicCamera, Vector3 orthographicPos)
    {
        Vector3 ret = Vector3.zero;

        Vector3 screenPos = OrthographicCamera.WorldToScreenPoint(orthographicPos);
        screenPos = new Vector3(screenPos.x, screenPos.y, 1f);
        ret = perspectiveCamera.ScreenToWorldPoint(screenPos);

        return ret;
    }

    public static Vector3 MousePositinToWorldPos(Camera referCamera, Vector3 mousePosition, Vector3 referObjPos)
    {
        Vector3 worldPos = referCamera.ScreenToWorldPoint(mousePosition);
        Vector3 screenPos = referCamera.WorldToScreenPoint(referObjPos);
        Vector3 curScreenPos = new Vector3(mousePosition.x, mousePosition.y, screenPos.z);
        worldPos = referCamera.ScreenToWorldPoint(curScreenPos);

        return worldPos;
    }
}
