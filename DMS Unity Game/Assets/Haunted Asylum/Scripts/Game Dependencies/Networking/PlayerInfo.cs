using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo PI;
    public static int PlayerNumber = 0;
    public int mynumber;
    public int mySelectedCharacter;
    public GameObject[] AllCharacters;
    // Start is called before the first frame update
    void OnEnable()
    {
        if(PlayerInfo.PI == null)
        {
            PlayerInfo.PI = this;
        }
        else
        {
            if(PlayerInfo.PI != this)
            {
                Destroy(PlayerInfo.PI.gameObject);
                PlayerInfo.PI = this;
                PlayerNumber++;
            }
        }
        mynumber = PlayerNumber;

        //DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(this.gameObject.transform.parent);
    }


    void Start()
    {
        if (PlayerPrefs.HasKey("MyCharacter"))
        {
            mySelectedCharacter = PlayerPrefs.GetInt("MyCharacter");
        }
        else
        {
            mySelectedCharacter = 0;
            mySelectedCharacter = PlayerPrefs.GetInt("mySelectedCharacter");

            //TO SET PLAYERPREFS PlayerPrefs.SetInt("MyCharacter", 2); 
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
