using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Action1006 : BaseAction
{
    private ActionResult actionResult;

    public Action1006()
        : base((int)ActionType.SendMessageToWorld)
    {
    }

    protected override void SendParameter(NetWriter writer, ActionParam actionParam)
    {
        writer.writeString("Pid", GameSetting.Instance.Pid);
        writer.writeInt64("UserId", NetWriter.UserID);
        writer.writeString("Message", actionParam.Get<string>("Message"));
    }

    protected override void DecodePackage(NetReader reader)
    {
        
    }

    public override ActionResult GetResponseData()
    {
        return actionResult;
    }
}
