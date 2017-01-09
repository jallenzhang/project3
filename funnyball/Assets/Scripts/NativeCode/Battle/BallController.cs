using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum BallStatus
{
    Stopped,
    Paused,
    Running,
    DrawingLine,
    DrawingLineFinished,
    HidingLine,
    Abort,
}

public class BallController : MonoBehaviour {

    public float speed = 11.0F;
    public float springSpeed = 0.3f;
    public Transform ballFace;
    public Transform arrowTrans;

    //public GameObject leftSpawn;
    //public GameObject rightSpawn;

    private GameObject m_hook;
    private GameObject m_lineContainer;
    private GameObject m_fixedPoint;
    private DistanceJoint2D m_distanceJoint2D;
    private SpringJoint2D m_springJoint2D;
    private GameObject m_connectedObj;
    private BallStatus m_ballStatus = BallStatus.Stopped;

    private Vector3 m_moveDirection = Vector3.zero;
    private Vector3 m_inputDir = Vector3.zero;
    private float distance = 0;
    private Vector3 m_spawnPos;
    private Vector3 m_originalPos;
    private bool m_bDraged = false;
    private Collider2D m_collider2D;

    private const float minDistance = 0.1f;
    int m_hardLayerMask = 1 << 3; //hard things layer
    int m_springLayerMask = 1 << 4; //spring things layer
    int m_playerLayerMask = 0; //player layer
    private float oldTmpDistance = 0;

    public float m_dragSpeed = 1;
    private float m_dragDistance = 0.1f;
    private float m_currentDragDistance = 0;
    private Vector2 m_targetDragCenter = Vector2.one * 0.5f;
    private Vector2 m_currentDragCenter = Vector2.one * 0.5f;
    private float m_currentDragShapeHead = 0.3f;
    private float m_targetDragShapeHead = 0.5f;

    //private Image m_imageComponent;
    //private Material m_imageMaterial;
    void Start()
    {
        //m_imageComponent = ballFace.GetComponent<Image>();
        //m_imageMaterial = m_imageComponent.material;
        m_playerLayerMask = m_hardLayerMask + 1; 
        //m_collider2D = GetComponent<Collider2D>();
        EventService.Instance.GetEvent<BallStatusEvent>().Subscribe(BallStatusChange);

    }

    void OnDestroy()
    {
        EventService.Instance.GetEvent<BallStatusEvent>().Unsubscribe(BallStatusChange);
    }

    void Update() {
        //transform.localRotation =  GetComponent<Rigidbody2D>().velocity.x >= 0.05 ? Quaternion.Euler(Vector3.zero) : Quaternion.Euler(Vector3.up * 180);

        UpdateDragInfos();
        //m_currentDragShapeHead = Mathf.Lerp(m_currentDragShapeHead, m_targetDragShapeHead, Time.deltaTime * m_dragSpeed);

        //m_imageMaterial.SetVector("_MousePosition", new Vector4(m_currentDragCenter.x, m_currentDragCenter.y, 0, 0));
        //m_imageMaterial.SetFloat("_ShapeHead", m_currentDragShapeHead);
    }

    void UpdateDragInfos()
    {
        if (m_ballStatus == BallStatus.Running || m_ballStatus == BallStatus.Stopped)
        {
            if (m_lineContainer == null)
            {
                m_targetDragCenter = Vector2.one * 0.5f;
                m_currentDragCenter = Vector2.one * 0.5f;
                m_targetDragShapeHead = 0.3f;
                m_currentDragDistance = 0;
                return;
            }

            Vector3 targetDir = m_lineContainer.transform.position - transform.position;

            if (targetDir == Vector3.zero)
            {
                m_targetDragCenter = Vector2.one * 0.5f;
                m_currentDragCenter = Vector2.one * 0.5f;
                m_targetDragShapeHead = 0.3f;
                m_currentDragDistance = 0;
                return;
            }

            targetDir = targetDir.normalized;

            float deg = Vector3.Angle(targetDir, Vector3.up);
            //if (targetDir.x < 0)
            //    deg *= -1f;
            float rad = deg * Mathf.Deg2Rad;
            //float dragCenterX = Mathf.Sin(rad) * m_dragDistance + 0.5f;
            //float dragCentery = Mathf.Cos(rad) * m_dragDistance + 0.5f;
            //m_targetDragCenter.x = dragCenterX;
            //m_targetDragCenter.y = dragCentery;
            m_currentDragDistance = Mathf.Lerp(m_currentDragDistance, m_dragDistance, Time.deltaTime * m_dragSpeed);
            float dragCenterX = Mathf.Sin(rad) * m_currentDragDistance + 0.5f;
            float dragCenterY = Mathf.Cos(rad) * m_currentDragDistance + 0.5f;
            m_currentDragCenter.x = dragCenterX;
            m_currentDragCenter.y = dragCenterY;
            m_targetDragShapeHead = 0.5f;
        }
    }

