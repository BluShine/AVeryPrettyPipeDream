using UnityEngine;
using System.Collections.Generic;

public class PipeMaker : MonoBehaviour {

    static float RADIUS = .5f;
    static int RADIALSEGMENTS = 10;
    static float JOINTSIZE = 1;
    static float FLANGEWIDTH = .2f;
    static float FLANGELENGTH = .4f;

    public GameObject pipeSegment;

    public int numPipes = 10;
    public float pipeLength = 10;
    public float pipeIterations = 10;

    public Vector3 pLimits = new Vector3(100, 100, 100);

	// Use this for initialization
	void Start () {
        generatePipes();
	}

    [BitStrap.Button]
    void generatePipes()
    {
        for (int i = 0; i < numPipes; i++)
        {
            Vector3 pipePos = new Vector3(Random.Range(-pLimits.x, pLimits.x),
                Random.Range(-pLimits.y, pLimits.y), Random.Range(-pLimits.z, pLimits.z));
            Quaternion pipeRot = Random.rotation;
            Transform pipeParent = new GameObject().transform;
            Color color = Random.ColorHSV();
            for (int j = 0; j < pipeIterations; j++)
            {
                GameObject pipe = GameObject.Instantiate(pipeSegment);
                pipe.transform.parent = pipeParent;
                pipe.transform.position = pipePos;
                float len = Random.Range(1, pipeLength);
                pipe.GetComponent<MeshFilter>().mesh = makePipe(len);
                pipe.transform.rotation = pipeRot;
                if (j != pipeIterations - 1)
                {
                    //generate joint
                    pipePos = pipePos + pipeRot * transform.up * (len + JOINTSIZE);
                    pipeRot = pipeRot * Quaternion.Euler(90, 0, Random.Range(0, 360));
                    GameObject joint = GameObject.Instantiate(pipeSegment);
                    joint.transform.parent = pipe.transform;
                    joint.transform.position = pipePos + pipeRot * new Vector3(0, JOINTSIZE, JOINTSIZE);
                    joint.GetComponent<MeshFilter>().mesh = makeJoint();
                    joint.transform.rotation = pipeRot * Quaternion.Euler(180, 90, 0);
                    pipePos = pipePos + pipeRot * transform.up * JOINTSIZE;
                }
            }
        }
    }

    Mesh makeJoint()
    {
        Mesh m = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();

        addBentTube(verts, tris, RADIUS, JOINTSIZE, 5);//bend

        m.SetVertices(verts);
        m.triangles = tris.ToArray();
        m.RecalculateNormals();
        return m;
    }

    Mesh makePipe(float length)
    {
        Mesh m = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();

        addTube(verts, tris, 0, length - FLANGELENGTH, RADIUS);//tube segment

        addTube(verts, tris, length - FLANGELENGTH, length, RADIUS + FLANGEWIDTH);//flange
        addTube(verts, tris, 0, FLANGELENGTH, RADIUS + FLANGEWIDTH);//flange

        addDisc(verts, tris, 0, RADIUS + FLANGEWIDTH, false);//bottom
        addDisc(verts, tris, length, RADIUS + FLANGEWIDTH, true);//top
        addDisc(verts, tris, length - FLANGELENGTH, RADIUS + FLANGEWIDTH, false);//flange
        addDisc(verts, tris, FLANGELENGTH, RADIUS + FLANGEWIDTH, true);//flange

        m.SetVertices(verts);
        m.triangles = tris.ToArray();
        m.RecalculateNormals();
        return m;
    }

    void addBentTube(List<Vector3> verts, List<int> tris, float rad, float length, int segments)
    {
        Vector3 offset = new Vector3(length, 0, 0);
        //starting ring of verts
        verts.Add(offset + new Vector3(rad, 0, 0));
        for (int j = 1; j < RADIALSEGMENTS; j++)
        {
            float r = ((float)j / (float)RADIALSEGMENTS) * Mathf.PI * 2;
            verts.Add(offset + new Vector3(rad * Mathf.Cos(r), 0, rad * Mathf.Sin(r)));
        }

        for(int i = 1; i < segments + 1; i++)
        {
            float segRatio = (float)i / (float)segments;
            Quaternion tubeRotation = Quaternion.Euler(0, 0, segRatio * 90);
            verts.Add(tubeRotation * (offset + new Vector3(rad, 0, 0)));
            for(int j = 1; j < RADIALSEGMENTS; j++)
            {
                float r = ((float)j / (float)RADIALSEGMENTS) * Mathf.PI * 2;
                verts.Add(tubeRotation * (offset + new Vector3(rad * Mathf.Cos(r), 0, rad * Mathf.Sin(r))));
                //tri 1
                tris.Add(verts.Count - 2);
                tris.Add(verts.Count - 1);
                tris.Add(verts.Count - 1 - RADIALSEGMENTS);
                //tri 2
                tris.Add(verts.Count - 2);
                tris.Add(verts.Count - 1 - RADIALSEGMENTS);
                tris.Add(verts.Count - 2 - RADIALSEGMENTS);
            }
            //last segment
            //tri 1
            tris.Add(verts.Count - RADIALSEGMENTS);
            tris.Add(verts.Count - RADIALSEGMENTS - RADIALSEGMENTS);
            tris.Add(verts.Count - 1 - RADIALSEGMENTS);
            //tri 2
            tris.Add(verts.Count - 1);
            tris.Add(verts.Count - RADIALSEGMENTS);
            tris.Add(verts.Count - 1 - RADIALSEGMENTS);
        }
    }

    void addTube(List<Vector3> verts, List<int> tris, float bottom, float top, float rad)
    {
        //starting verts
        int start = verts.Count;
        verts.Add(new Vector3(rad, bottom, 0));//first bottom
        verts.Add(new Vector3(rad, top, 0));//first top
        //build tube
        for (int i = 1; i < RADIALSEGMENTS; i++)
        {
            float r = ((float)i / (float)RADIALSEGMENTS) * Mathf.PI * 2;
            float x = Mathf.Cos(r) * rad;
            float z = Mathf.Sin(r) * rad;
            verts.Add(new Vector3(x, bottom, z));
            verts.Add(new Vector3(x, top, z));
            //side 1
            tris.Add(verts.Count - 2);
            tris.Add(verts.Count - 4);
            tris.Add(verts.Count - 3);
            //side 2
            tris.Add(verts.Count - 3);
            tris.Add(verts.Count - 1);
            tris.Add(verts.Count - 2);
        }
        //last segment
        //side 1
        tris.Add(verts.Count - 2);
        tris.Add(start + 1);
        tris.Add(start);
        //side 2
        tris.Add(start + 1);
        tris.Add(verts.Count - 2);
        tris.Add(verts.Count - 1);
    }

    void addDisc(List<Vector3> verts, List<int> tris, float y, float rad, bool faceUp)
    {
        int start = verts.Count;
        verts.Add(new Vector3(0, y, 0));//center
        verts.Add(new Vector3(rad, y, 0));//start

        for (int i = 1; i < RADIALSEGMENTS + 1; i++)
        {
            float r = ((float)i / (float)RADIALSEGMENTS) * Mathf.PI * 2;
            float x = Mathf.Cos(r) * rad;
            float z = Mathf.Sin(r) * rad;
            verts.Add(new Vector3(x, y, z));
            tris.Add(start);
            if (faceUp)
            {
                tris.Add(verts.Count - 1);
                tris.Add(verts.Count - 2);
            }
            else {
                tris.Add(verts.Count - 2);
                tris.Add(verts.Count - 1);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
