using UnityEngine;
using System.Collections;
using System.IO;

//taken from https://github.com/martinysa/Unity/blob/master/Libraries/ScreenShotMethods/shotMethod3.cs
//(MIT liscense)
//--Method 3 -- Using readPixel -- UNITY3D PRO ONLY --//

public class ScreenShotter : MonoBehaviour
{

    public int width = Screen.width;
    public int height = Screen.height;

    public PhotoStorage storage;

    public void Start()
    {
        width = Screen.width;
        height = Screen.height;
    }

    void LateUpdate()
    {
        if (Input.GetButtonDown("Fire1"))
            StartCoroutine(ScreenshotEncode());
    }

    IEnumerator ScreenshotEncode()
    {   // Call a coroutine let me know when all objects
        // have finished being rendered on screen 


        // -- that's not a another thread -- !!

        // wait for graphics to render
        yield return new WaitForEndOfFrame();

        // create a texture to pass to encoding
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);

        // put buffer into texture

        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture.Apply();

        // split the process up--ReadPixels() and the GetPixels()
        // call inside of the encoder are both pretty heavy
        yield return 0;

        storage.photos.Add(texture);

        byte[] bytes = texture.EncodeToPNG();

        // save to HD
        string timestamp = System.DateTime.Now.Month + "-" + System.DateTime.Now.Day + "_" + 
            System.DateTime.Now.Hour + "-" + System.DateTime.Now.Minute + "-" + System.DateTime.Now.Second;
        File.WriteAllBytes(Application.dataPath + "/../screenshots/photo-" + timestamp + ".png", bytes);


        //Release memory 
        //DestroyObject(texture); just kidding
        storage.newPhoto = true;
    }
}