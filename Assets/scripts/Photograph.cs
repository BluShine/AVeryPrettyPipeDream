using UnityEngine;
using System.Collections;

public class Photograph : MonoBehaviour {

    Vector3 normalScale;
    static float biggerScale = 3;

    public PhotoInfo info;

    public void Start()
    {
        normalScale = transform.localScale;
    }

	public void grow()
    {
        transform.localScale = normalScale * biggerScale;
        Debug.Log("pipes: " + info.countPipes() + " density: " + info.pipeDensity() + " redness " + info.detectColor(Color.red));
    }

    public void shrink()
    {
        transform.localScale = normalScale;
    }
}
