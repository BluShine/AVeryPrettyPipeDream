using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class DreamMenu : MonoBehaviour
{
    PhotoStorage storage;

    public Text hintText;

    [HideInInspector]
    public float textFade = 0;
    static float TTIMER = 5;

    [HideInInspector]
    public float hintTimer = 0;
    static float HINTWAIT = 10;

    GlobalFog fog;
    float fogDefault;
    bool fogTween = false;
    static float FOGSPEED = 5f;

    public Transform titleScreen;

    public void Start()
    {
        Weather weather = FindObjectOfType<Weather>();
        if(weather != null)
        {
            //set fog gradient
            GlobalFog f = FindObjectOfType<GlobalFog>();
            f.fogGradient = weather.transform.FindChild("FogGradient").GetComponent<GradientToTexture>();
            f.applyGradient();
            
        }
        storage = FindObjectOfType<PhotoStorage>();
        fog = FindObjectOfType<GlobalFog>();
        fogDefault = fog.endDistance;
        fog.endDistance = 0;
    }

    public void Update()
    {
        if(Input.GetButtonDown("Submit"))
        {
            if (storage.photos.Count >= 3)
            {
                Weather weather = FindObjectOfType<Weather>();
                if(weather != null)
                {
                    Destroy(weather.gameObject);
                }
                SceneManager.LoadScene("Classroom");
            } else {
                hintTimer = 0;
                textFade = TTIMER;
                hintText.text = "Shoot at least 3 photos.";
                hintText.color = Color.white;
            }
        } else if (hintTimer > HINTWAIT && storage.photos.Count >= 3)
        {
            hintTimer = 0;
            textFade = TTIMER;
            hintText.text = "[ENTER] to grade photos.";
            hintText.color = Color.white;
        }

        if(Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Hover") != 0 || Input.GetButtonDown("Fire1"))
        {
            fogTween = true;
        }

        if (fogTween)
        {
            hintTimer += Time.deltaTime;
            if(fog.endDistance != fogDefault)
            {
                fog.endDistance = Mathf.Min(fogDefault, fog.endDistance + Time.deltaTime * FOGSPEED);
                if (titleScreen != null)
                {
                    foreach (TextMesh t in titleScreen.GetComponentsInChildren<TextMesh>())
                    {
                        t.color = new Color(1, 1, 1, 1 - fog.endDistance / fogDefault);
                    }
                    foreach (SpriteRenderer s in titleScreen.GetComponentsInChildren<SpriteRenderer>())
                    {
                        s.color = new Color(1, 1, 1, 1 - fog.endDistance / fogDefault);
                    }
                }
            } else
            {
                if (titleScreen != null)
                {
                    titleScreen.gameObject.SetActive(false);
                }
            }
        }

        if(textFade > 0)
        {
            hintText.enabled = true;
            if(textFade < 1)
            {
                hintText.color = new Color(1, 1, 1, textFade);
            }
            textFade -= Time.deltaTime;
        } else
        {
            hintText.enabled = false;
        }
    }
}
