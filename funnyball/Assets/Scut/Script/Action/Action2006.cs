using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Action2006 : BaseAction
{
    private ActionResult actionResult;

    public Action2006()
        : base((int)ActionType.SendMessageToWorld)
    {
    }

    protected override void SendParameter(NetWriter writer, ActionParam actionParam)
    {
    }

    protected override void DecodePackage(NetReader reader)
    {
        string msgPush = reader.readString();
        //UnityEngine.Debug.logger.Log();
        EventService.Instance.GetEvent<Action2006Event>().Publish(msgPush);
        Net.Instance.ReBuildHearbeat();
    }

    public override ActionResult GetResponseData()
    {
        return null;
    }
}
