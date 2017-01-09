using UnityEngine;
using System.Collections;

public class Action2007 : BaseAction {

	private ActionResult actionResult;

    public Action2007()
        : base((int)ActionType.PushFingerPosition)
    {
    }

    protected override void SendParameter(NetWriter writer, ActionParam actionParam)
    {
        
    }

    protected override void DecodePackage(NetReader reader)
    {
        Debug.logger.Log("Action2007 back!");
        actionResult = new ActionResult();
        actionResult["x"] = reader.getFloat();
        actionResult["y"] = reader.getFloat();
        actionResult["z"] = reader.getFloat();
        actionResult["qx"] = reader.getFloat();
        actionResult["qy"] = reader.getFloat();
        actionResult["qz"] = reader.getFloat();
        actionResult["sx"] = reader.getFloat();
        actionResult["sy"] = reader.getFloat();
        actionResult["sz"] = reader.getFloat();
        Action2007EventParam param = new Action2007EventParam();
        param.mousePosition = new Vector3(-actionResult.Get<float>("x"), actionResult.Get<float>("y"), actionResult.Get<float>("z"));
        param.ballPosition = new Vector3(-actionResult.Get<float>("qx"), actionResult.Get<float>("qy"), actionResult.Get<float>("qz"));
        param.ballVelocity = new Vector3(-actionResult.Get<float>("sx"), actionResult.Get<float>("sy"), actionResult.Get<float>("sz"));
        EventService.Instance.GetEvent<Action2007Event>().Publish(param);

        Net.Instance.ReBuildHearbeat();
    }

    public override ActionResult GetResponseData()
    {
        return actionResult;
    }
}
