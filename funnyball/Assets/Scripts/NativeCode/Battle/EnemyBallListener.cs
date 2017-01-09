using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EnemyBallListener : MonoBehaviour {
    [HideInInspector]
    public GameManager gm;
	// Use this for initialization
	void Start () {
        EventService.Instance.GetEvent<Action2009Event>().Subscribe(onEnemyComing);
        EventService.Instance.GetEvent<Action2010Event>().Subscribe(onEnemyLeave);
        EventService.Instance.GetEvent<Action2007Event>().Subscribe(onFingerPositionPushed);
        EventService.Instance.GetEvent<Action2008Event>().Subscribe(onFingerReleasePushed);
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    void OnDestroy()
    {
        EventService.Instance.GetEvent<Action2009Event>().Unsubscribe(onEnemyComing);
        EventService.Instance.GetEvent<Action2010Event>().Unsubscribe(onEnemyLeave);
        EventService.Instance.GetEvent<Action2007Event>().Unsubscribe(onFingerPositionPushed);
        EventService.Instance.GetEvent<Action2008Event>().Unsubscribe(onFingerReleasePushed);
    }

    void onEnemyLeave()
    {
        gm.enemyBall.gameObject.SetActive(false);
        UITips.PopUp(new UITipsParam()
        {
            Confirm = LanguageManager.GetText("Confirm"),
            Title = LanguageManager.GetText("Tip"),
            Content = LanguageManager.GetText("EnemyLeave"),
            Callback = () => { SceneManager.LoadScene("initScene"); }
        });
    }

    void onEnemyComing()
    {
        gm.enemyBall.gameObject.SetActive(true);
    }

    void onFingerPositionPushed(Action2007EventParam param)
    {
        gm.enemyBall.transform.localPosition = param.ballPosition;
        gm.enemyBall.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(param.ballVelocity.x, param.ballVelocity.y);
        gm.EnemyUpdateDir(new Vector3(param.mousePosition.x, param.mousePosition.y, param.mousePosition.z));
    }

    void onFingerReleasePushed(Action2008EventParam param)
    {
        gm.enemyBall.transform.localPosition = param.ballPosition;
        gm.enemyBall.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(param.ballVelocity.x, param.ballVelocity.y);
        gm.ResetEnemy();
    }
}
