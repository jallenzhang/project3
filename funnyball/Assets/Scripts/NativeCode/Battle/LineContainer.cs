using UnityEngine;
using System.Collections;

public class LineContainer : MonoBehaviour {
    public Line line;
    public float goSpeed = 1;
    public float maxLength = 20;

    [HideInInspector]
    public Transform parent;
    private bool m_stretch = false;
    private Vector3 m_dir;
    private Vector3 m_from;
    private Transform m_arrowTrans;

    private int m_connectLayer = 0;
    private Rigidbody2D m_connectedRigidBody;
	// Use this for initialization
	void Start () {
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), parent.GetComponent<Collider2D>(), true);
	}
	
	// Update is called once per frame
	void Update () {
        if (m_arrowTrans != null)
        {
            m_arrowTrans.position = transform.position;
            Rotate();
        }

	    if (m_stretch)
        {
            transform.Translate(m_dir * Time.deltaTime * goSpeed);
            line.UpdateEndPoint(transform.position);
            if (Vector3.Distance(m_from, transform.position) >= maxLength)
            {
                EventService.Instance.GetEvent<BallStatusEvent>().Publish(new BallStatusEventParam() { status = BallStatus.Abort,
                ballName = parent.name});
            }
        }

	}

    public void Init(Vector3 from, Vector3 dir, Transform arrowTrans)
    {
        m_from = from;
        m_dir = dir;
        m_arrowTrans = arrowTrans;
        if (m_arrowTrans != null)
            m_arrowTrans.gameObject.SetActive(true);
        m_stretch = true;
        line.UpdateStartPoint(from);
        line.UpdateEndPoint(from);
        //Rotate();
    }

    void Rotate()
    {
        float dotValue = Vector3.Dot(new Vector3(0, 1, 0), (transform.position - parent.position).normalized);
        float angle = Mathf.Acos(dotValue) * Mathf.Rad2Deg;

        if (m_dir.x < 0)
        {
            angle = -angle;
        }

        if (m_arrowTrans != null)
            m_arrowTrans.rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
    }

    public void Stop()
    {
        m_stretch = false;
        EventService.Instance.GetEvent<BallStatusEvent>().Publish(new BallStatusEventParam() { status = BallStatus.Running,
        ballName = parent.name});
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        int hardLayerMask = 1 << 3; //hard things layer
        int springLayerMask = 1 << 4; //spring things layer
        int playerLayerMask = hardLayerMask + 1; //player layer

        if (other.name == parent.name)
        {
            //if it contact itself, then return;
            return;
        }

        if (other.gameObject.layer == hardLayerMask
            || other.gameObject.layer == springLayerMask
            || other.gameObject.layer == playerLayerMask)
        {
            m_connectLayer = other.gameObject.layer;
            EventService.Instance.GetEvent<BallStatusEvent>().Publish(new BallStatusEventParam() { status = BallStatus.DrawingLineFinished,
            ballName = parent.name});
            if (other.gameObject.layer == springLayerMask || playerLayerMask == other.gameObject.layer)
            {
                GetComponent<SpriteRenderer>().enabled = false;
                m_connectedRigidBody = other.GetComponent<Rigidbody2D>();
            }
        }
    }

    public int GetConnectLayer()
    {
        return m_connectLayer;
    }

    public GameObject GetConnectedObject()
    {
        return m_connectedRigidBody.gameObject;
    }

    void OnDestroy()
    {
        if (m_arrowTrans != null)
        {
            m_arrowTrans.gameObject.SetActive(false);
        }
    }
}
