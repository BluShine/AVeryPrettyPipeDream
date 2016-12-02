using UnityEngine;
using System.Collections.Generic;

public class MeshColorer : MonoBehaviour {

    public Color color;

	// Use this for initialization
	void Start () {
        Mesh m = GetComponent<MeshFilter>().mesh;
        List<Color> colors = new List<Color>();
        for(int i = 0; i < m.vertexCount; i++)
        {
            colors.Add(color);
        }
        m.SetColors(colors);
	}
}
