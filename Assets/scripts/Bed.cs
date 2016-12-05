using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Bed : MonoBehaviour {

    public enum World : int
    {
        ice, fire, earth, water, metal, rainbow, pvc
    }

    public enum Weather : int
    {
        sunny, overcast, foggy, dawn, dusk, ocean, orangecrate, milkyway, dank, dusty
    }

    public World world = World.ice;
    public Weather weather = Weather.sunny;

    public BedColorStorage colors;

    GameObject pillow;
    GameObject headboard;

    float finalY;
    static float RISEAMOUNT = 1.7f;
    float riseTime = 3;
    static float RISESPEED = 2;

    bool started = false;

	// Use this for initialization
	void Start () {
        //randomize bed
        world = (World)Random.Range(0, 7);
        weather = (Weather)Random.Range(0, 5);
        //set colors
        pillow = transform.Find("pillow").gameObject;
        headboard = transform.Find("headboard").gameObject;
        setColors();
        finalY = transform.position.y;
        transform.position = transform.position + new Vector3(0, -RISEAMOUNT, 0);
        riseTime = RISESPEED;
        FindObjectOfType<Grader>().beds.Add(this);
	}
	
	// Update is called once per frame
	void Update () {
        if(!started)
        {
            started = true;
            gameObject.SetActive(false);
            return;
        }
        //setColors();
        if(riseTime > 0)
        {
            transform.position = new Vector3(transform.position.x, 
                finalY - RISEAMOUNT * (riseTime / RISESPEED), transform.position.z);
        } else
        {
            riseTime = 0;
            transform.position = new Vector3(transform.position.x, finalY, transform.position.z);
        }
        riseTime -= Time.deltaTime;
	}

    public void Dream()
    {
        //throw out the old roll of film
        Destroy(FindObjectOfType<PhotoStorage>().gameObject);
        //Debug.Log("Start dream " + weather.ToString() + " " + world.ToString());
        DontDestroyOnLoad(Instantiate(colors.weathers[(int)weather]));
        SceneManager.LoadScene(colors.worlds[(int)world]);
    }

    void setColors()
    {
        //set diffuse colors
        GetComponent<MeshColorer>().setColor(colors.worldDiff[(int)world]);
        headboard.GetComponent<MeshColorer>().setColor(colors.worldDiff[(int)world]);
        pillow.GetComponent<MeshColorer>().setColor(colors.weatherDiff[(int)weather]);
        //set shader colors
        Material mat = GetComponent<MeshRenderer>().sharedMaterial;
        mat.SetColor("_RimColor", colors.weatherRim[(int)weather]);
        mat.SetColor("_SpecColor", colors.weatherSpec[(int)weather]);
    }
}
