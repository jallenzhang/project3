using System;
using System.Collections.Generic;
using UnityEngine;

public class Action1009 : BaseAction
{
    private ActionResult actionResult;

    public Action1009()
        : base((int)ActionType.CreateOrJoinRoom)
    {
    }

    protected override void SendParameter(NetWriter writer, ActionParam actionParam)
    {
        writer.writeInt32("UserId", (int)NetWriter.UserID);
    }

    protected override void DecodePackage(NetReader reader)
    {
        
    }

    public override ActionResult GetResponseData()
    {
        return actionResult;
    }
}
