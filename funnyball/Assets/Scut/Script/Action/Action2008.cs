using UnityEngine;
using System.Collections;

public class Action2008 : BaseAction {
    private ActionResult actionResult;
    public Action2008()
        : base((int)ActionType.PushReleaseFinger)
    {
    }

    protected override void SendParameter(NetWriter writer, ActionParam actionParam)
    {
        
    }

    protected override void DecodePackage(NetReader reader)
    {
        Debug.logger.Log("Action2008 back!");
        actionResult = new ActionResult();
        actionResult["qx"] = reader.getFloat();
        actionResult["qy"] = reader.getFloat();
        actionResult["qz"] = reader.getFloat();
        actionResult["sx"] = reader.getFloat();
        actionResult["sy"] = reader.getFloat();
        actionResult["sz"] = reader.getFloat();
        Action2008EventParam param = new Action2008EventParam();
        param.ballPosition = new Vector3(-actionResult.Get<float>("qx"), actionResult.Get<float>("qy"), actionResult.Get<float>("qz"));
        param.ballVelocity = new Vector3(-actionResult.Get<float>("sx"), actionResult.Get<float>("sy"), actionResult.Get<float>("sz"));
        EventService.Instance.GetEvent<Action2008Event>().Publish(param);
        Net.Instance.ReBuildHearbeat();
    }

    public override ActionResult GetResponseData()
    {
        return actionResult;
    }
}
