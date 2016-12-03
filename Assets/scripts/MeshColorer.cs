using UnityEngine;
using System.Collections.Generic;

public class MeshColorer : MonoBehaviour {

    public Color color;

	// Use this for initialization
	void Start () {
        setColor(color);
	}

    public void setColor(Color c)
    {
        color = c;
        Mesh m = GetComponent<MeshFilter>().mesh;
        List<Color> colors = new List<Color>();
        for (int i = 0; i < m.vertexCount; i++)
        {
            colors.Add(c);
        }
        m.SetColors(colors);
    }
}
