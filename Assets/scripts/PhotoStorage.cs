using UnityEngine;
using System.Collections.Generic;

public class PhotoStorage : MonoBehaviour
{
    public List<Texture2D> photos;

    public List<PhotoInfo> infos;

    public void Start()
    {
        if (photos == null)
        {
            photos = new List<Texture2D>();
        }
        if (infos == null)
        {
            infos = new List<PhotoInfo>();
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void transferTextures()
    {
        for (int i = 0; i < infos.Count; i++)
        {
            infos[i].texture = photos[i];
        }
    }

    public void Update()
    {
        /*if(newPhoto)
        {
            addPhoto(photos[photos.Count - 1]);
        }*/
    }
}
