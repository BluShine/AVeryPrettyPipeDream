using UnityEngine;
using System.Collections.Generic;

public class Photographer : MonoBehaviour
{
    public List<Texture2D> photos;

    public MeshRenderer photoRenderer;

    bool buttonPressed = false;

    public void Start()
    {
        photos = new List<Texture2D>();
    }

    public void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            ScreenShot();
        }
    }

    [BitStrap.Button]
    public void ScreenShot()
    {
        buttonPressed = true;
    }

    void OnPostRender()
    {
        if(buttonPressed)
        {
            buttonPressed = false;
            Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBAFloat, false);
            screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenshot.Apply();
            screenshot.alphaIsTransparency = false;
            screenshot.name = "photo" + photos.Count;
            photos.Add(screenshot);
            photoRenderer.material.mainTexture = screenshot;
        }
    }
}
