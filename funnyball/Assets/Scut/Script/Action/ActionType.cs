using System;

public enum ActionType
{
    Regist = 1002,
    Login = 1004,
    CreateRole = 1005,
    SendMessageToWorld = 1006,
    SendFingerPosition = 1007,
    ReleaseFinger = 1008,
    CreateOrJoinRoom = 1009,
    LeaveRoom = 1010,

    Push = 2000,
    PushMessageToMe = 2006,
    PushFingerPosition = 2007,
    PushReleaseFinger = 2008,
    PushJoinRoom = 2009,
    PushLeaveRoom = 2010,

}
