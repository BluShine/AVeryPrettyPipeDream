using UnityEngine;
using System.Collections.Generic;

public class Grader : MonoBehaviour
{
    public string greeting = "Hi.";

    static string wrongNumberOfPhotos = "You need to pin exactly 3 photos on the wall.";

    [HideInInspector]
    public Criteria[] criterias;

    public Transform speechBubble;
    Typewriter writer;

    public float hideAfterEndDelay = 1.5f;
    float delay = 0;
    bool talking = false;

    public void Start()
    {
        writer = GetComponentInChildren<Typewriter>();
        writer.SetText(greeting);

        criterias = GetComponentsInChildren<Criteria>();
    }

    public void Click()
    {
        if (ClassMenu.instance.photosToGrade.Count == 3)
        {
            string totalFeedback = "";
            foreach (Criteria c in criterias)
            {
                string feedback;
                int fav;
                float grade = c.gradePhotos(ClassMenu.instance.photosToGrade, out feedback, out fav);
                totalFeedback += feedback + " ";
            }
            writer.SetText(totalFeedback);
            writer.Stop();
            writer.Write();
        }
        else {
            writer.SetText(wrongNumberOfPhotos);
            writer.Stop();
            writer.Write();
        }
    }

    public void Update()
    {
        if(talking && writer.Completed)
        {
            delay += Time.deltaTime;
            if(delay >= hideAfterEndDelay)
            {
                writer.Stop();
                disableBubble();
                talking = false;
            }
        }
    }

    public void talk()
    {
        if (talking) return;
        delay = 0;
        talking = true;
        enableBubble();
        writer.Write();
    }

    void enableBubble()
    {
        if (speechBubble != null)
        {
            foreach (var t in speechBubble.transform)
            {
                (t as Transform).gameObject.GetComponent<Renderer>().enabled = true;
            }
        }
    }
    void disableBubble()
    {
        if (speechBubble != null)
        {
            foreach (var t in speechBubble.transform)
            {
                (t as Transform).gameObject.GetComponent<Renderer>().enabled = false;
            }
        }
    }
}
