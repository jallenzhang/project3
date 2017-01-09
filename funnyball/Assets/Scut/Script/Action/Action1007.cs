using UnityEngine;
using System.Collections;

public class Action1007 : BaseAction
{
    private Vector3 m_fingerPos;
	private ActionResult actionResult;

    public Action1007()
        : base((int)ActionType.SendFingerPosition)
    {
    }

    protected override void SendParameter(NetWriter writer, ActionParam actionParam)
    {
        writer.writeString("x", actionParam.Get<string>("x"));
        writer.writeString("y", actionParam.Get<string>("y"));
        writer.writeString("z", actionParam.Get<string>("z"));
        writer.writeString("qx", actionParam.Get<string>("qx"));
        writer.writeString("qy", actionParam.Get<string>("qy"));
        writer.writeString("qz", actionParam.Get<string>("qz"));
        writer.writeString("sx", actionParam.Get<string>("sx"));
        writer.writeString("sy", actionParam.Get<string>("sy"));
        writer.writeString("sz", actionParam.Get<string>("sz"));

        m_fingerPos = new Vector3(float.Parse(actionParam.Get<string>("x")), 
            float.Parse(actionParam.Get<string>("y")), float.Parse(actionParam.Get<string>("z")));
    }

    protected override void DecodePackage(NetReader reader)
    {
        Debug.logger.Log("Action1007 back!");
        actionResult = new ActionResult();
        actionResult["x"] = m_fingerPos.x;
        actionResult["y"] = m_fingerPos.y;
        actionResult["z"] = m_fingerPos.z;
        Net.Instance.ReBuildHearbeat();
    }

    public override ActionResult GetResponseData()
    {
        return actionResult;
    }
}
