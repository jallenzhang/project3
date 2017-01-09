using System;
using System.Collections.Generic;
using UnityEngine;

public class Action1010 : BaseAction
{

    public Action1010()
        : base((int)ActionType.LeaveRoom)
    {
    }

    protected override void SendParameter(NetWriter writer, ActionParam actionParam)
    {
        writer.writeInt32("UserId", (int)NetWriter.UserID);
    }

    protected override void DecodePackage(NetReader reader)
    {
        Net.Instance.ReBuildHearbeat();
    }

    public override ActionResult GetResponseData()
    {
        return null;
    }
}
