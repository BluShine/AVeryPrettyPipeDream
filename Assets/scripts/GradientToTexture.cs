using UnityEngine;
using System.IO;

[ExecuteInEditMode]
public class GradientToTexture : MonoBehaviour
{
    public Gradient gradient;
    public Material material;
    bool lateStart = false;

    void Awake()
    {
        saveTexture();
    }

    void Update()
    {
        if(!lateStart)
        {
            lateStart = true;
            saveTexture();
        }
    }

    [BitStrap.Button]
    void saveTexture()
    {
        if (material == null) return;

        material.SetTexture("_ColorRamp", makeTexture());
    }

    [BitStrap.Button]
    void exportTexture()
    {
        Texture2D texture = makeTexture();
        byte[] bytes = texture.EncodeToPNG();

        // save to HD
        string timestamp = System.DateTime.Now.Month + "-" + System.DateTime.Now.Day + "_" +
            System.DateTime.Now.Hour + "-" + System.DateTime.Now.Minute + "-" + System.DateTime.Now.Second;
        File.WriteAllBytes(Application.dataPath + "/../screenshots/photo-" + timestamp + ".png", bytes);
    }

    public Texture2D makeTexture()
    {
        Texture2D texture = new Texture2D(128, 1, TextureFormat.RGBAFloat, false);//default RGBA32 creates some weird artifacts when used as a fog gradient
        Color[] colors = new Color[128];
        for(int i = 0; i < 128; i++)
        {
            colors[i] = gradient.Evaluate((float)i / 128f);
        }
        texture.SetPixels(colors);
        texture.wrapMode = TextureWrapMode.Clamp;
        //texture.alphaIsTransparency = true;
        return texture;
    }
}
