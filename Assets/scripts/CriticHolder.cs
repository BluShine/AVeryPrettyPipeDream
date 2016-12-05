using UnityEngine;
using System.Collections.Generic;

public class CriticHolder : MonoBehaviour {

    public List<GameObject> critics;

    public int nextCritic;

    public bool firstCritic = true;

	// Use this for initialization
	void Start () {
	    foreach(CriticHolder c in FindObjectsOfType<CriticHolder>())
        {
            if(c != this)
            {
                c.spawnCritic();
                DestroyImmediate(this);
                return;
            }
        }
        DontDestroyOnLoad(this);
        spawnCritic();
	}

    public void spawnCritic()
    {
        Grader g = Instantiate(critics[nextCritic]).GetComponent<Grader>();
        if (firstCritic)
        {
            g.Tutorial();
            firstCritic = false;
        }
        int newCrit = nextCritic;
        while(newCrit == nextCritic && critics.Count > 1)
        {
            newCrit = Random.Range(0, critics.Count);
        }
        nextCritic = newCrit;
        GameObject nextText = GameObject.Find("Next Text");
        nextText.GetComponent<TextMesh>().text =
            "Tomorrow's critique:\n" +
            critics[nextCritic].GetComponent<Grader>().bio.Replace("NLN", "\n");
        nextText.GetComponent<MeshRenderer>().enabled = false;
    }
}
