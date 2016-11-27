using UnityEngine;
using System.Collections.Generic;

public class PhotoStorage : MonoBehaviour
{
    public List<Texture2D> photos;

    public GameObject photoPrefab;

    [HideInInspector]
    public bool newPhoto = false;

    public void Start()
    {
        photos = new List<Texture2D>();
        newPhoto = false;
        DontDestroyOnLoad(this.gameObject);
    }

    public void Update()
    {
        /*if(newPhoto)
        {
            addPhoto(photos[photos.Count - 1]);
        }*/
    }

    void addPhoto(Texture2D texture)
    {
        GameObject p = GameObject.Instantiate(photoPrefab);
        p.GetComponent<MeshRenderer>().material.mainTexture = texture;
        newPhoto = false;
    }
}
