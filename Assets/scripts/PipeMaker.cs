using UnityEngine;
using System.Collections.Generic;

public class PipeMaker : MonoBehaviour {

    static float RADIUS = .5f;
    static int RADIALSEGMENTS = 20;
    static float JOINTSIZE = 1;
    static float FLANGEWIDTH = .2f;
    static float FLANGELENGTH = .4f;

    public GameObject pipeSegment;

    public Material pipeMat;

    public int numPipes = 10;
    public float pipeLength = 10;
    public int pipeIterations = 10;

    public float layerScale = 20;
    public float layerDistribution = 2;
    public int outerLayerIterations = 3;
    public float layerRangeInner = .7f;
    public float layerRangeOuter = 2;

    public int pipeLayers = 3;

    public Vector3 pLimits = new Vector3(100, 100, 100);

    public enum ColorMode{ rainbow, monochrome, gradient }
    public ColorMode colorMode = ColorMode.rainbow;
    public Color pipeColor;
    public Gradient pipeGradient;

    public enum FlangeMode { normal, different, same, inverse }
    public FlangeMode flangeMode = FlangeMode.normal;
    public ColorMode flangeColorMode = ColorMode.rainbow;
    public Color flangeColor;
    public Gradient flangeGradient;

    public bool generateColliders = false;

    int pipeIDCounter = 0;

	// Use this for initialization
	void Start () {
        Weather w = FindObjectOfType<Weather>();
        if(w != null)
        {
            pipeMat.SetColor("_RimColor", w.rimLight);
            pipeMat.SetColor("_SpecColor", w.specLight);
            pipeMat.SetTexture("_ColorRamp", w.GetComponent<GradientToTexture>().makeTexture());
        }
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
            Vector3 pos = Random.onUnitSphere * Random.Range(inner, outer);
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, pos.normalized) * Quaternion.Euler(Random.Range(-90, 90), Random.Range(-90, 90), Random.Range(-90, 90));
            Color bodyCol = getBodyColor();
            makePipe(bodyCol, getFlangeColor(bodyCol), pos, rot, layerParent, scale, outerLayerIterations);
        }
    }

    Color getBodyColor()
    {
        switch (colorMode)
        {
            case ColorMode.rainbow:
                return randBrightColor();
            case ColorMode.monochrome:
                return pipeColor;
            case ColorMode.gradient:
                return pipeGradient.Evaluate(Random.value);
        }
        return Color.white;
    }

    Color getFlangeColor(Color bColor)
    {
        switch(flangeMode)
        {
            case FlangeMode.normal:
                return getBodyColor();
            case FlangeMode.same:
                return bColor;
            case FlangeMode.inverse:
                return new Color(1 - bColor.r, 1 - bColor.g, 1 - bColor.b);
            case FlangeMode.different:
                switch(flangeColorMode)
                {
                    case ColorMode.rainbow:
                        return randBrightColor();
                    case ColorMode.monochrome:
                        return flangeColor;
                    case ColorMode.gradient:
                        return flangeGradient.Evaluate(Random.value);
                }
                return Color.white;
        }
        return Color.white;
    }

    void makePipe(Color tubeCol, Color flangeCol, Vector3 pipePos, Quaternion pipeRot, Transform parent, float scale,
        int iterations)
    {
        Transform pipeParent = new GameObject().transform;
        pipeParent.parent = parent;
        pipeParent.name = "pipe";
        pipeParent.transform.position = pipePos;
        for (int j = 0; j < iterations; j++)
        {
            GameObject pipe = GameObject.Instantiate(pipeSegment);
            pipe.GetComponent<MeshRenderer>().material = pipeMat;
            pipe.transform.parent = pipeParent;
            pipe.transform.position = pipePos;
            float len = Random.Range(1, pipeLength);
            if (generateColliders)
            {
                CapsuleCollider capsule = pipe.AddComponent<CapsuleCollider>();
                capsule.radius = RADIUS;
                capsule.height = len;
                capsule.center = new Vector3(capsule.center.x, len / 2, capsule.center.z);
            }
            pipe.GetComponent<MeshFilter>().mesh = makePipe(len, tubeCol, flangeCol);
            pipe.transform.rotation = pipeRot;
            //info
            PipeInfo info = new PipeInfo();
            info.pipeID = pipeIDCounter;
            info.flangeColor = flangeCol;
            info.bodyColor = tubeCol;
            info.scale = scale;
            info.length = len;
            pipe.AddComponent<PipeInfoHolder>().info = info;
            //don't generate a joint on the last segment
            if (j != iterations - 1)
            {
                //generate joint
                pipePos = pipePos + pipeRot * transform.up * (len + JOINTSIZE);
                pipeRot = pipeRot * Quaternion.Euler(90, 0, Random.Range(0, 360));
                GameObject joint = GameObject.Instantiate(pipeSegment);
                joint.GetComponent<MeshRenderer>().material = pipeMat;
                joint.transform.parent = pipe.transform;
                joint.transform.position = pipePos + pipeRot * new Vector3(0, JOINTSIZE, JOINTSIZE);
                joint.GetComponent<MeshFilter>().mesh = makeJoint(tubeCol, tubeCol);
                joint.transform.rotation = pipeRot * Quaternion.Euler(180, 90, 0);
                pipePos = pipePos + pipeRot * transform.up * JOINTSIZE;
                if (generateColliders)
                {
                    MeshCollider coll = joint.AddComponent<MeshCollider>();
                    coll.sharedMesh = joint.GetComponent<MeshFilter>().mesh;
                    coll.convex = true;
                }
                //info
                PipeInfo jInfo = new PipeInfo();
                jInfo.pipeID = pipeIDCounter;
                jInfo.flangeColor = flangeCol;
                jInfo.bodyColor = tubeCol;
                jInfo.scale = scale;
                jInfo.length = JOINTSIZE;
                joint.AddComponent<PipeInfoHolder>().info = jInfo;
            }
        }
        pipeParent.localScale = new Vector3(scale, scale, scale);
        pipeIDCounter++;
    }

    [BitStrap.Button]
    void generatePipes()
    {
        Transform groupParent = new GameObject().transform;
        groupParent.name = "pipe group";
        for (int i = 0; i < numPipes; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-pLimits.x, pLimits.x),
                Random.Range(-pLimits.y, pLimits.y), Random.Range(-pLimits.z, pLimits.z));
            Quaternion rot = Random.rotation;
            Color bodyCol = getBodyColor();
            makePipe(bodyCol, getFlangeColor(bodyCol), pos, rot, groupParent, 1, pipeIterations);
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
