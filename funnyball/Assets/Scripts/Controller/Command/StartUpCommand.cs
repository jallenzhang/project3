﻿using UnityEngine;
using System.Collections;
using SimpleFramework;
using SimpleFramework.Manager;

public class StartUpCommand : ControllerCommand {

    public override void Execute(IMessage message) {
        if (!Util.CheckEnvironment()) return;

        GameObject gameMgr = GameObject.Find("GlobalGenerator");
        if (gameMgr != null) {
            AppView appView = gameMgr.AddComponent<AppView>();
        }
        //-----------------关联命令-----------------------
        AppFacade.Instance.RegisterCommand(NotiConst.DISPATCH_MESSAGE, typeof(SocketCommand));

        //-----------------初始化管理器-----------------------
        AppFacade.Instance.AddManager(ManagerName.Lua, new LuaScriptMgr());

        AppFacade.Instance.AddManager<PanelManager>(ManagerName.Panel);
        AppFacade.Instance.AddManager<MusicManager>(ManagerName.Music);
        AppFacade.Instance.AddManager(ManagerName.Timer, new TimerManager());
        AppFacade.Instance.AddManager<NetworkManager>(ManagerName.Network);
        AppFacade.Instance.AddManager<ResourceManager>(ManagerName.Resource);
        AppFacade.Instance.AddManager<ThreadManager>(ManagerName.Thread);
        AppFacade.Instance.AddManager<SimpleFramework.Manager.GameManager>(ManagerName.Game);

        Debug.Log("SimpleFramework StartUp-------->>>>>");
    }
}