    void FixedUpdate()
    {
        if (m_lineContainer != null)
        {
            m_spawnPos = transform.position;
            m_lineContainer.GetComponent<LineContainer>().line.UpdateStartPoint(transform.position);
        }

        switch (m_ballStatus)
        {
            case BallStatus.Running:
                {
                    int connectLayer = m_lineContainer.GetComponent<LineContainer>().GetConnectLayer();
                    
                    m_moveDirection = (m_lineContainer.transform.position - transform.position).normalized;

                    if (connectLayer == m_hardLayerMask)
                    {
                        //用来给球加力m_moveDirection
                        m_moveDirection *= speed;

                        //用distance joint控制球与链接点距离
                        if (m_distanceJoint2D == null)
                        {
                            m_distanceJoint2D = GetComponent<DistanceJoint2D>();
                            if (m_distanceJoint2D == null)
                            {
                                m_distanceJoint2D = gameObject.AddComponent<DistanceJoint2D>();
                            }
                            m_distanceJoint2D.distance = distance;
                            m_distanceJoint2D.connectedAnchor = m_lineContainer.transform.position;
                            //m_distanceJoint2D.anchor = m_inputDir.x > 0 ? new Vector2(0.5f, 0) : new Vector2(-0.5f, 0);
                            m_distanceJoint2D.anchor = Vector2.zero;//new Vector2(0.2f, 0f);
                            m_distanceJoint2D.enabled = false;
                            m_distanceJoint2D.enabled = true; //awake joint
                            m_distanceJoint2D.enableCollision = true;
                            m_distanceJoint2D.maxDistanceOnly = true;//必须设置，不然distancejoint的distance如同一根棒子一样不能比distance下了
                        }

                        //duration += Time.deltaTime * speed;
                        //m_distanceJoint2D.distance = Mathf.Lerp(distance, minDistance, duration);

                        m_distanceJoint2D.distance = Vector3.Distance(m_lineContainer.transform.position, transform.position);
                        if (m_distanceJoint2D.distance <= minDistance)
                        {
                            EventService.Instance.GetEvent<BallStatusEvent>().Publish(new BallStatusEventParam() { status = BallStatus.Stopped,
                            ballName = gameObject.name});
                        }

                        GetComponent<JelloBody>().AddForce(m_moveDirection);
                        //GetComponent<Rigidbody2D>().AddForce(m_moveDirection, ForceMode2D.Force);
                    }
                    else if (connectLayer == m_springLayerMask || m_playerLayerMask == connectLayer)
                    {
                        if (m_springJoint2D == null)
                        {
                            m_springJoint2D = GetComponent<SpringJoint2D>();
                            if (m_springJoint2D == null)
                                m_springJoint2D = gameObject.AddComponent<SpringJoint2D>();

                            m_springJoint2D.distance = distance;
                            m_springJoint2D.connectedBody = m_lineContainer.GetComponent<LineContainer>().GetConnectedObject().GetComponent<Rigidbody2D>();
                            m_springJoint2D.enabled = false;
                            m_springJoint2D.enabled = true;
                            m_springJoint2D.enableCollision = true;
                            m_springJoint2D.dampingRatio = 10f;
                            m_springJoint2D.frequency = 1f;
                        }

                        m_springJoint2D.distance = Mathf.Lerp(m_springJoint2D.distance, minDistance, Time.fixedDeltaTime * springSpeed);
                        m_lineContainer.GetComponent<LineContainer>().line.UpdateEndPoint(m_lineContainer.GetComponent<LineContainer>().GetConnectedObject().transform.position);
                    }
                    
                }
                break;
            case BallStatus.Stopped:
                {
                    if (!m_bDraged)
                    {
                        m_moveDirection = Vector3.zero;
                        
                        m_connectedObj = null;
                        if (m_lineContainer != null)
                        {
                            Destroy(m_lineContainer);
                            m_lineContainer = null;
                            if (m_fixedPoint != null)
                            {
                                Destroy(m_fixedPoint);
                                m_fixedPoint = null;
                            }

                            if (m_distanceJoint2D != null)
                            {
                                Destroy(m_distanceJoint2D);
                                m_distanceJoint2D = null;
                            }

                            if (m_springJoint2D != null)
                            {
                                Destroy(m_springJoint2D);
                                m_springJoint2D = null;
                            }

                            return;
                        }
                    }
                }
                break;
        }

        //if (GetComponent<Rigidbody2D>() != null)
        //{
        //    if (GetComponent<Rigidbody2D>().velocity.y > 0 && GetComponent<Rigidbody2D>().gravityScale > 0)
        //    {
        //        m_collider2D.isTrigger = true;
        //        //SetIgnoreLayer(true);
        //    }
        //    else
        //    {
        //        m_collider2D.isTrigger = false;
        //        //SetIgnoreLayer(false);
        //    }
        //}   
    }

