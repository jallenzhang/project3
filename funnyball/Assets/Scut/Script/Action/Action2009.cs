using UnityEngine;
using System.Collections;

public class Action2009 : BaseAction
{
    private ActionResult actionResult;
    public Action2009()
        : base((int)ActionType.PushJoinRoom)
    {
    }

    protected override void SendParameter(NetWriter writer, ActionParam actionParam)
    {

    }

    protected override void DecodePackage(NetReader reader)
    {
        Debug.logger.Log("Action2009 back!");
        EventService.Instance.GetEvent<Action2009Event>().Publish();
        Net.Instance.ReBuildHearbeat();
    }

    public override ActionResult GetResponseData()
    {
        return actionResult;
    }
}
