using UnityEngine;
using System.Collections;

public class UIWaitingDialog : DialogBase
{
    public static void PopUp()
    {
        DialogManager.Instance.PopupDialog("Prefabs/Dialogs/UIWaitingDialog");
    }

    void Start()
    {
        EventService.Instance.GetEvent<Action2009Event>().Subscribe(OnMatched);
    }

    void OnDestroy()
    {
        EventService.Instance.GetEvent<Action2009Event>().Unsubscribe(OnMatched);
    }

    void OnMatched()
    {
        Destroy(gameObject);
    }
}
