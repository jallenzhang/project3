using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class UIPauseDialogParam : DialogParam
{
    public Action Callback;
}

public class UIPauseDialog : BaseDialog<UIPauseDialogParam>
{
    private static string prefabPath = "Prefabs/Dialogs/UIPauseDialog";

    public static void PopUp(UIPauseDialogParam param)
    {
        DialogManager.Instance.PopupDialog<UIPauseDialogParam>(prefabPath, param);
    }

    public void OnContinue()
    {
        if (this.Param.Callback != null)
            this.Param.Callback();

        CloseDialog();
    }

    public void OnRestart()
    {
        SceneManager.LoadScene("battleScene");
    }

    public void OnExit()
    {
        SceneManager.LoadScene("UI");
    }

    private void CloseDialog()
    {
        DialogManager.Instance.CloseDialog();
    }
}
