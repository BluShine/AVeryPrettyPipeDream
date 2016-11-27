using UnityEngine;
using System.Collections;

public class Photograph : MonoBehaviour {

    Vector3 normalScale;
    static float biggerScale = 3;

    public void Start()
    {
        normalScale = transform.localScale;
    }

	public void grow()
    {
        transform.localScale = normalScale * biggerScale;
    }

    public void shrink()
    {
        transform.localScale = normalScale;
    }
}
