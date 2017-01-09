using UnityEngine;
using System.Collections;

public class Action2010 : BaseAction
{
    private ActionResult actionResult;
    public Action2010()
        : base((int)ActionType.PushLeaveRoom)
    {
    }

    protected override void SendParameter(NetWriter writer, ActionParam actionParam)
    {

    }

    protected override void DecodePackage(NetReader reader)
    {
        Debug.logger.Log("Action2010 back!");
        EventService.Instance.GetEvent<Action2010Event>().Publish();
        Net.Instance.ReBuildHearbeat();
    }

    public override ActionResult GetResponseData()
    {
        return actionResult;
    }
}
