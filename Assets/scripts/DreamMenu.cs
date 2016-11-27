using UnityEngine;
using UnityEngine.SceneManagement;


public class DreamMenu : MonoBehaviour
{
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneManager.LoadScene("Classroom");
        }
    }
}
