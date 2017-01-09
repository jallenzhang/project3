using UnityEngine;
using System.Collections;

public enum NetworkStatusEventParam
{
    Closed
}

public class NetworkStatusEvent : EventBase<NetworkStatusEventParam>
{

}
