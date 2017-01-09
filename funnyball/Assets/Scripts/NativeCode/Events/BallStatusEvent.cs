using UnityEngine;
using System.Collections;

public class BallStatusEventParam
{
    public BallStatus status;
    public string ballName;
}

public class BallStatusEvent : EventBase<BallStatusEventParam>
{

}
