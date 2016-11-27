using UnityEngine;

public class ClassMenu : MonoBehaviour
{
    public Texture2D pinTexture;
    public Texture2D rectTexture;

    public float holdDistance = .3f;

    public float photoWidth = 200;
    public float photoHeight = 200;

    bool pinning = false;
    bool mouseOver = false;

    Photograph grabbedPhoto;

    void Update()
    {
        mouseOver = false;
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit rayHit = new RaycastHit();
        if (Physics.Raycast(ray, out rayHit))
        {
            Debug.DrawLine(rayHit.point, transform.position);
            Photograph photo = rayHit.collider.gameObject.GetComponent<Photograph>();
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
                    }
                    grabbedPhoto = photo;
                    grabbedPhoto.transform.position = new Vector3(0, -10, 0);
                }
            } else if(grabbedPhoto != null && Input.GetButtonDown("Fire1"))
            {
                grabbedPhoto.transform.position = rayHit.point + rayHit.normal * .05f;
                grabbedPhoto.transform.rotation = Quaternion.LookRotation(-rayHit.normal, Vector3.up);
                if (rayHit.normal.y < .5f)
                {
                    grabbedPhoto.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    grabbedPhoto.grow();
                } else
                {
                    grabbedPhoto.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    grabbedPhoto.shrink();
                }
                grabbedPhoto = null;
            }
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
        }
    }

    void drawCrosshair(Texture2D crosshairTexture, float width, float height)
    {
        GUI.DrawTexture(new Rect((Screen.width - width) / 2,
            (Screen.height - height) / 2,
            width, height), crosshairTexture);
    }
}
