using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChatLogic : MonoBehaviour {
    public InputField textInput;
    public Transform mChatRoot;
    public GameObject mSendItem;
    public GameObject mReceiveItem;

	// Use this for initialization
	void Start () {
        EventService.Instance.GetEvent<Action2006Event>().Subscribe(ReceiveMessageFromWorldCallback);
	}

    void OnDestroy()
    {
        EventService.Instance.GetEvent<Action2006Event>().Unsubscribe(ReceiveMessageFromWorldCallback);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void onSendMessage()
    {
        ActionParam actionParam = new ActionParam();
        actionParam["Message"] = textInput.text;
        Net.Instance.Send((int)ActionType.SendMessageToWorld, SendMessageToWorldCallback, actionParam);
    }

    private void SendMessageToWorldCallback(ActionResult actionResult)
    {
        GameObject obj = (GameObject)Instantiate(mSendItem);
        obj.transform.parent = mChatRoot;
        obj.transform.localScale = Vector3.one;
        obj.GetComponent<Text>().text = textInput.text;

        UpdateGridSize();

        textInput.text = string.Empty; //reset input field
        Debug.logger.Log("SendMessageToWorldCallback");
    }

    private void UpdateGridSize()
    {
        mChatRoot.GetComponent<RectTransform>().sizeDelta = new Vector2(600f, mChatRoot.childCount * mChatRoot.GetComponent<GridLayoutGroup>().cellSize.y);
    }

    private void ReceiveMessageFromWorldCallback(string message)
    {
        GameObject obj = (GameObject)Instantiate(mReceiveItem);
        obj.transform.parent = mChatRoot;
        obj.transform.localScale = Vector3.one;
        obj.GetComponent<Text>().text = message;

        UpdateGridSize();

        Debug.logger.Log("ReceiveMessageFromWorldCallback");
    }
}
