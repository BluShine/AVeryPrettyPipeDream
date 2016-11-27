using UnityEngine;
using System.Collections.Generic;

public class PhotoStorage : MonoBehaviour
{
    public List<Texture2D> photos;

    public GameObject photoPrefab;

    public void Start()
    {
        photos = new List<Texture2D>();
    }
}
