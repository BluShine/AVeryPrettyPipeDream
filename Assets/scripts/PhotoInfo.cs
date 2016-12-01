using UnityEngine;
using System.Collections.Generic;

public class PhotoInfo
{
    public PipeInfo[] pipeDetection;

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
}
