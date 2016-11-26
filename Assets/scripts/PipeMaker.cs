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

    public float layerScale = 20;
    public float layerDistribution = 2;
    public float outerLayerIterations = 3;
    public float layerRangeInner = .7f;
    public float layerRangeOuter = 2;

    public int pipeLayers = 3;

    public Vector3 pLimits = new Vector3(100, 100, 100);

	// Use this for initialization
	void Start () {
        generatePipes();
        generateOuterPipeLayers();
    }

    [BitStrap.Button]
    void generateOuterPipeLayers()
    {
        float scale = layerScale;
        float distribution = numPipes * layerDistribution;
        float innerRange = pLimits.x * layerRangeInner;
        float outerRange = pLimits.x * layerRangeOuter;
        for(int i = 0; i < pipeLayers; i++)
        {
            generatePipeLayer(scale, distribution, innerRange, outerRange);
            scale = scale * layerScale;
            distribution = numPipes * layerDistribution;
            innerRange = outerRange * layerRangeInner;
            outerRange = outerRange * layerRangeOuter;
        }
    }

    void generatePipeLayer(float scale, float num, float inner, float outer)
    {
        Transform layerParent = new GameObject().transform;
        layerParent.name = "layer " + scale;
        for (int i = 0; i < num; i++)
        {
            Color tubeCol = randBrightColor();
            Color flangeCol = randBrightColor();
            Vector3 pipePos = Random.onUnitSphere * Random.Range(inner, outer);
            Quaternion pipeRot = Quaternion.FromToRotation(Vector3.up, pipePos.normalized) * Quaternion.Euler(Random.Range(-90, 90), Random.Range(-90, 90), Random.Range(-90, 90));
            Transform pipeParent = new GameObject().transform;
            pipeParent.parent = layerParent;
            pipeParent.name = "pipe";
            pipeParent.transform.position = pipePos;
            for (int j = 0; j < outerLayerIterations; j++)
            {
                GameObject pipe = GameObject.Instantiate(pipeSegment);
                pipe.transform.parent = pipeParent;
                pipe.transform.position = pipePos;
                float len = Random.Range(1, pipeLength);
                pipe.GetComponent<MeshFilter>().mesh = makePipe(len, tubeCol, flangeCol);
                pipe.transform.rotation = pipeRot;
                if (j != outerLayerIterations - 1)
                {
                    //generate joint
                    pipePos = pipePos + pipeRot * transform.up * (len + JOINTSIZE);
                    pipeRot = pipeRot * Quaternion.Euler(90, 0, Random.Range(0, 360));
                    GameObject joint = GameObject.Instantiate(pipeSegment);
                    joint.transform.parent = pipe.transform;
                    joint.transform.position = pipePos + pipeRot * new Vector3(0, JOINTSIZE, JOINTSIZE);
                    joint.GetComponent<MeshFilter>().mesh = makeJoint(tubeCol, tubeCol);
                    joint.transform.rotation = pipeRot * Quaternion.Euler(180, 90, 0);
                    pipePos = pipePos + pipeRot * transform.up * JOINTSIZE;
                }
            }
            pipeParent.localScale = new Vector3(scale, scale, scale);
        }
    }

    [BitStrap.Button]
    void generatePipes()
    {
        Transform groupParent = new GameObject().transform;
        groupParent.name = "pipe group";
        for (int i = 0; i < numPipes; i++)
        {
            Color tubeCol = randBrightColor();
            Color flangeCol = randBrightColor();
            Vector3 pipePos = new Vector3(Random.Range(-pLimits.x, pLimits.x),
                Random.Range(-pLimits.y, pLimits.y), Random.Range(-pLimits.z, pLimits.z));
            Quaternion pipeRot = Random.rotation;
            Transform pipeParent = new GameObject().transform;
            pipeParent.name = "pipe";
            pipeParent.parent = groupParent;
            pipeParent.transform.position = pipePos;
            for (int j = 0; j < pipeIterations; j++)
            {
                GameObject pipe = GameObject.Instantiate(pipeSegment);
                pipe.transform.parent = pipeParent;
                pipe.transform.position = pipePos;
                float len = Random.Range(1, pipeLength);
                pipe.GetComponent<MeshFilter>().mesh = makePipe(len, tubeCol, flangeCol);
                pipe.transform.rotation = pipeRot;
                if (j != pipeIterations - 1)
                {
                    //generate joint
                    pipePos = pipePos + pipeRot * transform.up * (len + JOINTSIZE);
                    pipeRot = pipeRot * Quaternion.Euler(90, 0, Random.Range(0, 360));
                    GameObject joint = GameObject.Instantiate(pipeSegment);
                    joint.transform.parent = pipe.transform;
                    joint.transform.position = pipePos + pipeRot * new Vector3(0, JOINTSIZE, JOINTSIZE);
                    joint.GetComponent<MeshFilter>().mesh = makeJoint(tubeCol, tubeCol);
                    joint.transform.rotation = pipeRot * Quaternion.Euler(180, 90, 0);
                    pipePos = pipePos + pipeRot * transform.up * JOINTSIZE;
                }
            }
        }
    }

    Mesh makeJoint(Color col1, Color col2)
    {
        Mesh m = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Color> colors = new List<Color>();

        addBentTube(verts, tris, colors, RADIUS, JOINTSIZE, 5, col1, col2);//bend

        m.SetVertices(verts);
        m.triangles = tris.ToArray();
        m.SetColors(colors);
        m.RecalculateNormals();
        return m;
    }

    Color randBrightColor()
    {
        return Random.ColorHSV(0, 1, .5f, 1, 1, 1);
    }

    Mesh makePipe(float length, Color tubeColor, Color flangeColor)
    {
        Mesh m = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Color> colors = new List<Color>();

        addTube(verts, tris, colors, 0, length - FLANGELENGTH, RADIUS, tubeColor, tubeColor);//tube segment

        addTube(verts, tris, colors, length - FLANGELENGTH, length, RADIUS + FLANGEWIDTH, flangeColor, flangeColor);//flange
        addTube(verts, tris, colors, 0, FLANGELENGTH, RADIUS + FLANGEWIDTH, flangeColor, flangeColor);//flange

        addDisc(verts, tris, colors, 0, RADIUS + FLANGEWIDTH, false, flangeColor, flangeColor);//bottom
        addDisc(verts, tris, colors, length, RADIUS + FLANGEWIDTH, true, flangeColor, flangeColor);//top
        addDisc(verts, tris, colors, length - FLANGELENGTH, RADIUS + FLANGEWIDTH, false, flangeColor, flangeColor);//bottom flange inner edge
        addDisc(verts, tris, colors, FLANGELENGTH, RADIUS + FLANGEWIDTH, true, flangeColor, flangeColor);//top flange flange inner edge

        m.SetVertices(verts);
        m.triangles = tris.ToArray();
        m.SetColors(colors);
        m.RecalculateNormals();
        return m;
    }

    void addBentTube(List<Vector3> verts, List<int> tris, List<Color> colors, float rad, float length, int segments, Color startCol, Color endCol)
    {
        Vector3 offset = new Vector3(length, 0, 0);
        //starting ring of verts
        verts.Add(offset + new Vector3(rad, 0, 0));
        colors.Add(startCol);
        for (int j = 1; j < RADIALSEGMENTS; j++)
        {
            float r = ((float)j / (float)RADIALSEGMENTS) * Mathf.PI * 2;
            verts.Add(offset + new Vector3(rad * Mathf.Cos(r), 0, rad * Mathf.Sin(r)));
            colors.Add(startCol);
        }

        for(int i = 1; i < segments + 1; i++)
        {
            float segRatio = (float)i / (float)segments;
            Quaternion tubeRotation = Quaternion.Euler(0, 0, segRatio * 90);
            verts.Add(tubeRotation * (offset + new Vector3(rad, 0, 0)));
            colors.Add(Color.Lerp(startCol, endCol, segRatio));
            for (int j = 1; j < RADIALSEGMENTS; j++)
            {
                float r = ((float)j / (float)RADIALSEGMENTS) * Mathf.PI * 2;
                verts.Add(tubeRotation * (offset + new Vector3(rad * Mathf.Cos(r), 0, rad * Mathf.Sin(r))));
                colors.Add(Color.Lerp(startCol, endCol, segRatio));
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

    void addTube(List<Vector3> verts, List<int> tris, List<Color> colors, float bottom, float top, float rad, Color bottomCol, Color topCol)
    {
        //starting verts
        int start = verts.Count;
        verts.Add(new Vector3(rad, bottom, 0));//first bottom
        colors.Add(bottomCol);
        verts.Add(new Vector3(rad, top, 0));//first top
        colors.Add(topCol);
        //build tube
        for (int i = 1; i < RADIALSEGMENTS; i++)
        {
            float r = ((float)i / (float)RADIALSEGMENTS) * Mathf.PI * 2;
            float x = Mathf.Cos(r) * rad;
            float z = Mathf.Sin(r) * rad;
            verts.Add(new Vector3(x, bottom, z));
            colors.Add(bottomCol);
            verts.Add(new Vector3(x, top, z));
            colors.Add(topCol);
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

    void addDisc(List<Vector3> verts, List<int> tris, List<Color> colors, float y, float rad, bool faceUp, Color centerCol, Color edgeCol)
    {
        int start = verts.Count;
        verts.Add(new Vector3(0, y, 0));//center
        colors.Add(centerCol);
        verts.Add(new Vector3(rad, y, 0));//start
        colors.Add(edgeCol);

        for (int i = 1; i < RADIALSEGMENTS; i++)
        {
            float r = ((float)i / (float)RADIALSEGMENTS) * Mathf.PI * 2;
            float x = Mathf.Cos(r) * rad;
            float z = Mathf.Sin(r) * rad;
            verts.Add(new Vector3(x, y, z));
            colors.Add(edgeCol);
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

        tris.Add(start);
        if (faceUp)
        {
            tris.Add(start + 1);
            tris.Add(verts.Count - 1);
        } else
        {
            tris.Add(verts.Count - 1);
            tris.Add(start + 1);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
