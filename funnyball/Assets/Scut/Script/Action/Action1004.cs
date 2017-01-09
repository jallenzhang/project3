using System;
using System.Collections.Generic;
using UnityEngine;

public class Action1004 : BaseAction//GameAction
{
    private ActionResult actionResult;

    public Action1004()
        : base((int)ActionType.Login)
    {
    }

    protected override void SendParameter(NetWriter writer, ActionParam actionParam)
    {
        writer.writeString("Pid", GameSetting.Instance.Pid);
    }

    protected override void DecodePackage(NetReader reader)
    {
        actionResult = new ActionResult();
        //默认Scut流格式解包
        actionResult["SessionID"] = reader.readString();
        actionResult["Uid"] = reader.getInt();
        Debug.logger.Log(actionResult.Get<int>("Uid") + "");
        actionResult["Uname"] = reader.readString();
        Debug.logger.Log(actionResult.Get<string>("Uname"));
        actionResult["GuideID"] = reader.getInt();
        NetWriter.setUserID((ulong)actionResult.Get<int>("Uid"));
        NetWriter.setSessionID(actionResult.Get<string>("SessionID"));
		Net.Instance.ReBuildHearbeat();
    }

    public override ActionResult GetResponseData()
    {
        return actionResult;
    }
}
