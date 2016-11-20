using UnityEngine;
using System.Collections;

public class PipeMaker : MonoBehaviour {

    public GameObject pipeSegment;

    public int numPipes = 10;
    public float pipeLength = 10;
    public float pipeIterations = 10;

    public Vector3 pLimits = new Vector3(100, 100, 100);

	// Use this for initialization
	void Start () {
	    for(int i = 0; i < numPipes; i++)
        {
            Vector3 pipePos = new Vector3(Random.Range(-pLimits.x, pLimits.x), 
                Random.Range(-pLimits.y, pLimits.y), Random.Range(-pLimits.z, pLimits.z));
            Quaternion pipeRot = Random.rotation;
            Transform pipeParent = new GameObject().transform;
            Color color = Random.ColorHSV();
            for(int j = 0; j < pipeIterations; j++)
            {
                GameObject pipe = GameObject.Instantiate(pipeSegment);
                pipe.transform.parent = pipeParent;
                pipe.transform.position = pipePos;
                float len = Random.Range(1, pipeLength);
                pipe.transform.localScale = new Vector3(1, len, 1);
                pipe.transform.rotation = pipeRot;
                pipePos = pipePos + pipeRot * transform.up * len;
                pipeRot = Random.rotation;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
