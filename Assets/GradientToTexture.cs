using UnityEngine;

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
        material.SetTexture("_ColorRamp", makeTexture());
    }

    Texture2D makeTexture()
    {
        Texture2D texture = new Texture2D(128, 1);
        Color[] colors = new Color[128];
        for(int i = 0; i < 128; i++)
        {
            colors[i] = gradient.Evaluate((float)i / 128f);
        }
        texture.SetPixels(colors);
        return texture;
    }
}
