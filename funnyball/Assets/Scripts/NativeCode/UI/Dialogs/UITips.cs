using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UITipsParam : DialogParam
{
    public string Title;
    public string Content;
    public string Confirm;
    public Action Callback;
}

public class UITips : BaseDialog<UITipsParam>
{
    public Text mTitle;
    public Text mContent;
    public Text mConfirm;

    private static string prefabPath = "Prefabs/Dialogs/UITips";
    public static void PopUp(UITipsParam param)
    {
        DialogManager.Instance.PopupDialog<UITipsParam>(prefabPath, param);
    }

    void Start()
    {
        mTitle.text = this.Param.Title;
        mContent.text = this.Param.Content;
        mConfirm.text = this.Param.Confirm;
    }

    public void onConfirm()
    {
        if (this.Param.Callback != null)
            this.Param.Callback();

        DialogManager.Instance.CloseDialog();
    }
}
