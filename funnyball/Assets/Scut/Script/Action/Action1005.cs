using System;
using System.Collections.Generic;
using UnityEngine;

public class Action1005 : BaseAction
{
    private ActionResult actionResult;

    public Action1005()
        : base((int)ActionType.CreateRole)
    {
    }

    protected override void SendParameter(NetWriter writer, ActionParam actionParam)
    {
        //default url param
        writer.writeString("Pid", GameSetting.Instance.Pid);
        writer.writeInt32("UserId", (int)NetWriter.UserID);
        writer.writeString("Uname", actionParam.Get<string>("roleName"));
    }

    protected override void DecodePackage(NetReader reader)
    {
        actionResult = new ActionResult();
        //默认Scut流格式解包
        actionResult["SessionID"] = reader.readString();
        actionResult["Uid"] = reader.getInt();
        actionResult["Uname"] = reader.readString();
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