    private void SetIgnoreLayer(bool ignore)
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        int hardThingsLayer = LayerMask.NameToLayer("HardThings");

        Physics2D.IgnoreLayerCollision(playerLayer, hardThingsLayer, ignore);
    }

    public void UpdateDir(Vector3 dir)
    {
        UpdateDir(dir, Vector3.zero);
    }

    public void UpdateDir(Vector3 dir, Vector3 targetPos)
    {
        if (dir == Vector3.zero)
        {
            m_bDraged = false;

            if (m_ballStatus == BallStatus.DrawingLine)
                EventService.Instance.GetEvent<BallStatusEvent>().Publish(new BallStatusEventParam() { status = BallStatus.Abort,
                ballName = gameObject.name});

            if (m_ballStatus == BallStatus.Running)
                EventService.Instance.GetEvent<BallStatusEvent>().Publish(new BallStatusEventParam() { status = BallStatus.Stopped,
                ballName = gameObject.name});
        }
        else
        {
            m_bDraged = true;
            //BallStatusEventParam param = 
            EventService.Instance.GetEvent<BallStatusEvent>().Publish(new BallStatusEventParam()
            {
                status = BallStatus.DrawingLine,
                ballName = gameObject.name
            });
            m_inputDir = dir;
            if (targetPos != Vector3.zero)
                GenerateLine(dir, targetPos);
        }
    }

    private void GenerateLine(Vector3 dir, Vector3 pos)
    {
        if (m_lineContainer == null)
        {
            m_originalPos = transform.position;

            m_lineContainer = (GameObject)Instantiate(Resources.Load("LineContainer"));
            m_lineContainer.transform.parent = transform.parent ;
            m_lineContainer.transform.position = m_originalPos;
            m_lineContainer.transform.localScale = Vector3.one * 0.2f;
            m_lineContainer.GetComponent<LineContainer>().parent = transform;
            m_lineContainer.GetComponent<LineContainer>().Init(m_originalPos, dir, arrowTrans);
        }

    }

    void BallStatusChange(BallStatusEventParam param)
    {
        Debug.logger.Log("BallStatusChange " + param.status);
        if (param.ballName != gameObject.name)
            return;

        m_ballStatus = param.status;
        switch(param.status)
        {
            case BallStatus.DrawingLineFinished:
                {
                    distance = Vector3.Distance(m_spawnPos, m_lineContainer.transform.position);
                    m_lineContainer.GetComponent<LineContainer>().Stop();

                }
                break;
            case BallStatus.Abort:
                {
                    m_bDraged = false;
                
                    m_moveDirection = Vector3.zero;
                    if (m_lineContainer != null)
                    {
                        Destroy(m_lineContainer);
                        m_lineContainer = null;
                        if (m_fixedPoint != null)
                        {
                            Destroy(m_fixedPoint);
                            m_fixedPoint = null;
                        }

                        if (m_distanceJoint2D != null)
                        {
                            Destroy(m_distanceJoint2D);
                            m_distanceJoint2D = null;
                        }
                    }
                }
                break;
        }
    }
}
