using UnityEngine;

public class FlyCam : MonoBehaviour
{
    public float moveAccel = 10;
    public float maxSpeed = 1;
    public float drag = .05f;
    static string fAxis = "Vertical";
    static string hAxis = "Horizontal";
    static string vAxis = "Hover";

    void Start()
    {

    }

    void FixedUpdate()
    {
        Rigidbody body = GetComponent<Rigidbody>();
        Vector3 inputVec = transform.forward * Input.GetAxis(fAxis) + 
            transform.right * Input.GetAxis(hAxis) + 
            transform.up * Input.GetAxis(vAxis);
        body.velocity += inputVec * moveAccel * Time.fixedDeltaTime;
        //drag
        if (body.velocity.magnitude < drag * Time.fixedDeltaTime)
        {
            body.velocity = Vector3.zero;
        } else
        {
            body.velocity = body.velocity - body.velocity.normalized * drag * Time.fixedDeltaTime;
        }
        //max speed
        if(body.velocity.magnitude > maxSpeed)
        {
            body.velocity = body.velocity.normalized * maxSpeed;
        }
    }
}
