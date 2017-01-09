using System;
using System.Collections.Generic;
using UnityEngine;

public class Action1008 : BaseAction
{
    private ActionResult actionResult;

    public Action1008()
        : base((int)ActionType.ReleaseFinger)
    {
    }

    protected override void SendParameter(NetWriter writer, ActionParam actionParam)
    {
        writer.writeString("qx", actionParam.Get<string>("qx"));
        writer.writeString("qy", actionParam.Get<string>("qy"));
        writer.writeString("qz", actionParam.Get<string>("qz"));
        writer.writeString("sx", actionParam.Get<string>("sx"));
        writer.writeString("sy", actionParam.Get<string>("sy"));
        writer.writeString("sz", actionParam.Get<string>("sz"));
    }

    protected override void DecodePackage(NetReader reader)
    {
        Debug.logger.Log("Action1008 back!");
        Net.Instance.ReBuildHearbeat();
    }

    public override ActionResult GetResponseData()
    {
        return actionResult;
    }
}
