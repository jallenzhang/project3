using UnityEngine;
using System.Collections;
using System;

public class Hook : MonoBehaviour {
    public GameObject Trail;
    public float goSpeed = 1f;
    public float backSpeed = 1f;
    public float maxLength = 20f;

    private bool mode = true; //true:go false:back

    private bool reachTarget = false; //到达终点
    private bool reachSouce = false; //回到原点
    private Vector3 dirGo;
    private Vector3 dirBack;
    private Vector3 m_from;
    private Vector3 m_dir;
    private float stopDistance = 0.1f;
    private bool m_stretch = false;
    private float m_stretchSpeed = 1f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (m_stretch)
        {
            transform.Translate(m_dir * Time.deltaTime * goSpeed);
            if (Vector3.Distance(m_from, transform.position) >= maxLength)
            {
                m_stretch = false;
            }
        }
	}

    public void Init(Vector3 from, Vector3 dir, Action reachAction)
    {
        m_dir = dir.normalized;
        m_from = from;
        m_stretch = true;
    }

    public void Stop()
    {
        Debug.logger.Log("111111111111111111111");
        m_stretch = false;
        EventService.Instance.GetEvent<BallStatusEvent>().Publish(new BallStatusEventParam() { status = BallStatus.Running,
        ballName = gameObject.name });
    }


    //IEnumerator Go(Action reachAction)
    //{
    //    //reach the target
    //    while(!reachTarget)
    //    {
    //        yield return new WaitForEndOfFrame();
    //        float originalDistance = Vector3.Distance(Trail.transform.position, m_to);
    //        if (originalDistance > stopDistance)
    //        {
    //            Vector3 tmpPos = Trail.transform.position;
    //            tmpPos += dirGo * Time.deltaTime * goSpeed;
    //            float tmpDistance = Vector3.Distance(tmpPos, m_to);
    //            if (originalDistance < tmpDistance)
    //            {
    //                Trail.transform.position = m_to;
    //                reachTarget = true;
    //            }
    //            else
    //                Trail.transform.position = tmpPos;
    //        }
    //        else
    //        {
    //            reachTarget = true;
    //        }
    //    }

    //    //todo:play some effect


    //    //destroy 
    //    if (reachAction != null)
    //    {
    //        yield return new WaitForSeconds(0.5f);
    //        reachAction();
    //    }
    //}

    //IEnumerator Back()
    //{

    //}
}
