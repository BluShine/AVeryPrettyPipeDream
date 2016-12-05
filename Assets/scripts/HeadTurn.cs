using UnityEngine;
using System.Collections;

public class HeadTurn : MonoBehaviour {

    Quaternion defaultRot;
    Quaternion target;
    bool targeting = false;
    float lookLerp = 0;

	// Use this for initialization
	void Start () {
        defaultRot = transform.rotation;
        target = defaultRot;
	}

    void OnTriggerStay(Collider other)
    {
        if(other.GetComponent<Camera>() != null)
        {
            targeting = true;
            target = Quaternion.FromToRotation(Vector3.forward, 
                (other.transform.position - transform.position).normalized);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Camera>() != null)
        {
            targeting = false;
        }
    }

    // Update is called once per frame
    void Update () {
	    if(targeting)
        {
            lookLerp = Mathf.Min(1, lookLerp + Time.deltaTime);
        } else
        {
            lookLerp = Mathf.Max(0, lookLerp - Time.deltaTime);
        }
        transform.rotation = Quaternion.Lerp(defaultRot, target, lookLerp);
	}
}
