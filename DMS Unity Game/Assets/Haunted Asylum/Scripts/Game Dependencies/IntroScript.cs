using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroScript : MonoBehaviour
{
    public GameObject Video;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Video.GetComponent<VideoPlayer>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(Video.GetComponent<VideoPlayer>().isPaused)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
    public void Skip()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
