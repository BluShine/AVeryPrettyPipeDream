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
}
