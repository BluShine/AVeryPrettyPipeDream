using UnityEngine;
using System.Collections.Generic;

public class Criteria : MonoBehaviour
{
    public string gradeName = "Redness";

    public CritType type = CritType.color;
    public bool invertCrit = false; // if true, flip the result
    public float threashold = .5f;
    public Color color = Color.red;
    public float amount = 50;

    public string positiveFeedback = "Very red. I like it. ";
    public string negativeFeedback = "Try using more red. ";

    public enum CritType
    {
        color, density, count, saturation, brightness, contrast, centerNoise, edgeNoise
    }

    public float gradePhotos(List<Photograph> photos, out string feedback, out int favorite, out bool good)
    {
        float result = 0;
        favorite = 0;
        for (int i = 0; i < photos.Count; i++)
        {
            float r = 0;
            switch (type)
            {
                case CritType.color:
                    r = photos[i].info.detectColor(color);
                    break;
                case CritType.density:
                    r = photos[i].info.pipeDensity();
                    break;
                case CritType.count:
                    r = (float)photos[i].info.countPipes() / amount;
                    break;
                case CritType.saturation:
                    r = photos[i].info.averageSaturation();
                    break;
                case CritType.brightness:
                    r = photos[i].info.averageBrightness();
                    break;
                case CritType.contrast:
                    r = photos[i].info.contrast();
                    break;
                case CritType.edgeNoise:
                    r = photos[i].info.edgeNoise(amount);
                    break;
                case CritType.centerNoise:
                    r = photos[i].info.centerNoise(amount);
                    break;
            }
            if(r > result)
            {
                result = r;
                favorite = i;
            }
        }
        if(invertCrit)
        {
            result = 1 - result;
        }
        if(result > threashold)
        {
            feedback = positiveFeedback;
            good = true;
        } else
        {
            feedback = negativeFeedback;
            good = false;
        }
        
        return result;
    }
}
