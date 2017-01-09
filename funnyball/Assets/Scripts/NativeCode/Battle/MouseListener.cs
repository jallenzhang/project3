using UnityEngine;
using System.Collections;

public class MouseListener : MonoBehaviour {
    [HideInInspector]
    public GameManager gm;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDestroy()
    {
    }

    void OnMouseDown()
    {
        Vector3 worldPos = Helper.MousePositinToWorldPos(Camera.main, Input.mousePosition, gm.ball.transform.position);
        if (GameSetting.Instance.UseNetwork)
        {
            ActionParam param = new ActionParam();
            param["x"] = worldPos.x.ToString();
            param["y"] = worldPos.y.ToString();
            param["z"] = worldPos.z.ToString();
            //Debug.logger.Log("gm.ball.transform.position " + gm.ball.transform.position);
            Vector3 screenBallPos = Camera.main.WorldToScreenPoint(gm.ball.transform.position);
            param["qx"] = gm.ball.transform.localPosition.x.ToString();
            param["qy"] = gm.ball.transform.localPosition.y.ToString();
            param["qz"] = gm.ball.transform.localPosition.z.ToString();
            param["sx"] = gm.ball.gameObject.GetComponent<Rigidbody2D>().velocity.x.ToString();
            param["sy"] = gm.ball.gameObject.GetComponent<Rigidbody2D>().velocity.y.ToString();
            param["sz"] = "0";
            Net.Instance.Send((int)ActionType.SendFingerPosition, onSendFingerPositionCallback, param);
        }
        else
        {
            gm.UpdateDir(worldPos);
        }
    }

    void onSendFingerPositionCallback(ActionResult result)
    {
        Vector3 pos = new Vector3(result.Get<float>("x"), result.Get<float>("y"), result.Get<float>("z"));
        gm.UpdateDir(pos);
    }

    void OnMouseUp()
    {
        if (GameSetting.Instance.UseNetwork)
        {
            ActionParam param = new ActionParam();
            Vector3 screenBallPos = Camera.main.WorldToScreenPoint(gm.ball.transform.position);
            param["qx"] = gm.ball.transform.localPosition.x.ToString();
            param["qy"] = gm.ball.transform.localPosition.y.ToString();
            param["qz"] = gm.ball.transform.localPosition.z.ToString();
            param["sx"] = gm.ball.gameObject.GetComponent<Rigidbody2D>().velocity.x.ToString();
            param["sy"] = gm.ball.gameObject.GetComponent<Rigidbody2D>().velocity.y.ToString();
            param["sz"] = "0";
            Net.Instance.Send((int)ActionType.ReleaseFinger, onReleaseFingerCallback, param);
        }
        else
        {
            gm.Reset();
        }
    }

    void onReleaseFingerCallback(ActionResult result)
    {
        gm.Reset();
    }

   

    /// <summary>
    /// 因为在手机上（android）默认会调用走下角OnMouseEnter这个函数，所以这里加个hack，让他不选中
    /// </summary>
    /// <returns></returns>
    IEnumerator hackCode()
    {
        yield return new WaitForEndOfFrame();
        OnMouseUp();
    }
}
