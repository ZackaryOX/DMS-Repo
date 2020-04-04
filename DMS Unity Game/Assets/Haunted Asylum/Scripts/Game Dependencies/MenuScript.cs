using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public GameObject Loading;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Loading.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Skip()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void Play()
    {
        Loading.SetActive(true);
        SceneManager.LoadScene("HauntedAsylum");
    }
    public void Exit()
    {
        Application.Quit();
    }
}
