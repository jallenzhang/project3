using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginLogic : MonoBehaviour {
    public InputField mInputField;
    public GameObject mNameConfirm;
    public InputField mMessageInputField;
    public GameObject mMessageSend;

    public bool mUseNetwork = false;
    public string mNetworkAdress = "42.96.149.51:9001";
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void onLogin()
    {
        GameSetting.Instance.UseNetwork = mUseNetwork;
        if (GameSetting.Instance.UseNetwork)
        {
            NetWriter.SetUrl(mNetworkAdress);
            Net.Instance.Send((int)ActionType.Login, LoginCallback, null);
        }
        else
        {
            SceneManager.LoadScene("UI");
        }
    }

    private void LoginCallback(ActionResult actionResult)
    {
        if (actionResult != null && actionResult.Get<int>("GuideID") == (int)ActionType.CreateRole)
        {
            //Net.Instance.Send((int)ActionType.CreateRote, LoginCallback, null);
            mInputField.gameObject.SetActive(true);
            mNameConfirm.SetActive(true);
            return;
        }
        else if (actionResult != null && actionResult.Get<int>("GuideID") == (int)ActionType.SendMessageToWorld)
        {
            SceneManager.LoadScene("battleScene");
            //mMessageInputField.gameObject.SetActive(true);
            //mMessageSend.SetActive(true);
            return;
        }
    }

    public void onChangeName()
    {
        NetWriter.SetUrl(mNetworkAdress);// ("42.96.149.51:9001");
        ActionParam actionParam = new ActionParam();
        actionParam["roleName"] = mInputField.text;
        Net.Instance.Send((int)ActionType.CreateRole, CreateRoleCallback, actionParam);
    }

    public void SendMessage()
    {
        ActionParam actionParam = new ActionParam();
        actionParam["Message"] = mMessageInputField.text;
        Net.Instance.Send((int)ActionType.SendMessageToWorld, SendMessageToWorldCallback, actionParam);
    }

    private void CreateRoleCallback(ActionResult actionResult)
    {
        if (actionResult != null )
        {
            Debug.logger.Log(actionResult.Get<int>("GuideID"));
        }

        SceneManager.LoadScene("battleScene");
    }

    private void SendMessageToWorldCallback(ActionResult actionResult)
    {
        Debug.logger.Log("SendMessageToWorldCallback");
    }
}
