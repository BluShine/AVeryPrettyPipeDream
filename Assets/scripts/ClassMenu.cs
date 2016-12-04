using UnityEngine;
using System.Collections.Generic;

public class ClassMenu : MonoBehaviour
{
    public static ClassMenu instance;

    public Texture2D pinTexture;
    public Texture2D rectTexture;

    public float holdDistance = .3f;

    public float photoWidth = 200;
    public float photoHeight = 200;

    public GameObject tutorialText;

    bool pinning = false;
    bool mouseOver = false;

    float cursorFadeTimer = 0;

    Photograph grabbedPhoto;

    [HideInInspector]
    public List<Photograph> photosToGrade;

    void Start()
    {
        instance = this;
        photosToGrade = new List<Photograph>();
    }

    void Update()
    {
        mouseOver = false;
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit rayHit = new RaycastHit();
        if (Physics.Raycast(ray, out rayHit))
        {
            Debug.DrawLine(rayHit.point, transform.position);
            Photograph photo = rayHit.collider.gameObject.GetComponent<Photograph>();
            Grader grader = rayHit.collider.gameObject.GetComponentInParent<Grader>();
            Bed bed = rayHit.collider.gameObject.GetComponent<Bed>();
            if(bed == null)
            {
                bed = rayHit.collider.gameObject.GetComponentInParent<Bed>();
            }
            if (photo != null)
            {
                mouseOver = true;
                if (Input.GetButtonDown("Fire1"))
                {
                    if(grabbedPhoto != null)
                    {
                        grabbedPhoto.transform.position = photo.transform.position;
                        grabbedPhoto.transform.rotation = photo.transform.rotation;
                        grabbedPhoto.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                        grabbedPhoto.shrink();
                        photosToGrade.Remove(grabbedPhoto);
                    }
                    grabbedPhoto = photo;
                    grabbedPhoto.transform.position = new Vector3(0, -10, 0);
                }
                photosToGrade.Remove(grabbedPhoto);
            } else if (grader != null)
            {
                if(Input.GetButtonDown("Fire1"))
                {
                    grader.Click();
                }
                grader.talk();
            } else if (bed != null && Input.GetButtonDown("Fire1"))
            {
                bed.Dream();
            }
            else if(grabbedPhoto != null && Input.GetButtonDown("Fire1"))
            {
                grabbedPhoto.transform.position = rayHit.point + rayHit.normal * .05f;
                grabbedPhoto.transform.rotation = Quaternion.LookRotation(-rayHit.normal, Vector3.up);
                if (rayHit.normal.y < .5f)
                {
                    grabbedPhoto.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    grabbedPhoto.grow();
                    if (rayHit.transform.name == "Grading Wall")
                    {
                        photosToGrade.Add(grabbedPhoto);
                    }
                } else
                {
                    grabbedPhoto.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    grabbedPhoto.shrink();
                    photosToGrade.Remove(grabbedPhoto);
                }

                grabbedPhoto = null;
            }
        }
        if(photosToGrade.Count == 3)
        {
            tutorialText.SetActive(false);
        } else
        {
            tutorialText.SetActive(true);
        }
    }

    void OnGUI()
    {
        if (grabbedPhoto != null)
        {
            //draw white border
            GUI.DrawTexture(new Rect((Screen.width - photoWidth - 16) / 2,
            (Screen.height - photoHeight - 16) / 2 + photoHeight / 2 + 32,
            photoWidth + 16, photoHeight + 16),
            Texture2D.whiteTexture);
            //draw photo
            GUI.DrawTexture(new Rect((Screen.width - photoWidth) / 2,
            (Screen.height - photoHeight) / 2 + photoHeight / 2 + 32,
            photoWidth, photoHeight),
            grabbedPhoto.GetComponent<MeshRenderer>().material.mainTexture as Texture2D);
            drawCrosshair(pinTexture, 32, 32);
        }
        else if (mouseOver)
        {
            drawCrosshair(rectTexture, 32, 32);
        } else 
        {
            if(GetComponent<Rigidbody>().velocity.magnitude > 0  || 
                Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                cursorFadeTimer = 3;
            }
            if (cursorFadeTimer > 0) {
                drawCrosshair(rectTexture, 32, 32);
            }
        }
        cursorFadeTimer = Mathf.Max(0, cursorFadeTimer - Time.deltaTime);
    }

    void drawCrosshair(Texture2D crosshairTexture, float width, float height)
    {
        GUI.DrawTexture(new Rect((Screen.width - width) / 2,
            (Screen.height - height) / 2,
            width, height), crosshairTexture);
    }
}
