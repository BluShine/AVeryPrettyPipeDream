using UnityEngine;
using System.Collections.Generic;

public class PhotoInfo
{
    public PipeInfo[] pipeDetection;

    public Texture2D texture;

    static int SEARCHRES = 10;
    static float HDIST = .125f;
    static float SDIST = .5f;
    static float VDIST = .5f;

    public PhotoInfo (int rays)
    {
        pipeDetection = new PipeInfo[rays];
    }

    public int countPipes()
    {
        HashSet<int> pipesSpotted = new HashSet<int>();
        for (int i = 0; i < pipeDetection.Length; i++) {
            if (pipeDetection[i] != null && !pipesSpotted.Contains(pipeDetection[i].pipeID))
            {
                pipesSpotted.Add(pipeDetection[i].pipeID);
            }
        }
        return pipesSpotted.Count;
    }

    public float pipeDensity()
    {
        int pCount = 0;
        for (int i = 0; i < pipeDetection.Length; i++)
        {
            if (pipeDetection[i] != null)
            {
                pCount++;
            }
        }
        return (float)pCount / (float)pipeDetection.Length;
    }

    //returns 0 to 1 based on how much of the photo is close to that color
    //works best with bright, saturated colors
    //probably won't work well for very dark or very desaturated colors.
    public float detectColor(Color c) 
    {
        float H;
        float S;
        float V;
        Color.RGBToHSV(c, out H, out S, out V);
        float influence = 1f / (float)(SEARCHRES * SEARCHRES);
        float total = 0;
        for(int i = 0; i < SEARCHRES; i++)
        {
            for(int j = 0; j < SEARCHRES; j++)
            {
                Color n = texture.GetPixelBilinear(((float)i + .5f) / (float)SEARCHRES, ((float)j + .5f) / (float)SEARCHRES);
                float nH;
                float nS;
                float nV;
                Color.RGBToHSV(n, out nH, out nS, out nV);
                float hDiff = Mathf.Min(Mathf.Abs(H - nH), Mathf.Abs((H + 1) - nH));
                float sDiff = Mathf.Abs(S - nS);
                float vDiff = Mathf.Abs(V - nV);
                if(hDiff <= HDIST && sDiff < SDIST && vDiff < VDIST)
                {
                    if (hDiff < HDIST / 2f)
                        total += influence;
                    else
                        total += influence / 2f;
                }
            }
        }
        return total;
    }

    //return average saturation
    public float averageSaturation()
    {
        float influence = 1f / (float)(SEARCHRES * SEARCHRES);
        float total = 0;
        for (int i = 0; i < SEARCHRES; i++)
        {
            for (int j = 0; j < SEARCHRES; j++)
            {
                Color n = texture.GetPixelBilinear(((float)i + .5f) / (float)SEARCHRES, ((float)j + .5f) / (float)SEARCHRES);
                float nH;
                float nS;
                float nV;
                Color.RGBToHSV(n, out nH, out nS, out nV);
                total += influence * nS;
            }
        }
        return total;
    }

    //return average brightness
    public float averageBrightness()
    {
        float influence = 1f / (float)(SEARCHRES * SEARCHRES);
        float total = 0;
        for (int i = 0; i < SEARCHRES; i++)
        {
            for (int j = 0; j < SEARCHRES; j++)
            {
                Color n = texture.GetPixelBilinear(((float)i + .5f) / (float)SEARCHRES, ((float)j + .5f) / (float)SEARCHRES);
                total += influence * n.grayscale;
            }
        }
        return total;
    }

    //detect contrast by adding up all the "light" (>50% colors) and "dark" colors, and returning the minimum of those two.
    //all gray = 0, all white = 0, half gray half white = 0, half black half white = 1
    public float contrast()
    {
        float darkness = 0;
        float lightness = 0;
        float influence = 1f / (float)(SEARCHRES * SEARCHRES);
        for (int i = 0; i < SEARCHRES; i++)
        {
            for (int j = 0; j < SEARCHRES; j++)
            {
                Color n = texture.GetPixelBilinear(((float)i + .5f) / (float)SEARCHRES, ((float)j + .5f) / (float)SEARCHRES);
                if (n.grayscale < .5f)
                {
                    darkness += influence * n.grayscale * 2;
                } else
                {
                    lightness += influence * (n.grayscale - .5f) * 2;
                }
            }
        }
        return Mathf.Min(darkness, lightness) * 2;
    }

    //detect noise in an area at the center of the screen
    public float centerNoise(float area)
    {
        float influence = 1f / (float)(SEARCHRES * SEARCHRES);
        float offset = 1f - area / 2f;
        float contrastDistance = .1f;
        float total = 0;
        for (int i = 0; i < SEARCHRES; i++)
        {
            for (int j = 0; j < SEARCHRES; j++)
            {
                float x = ((float)i + .5f) / (float)SEARCHRES;
                x = offset + x * area;
                float y = ((float)j + .5f) / (float)SEARCHRES;
                y = offset + x * area;
                Color n = texture.GetPixelBilinear(x, y);
                Color up = texture.GetPixelBilinear(x, y + contrastDistance);
                Color down = texture.GetPixelBilinear(x, y - contrastDistance);
                Color left = texture.GetPixelBilinear(x + contrastDistance, y);
                Color right = texture.GetPixelBilinear(x - contrastDistance, y);
                total += influence * Mathf.Max(rgbDiff(n, up), rgbDiff(n, down), rgbDiff(n, left), rgbDiff(n, right));
            }
        }
        return total;
    }

    //detect noise in an area at the center of the screen
    public float edgeNoise(float area)
    {
        float influence = 1f / (float)(SEARCHRES * SEARCHRES);
        float offset = area + 1f - area / 2f;
        float contrastDistance = .1f;
        float total = 0;
        int HALFRES = SEARCHRES / 2;
        for (int i = 0; i < SEARCHRES; i++)
        {
            for (int j = 0; j < SEARCHRES; j++)
            {
                float x = ((float)i + .5f) / (float)SEARCHRES;
                if (i > HALFRES)
                {
                    x += offset;
                }
                float y = ((float)j + .5f) / (float)SEARCHRES;
                if (y > HALFRES) {
                    y += offset;
                }
                Color n = texture.GetPixelBilinear(x, y);
                Color up = texture.GetPixelBilinear(x, y + contrastDistance);
                Color down = texture.GetPixelBilinear(x, y - contrastDistance);
                Color left = texture.GetPixelBilinear(x + contrastDistance, y);
                Color right = texture.GetPixelBilinear(x - contrastDistance, y);
                total += influence * Mathf.Max(rgbDiff(n, up), rgbDiff(n, down), rgbDiff(n, left), rgbDiff(n, right));
            }
        }
        return total;
    }

    private float rgbDiff(Color a, Color b)
    {
        return Mathf.Max(Mathf.Abs(a.r - b.r), Mathf.Abs(a.g - b.g), Mathf.Abs(a.b - b.b));
    }
}
