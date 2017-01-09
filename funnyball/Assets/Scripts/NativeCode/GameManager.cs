using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public BallController ball;
    public MouseListener mouseListener;
    [HideInInspector]
    public Vector3 currentPos = new Vector2(-100, -100);
    private Vector3 defaultPos = new Vector2(-100, -100);
    
    public BallController enemyBall;
    public EnemyBallListener enemyBallListener;
    [HideInInspector]
    public Vector3 enemyCurrentPos = new Vector2(100, -100);
    private Vector3 enemyDefaultPos = new Vector2(100, -100);

    void Awake()
    {
        if (GameSetting.Instance.UseNetwork)
        {
            UIWaitingDialog.PopUp();
        }
    }

	// Use this for initialization
	void Start () {
        Reset();
        mouseListener.gm = this;

        if (GameSetting.Instance.UseNetwork)
        {
            CreateOrJoinRoom();
            ResetEnemy();
            enemyBallListener.gm = this;
            EventService.Instance.GetEvent<NetworkStatusEvent>().Subscribe(OnNetworkStatusChanged);
        }
	}

    void OnDestroy()
    {
        if (GameSetting.Instance.UseNetwork)
        {
            EventService.Instance.GetEvent<NetworkStatusEvent>().Unsubscribe(OnNetworkStatusChanged);
        }
    }

	// Update is called once per frame
	void Update () {
	
	}

    private void CreateOrJoinRoom()
    {
        Net.Instance.Send((int)ActionType.CreateOrJoinRoom, null, null);
    }

    private void OnNetworkStatusChanged(NetworkStatusEventParam status)
    {
        switch(status)
        {
            case NetworkStatusEventParam.Closed:
                UITips.PopUp(new UITipsParam() { 
                    Content = "aaaaaa", 
                    Callback = () => { SceneManager.LoadScene("initScene"); }
                });
                break;
        }
    }
	

    public void Reset()
    {
        currentPos = defaultPos;
        ball.UpdateDir(Vector3.zero);
    }

    public void UpdateDir(Vector3 pos)
    {
        if (currentPos != defaultPos)
            return;

        currentPos = pos;
        Vector3 dir = currentPos - ball.transform.position;
        ball.UpdateDir(dir.normalized, pos);
    }

    public void ResetEnemy()
    {
        if (enemyBall.gameObject.activeSelf)
        {
            enemyCurrentPos = enemyDefaultPos;
            enemyBall.UpdateDir(Vector3.zero);
        }
    }

    public void EnemyUpdateDir(Vector3 pos)
    {
        if (enemyBall.gameObject.activeSelf)
        {
            if (enemyCurrentPos != enemyDefaultPos)
                return;

            enemyCurrentPos = pos;
            Vector3 dir = enemyCurrentPos - enemyBall.transform.position;
            enemyBall.UpdateDir(dir.normalized, pos);
        }
    }
}
