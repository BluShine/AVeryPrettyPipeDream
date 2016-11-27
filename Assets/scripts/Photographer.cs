using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Photographer : MonoBehaviour
{
    public List<Texture2D> photos;

    public MeshRenderer photoRenderer;

    bool buttonPressed = false;

    IEnumerator Start()
    {
        photos = new List<Texture2D>();
        ScreenShot();
        yield return null;
    }

    public void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(ScreenShot());
        }
    }

    IEnumerator ScreenShot()
    {
        Debug.Log("ready...");
        yield return new WaitForEndOfFrame();
        Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBAFloat, false);
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        /*screenshot.Apply();
        screenshot.alphaIsTransparency = false;
        screenshot.name = "photo" + photos.Count;
        photos.Add(screenshot);*/
        //photoRenderer.material.mainTexture = screenshot;
        Debug.Log("click!");
    }
}
