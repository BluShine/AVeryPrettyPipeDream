using UnityEngine;
using System.Collections;

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

	// Use this for initialization
	void Start () {
        pillow = transform.Find("pillow").gameObject;
        headboard = transform.Find("headboard").gameObject;
        setColors();
	}
	
	// Update is called once per frame
	void Update () {
        setColors();
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
