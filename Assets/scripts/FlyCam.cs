using UnityEngine;

public class FlyCam : MonoBehaviour
{
    public float moveAccel = 10;
    public float maxSpeed = 1;
    public float drag = .05f;
    static string fAxis = "Vertical";
    static string hAxis = "Horizontal";

    Vector3 inertia;

    void Start()
    {
        inertia = Vector3.zero;
    }

    void FixedUpdate()
    {
        Vector3 inputVec = transform.forward * Input.GetAxis(fAxis) + transform.right * Input.GetAxis(hAxis);
        inertia += inputVec * moveAccel * Time.fixedDeltaTime;
        //drag
        if (inertia.magnitude < drag * Time.fixedDeltaTime)
        {
            inertia = Vector3.zero;
        } else
        {
            inertia = inertia - inertia.normalized * drag * Time.fixedDeltaTime;
        }
        //max speed
        if(inertia.magnitude > maxSpeed)
        {
            inertia = inertia.normalized * maxSpeed;
        }

        transform.position = transform.position + inertia;
    }
}
