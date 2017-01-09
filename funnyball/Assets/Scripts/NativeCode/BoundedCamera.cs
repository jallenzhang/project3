using UnityEngine;
using System.Collections;

public class BoundedCamera : MonoBehaviour {

    public float speed = 2f;
    public Transform min, max;
    private float aspect;
    private float size = 5f;
    private float largeSize = 6f;
    private void Start()
    {
        this.aspect = Camera.main.aspect;
    }

    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        if (x != 0f || y != 0f)
        {
            Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, largeSize, Time.deltaTime);
        }
        else
        {
            Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, size, Time.deltaTime);
        }
        Vector3 position = this.transform.position;
        position.x += x * Time.deltaTime * this.speed;
        position.y += y * Time.deltaTime * this.speed;
        float orthSize = Camera.main.orthographicSize;

        //if (position.x < (this.min.position.x + orthSize * this.aspect))
        //{
        //    position.x = this.min.position.x + orthSize * this.aspect;
        //}
        //else if (position.x > (this.max.position.x - orthSize * this.aspect))
        //{
        //    position.x = this.max.position.x - orthSize * this.aspect;
        //}
        //if (position.y < (this.min.position.y + orthSize))
        //{
        //    position.y = this.min.position.y + orthSize;
        //}
        //else if (position.y > (this.max.position.y - orthSize))
        //{
        //    position.y = this.max.position.y - orthSize;
        //}
        //this.transform.position = position;
    }
}
