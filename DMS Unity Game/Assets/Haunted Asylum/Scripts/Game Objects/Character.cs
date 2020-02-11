using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class Character : MonoBehaviour
{
    Player ThisPlayer;
    private PhotonView PV;
    public GameObject head;
    public GameObject MyCamera;
    public GameObject MyFBOCam;
    public Image defaultIcon;
    public Image selectedIcon;
    public Image emptyItem;
    public Text SlotNumber;
    public Image StaminaBar;
    public Image HealthBar;
    public Image SanityBar;
    PlayerInventory hotbar;
    StatObserver Player1Stats;
    ScoreObserver Player1Score;

    private InputManager input;
    private PausedState PauseMenu;
    private PlayerState OldState;
    public AudioManager ThisAudioManager;
    private bool PlayedMusic = false;
    private bool DeletedDoors = false;
    public GameObject HandTarget;
    public GameObject menu;
    public GameObject settings;
    public GameObject audiosettings;
    public GameObject keybinds;
    public GameObject confirmation;
    public GameObject UIElements;

    [FMODUnity.EventRef]
    public string[] SFXEventNames;

    [FMODUnity.EventRef]
    public string[] MusicEventNames;

    void Awake()
    {
        PauseMenu = new PausedState();
        PV = GetComponent<PhotonView>();
        UIElements.SetActive(false);
        PlayerAwake();
        //PV.RPC("PlayerAwake", RpcTarget.AllBuffered);
        Application.targetFrameRate = 144;
        ThisAudioManager = new AudioManager(SFXEventNames, MusicEventNames, head);
        Debug.Log("CHARACTER CREATED");
    }

    void Update()
    {
               
    }

    void OnAnimatorIK()
    {
        if (PV.IsMine)
        {
            ThisPlayer.PutHandOut();
        }
    }


    void LateUpdate()
    {
        if (PV.IsMine)
        {
           
            if (!PlayedMusic)
            {
                PlayedMusic = true;
                ThisAudioManager.PlayMusic();
            }
            if (input.GetKeyDown("escape"))
            {
                if (OldState == null)
                {
                    OldState = ThisPlayer.GetState();
                    ThisPlayer.SetState(PauseMenu);
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    menu.SetActive(true);
                }
                else
                {
                    Resume();
                }
            }

            ThisPlayer.Update();
            hotbar.Update();
            input.Update();
            MyCamera.SetActive(true);
            MyFBOCam.SetActive(true);
            UIElements.SetActive(true);
            Vector3 Data = Player1Stats.GetData();
            StaminaBar.transform.localScale = new Vector3(Data.y / 100, 1, 1);
            HealthBar.transform.localScale = new Vector3(Data.x / 100, 1, 1);
            SanityBar.transform.localScale = new Vector3(Data.z / 100, 1, 1);
            ThisAudioManager.Update(Data.z);
        }
        
    }

    public void SetSFXVolume(float temp)
    {
        ThisAudioManager.SetSFXVolume(temp);
    }
    public void SetMusicVolume(float temp)
    {
        ThisAudioManager.SetMusicVolume(temp);
    }
    public void SetMasterVolume(float temp)
    {
        ThisAudioManager.SetMasterVolume(temp);
    }
    public void RebindKey(string KeyName)
    {
        input.RebindKey(KeyName);
    }
    public void StoreText(Text KeyText)
    {
        input.StoreText(KeyText);
    }
    public void ResetKeys()
    {
        input.ResetKeys();
    }
    public void Resume()
    {
        menu.SetActive(false);
        settings.SetActive(false);
        audiosettings.SetActive(false);
        keybinds.SetActive(false);
        confirmation.SetActive(false);
        ThisPlayer.SetState(OldState);
        OldState = null;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void Settings()
    {
        settings.SetActive(true);
        menu.SetActive(false);
        audiosettings.SetActive(false);
        keybinds.SetActive(false);
    }
    public void Audio()
    {
        audiosettings.SetActive(true);
        settings.SetActive(false);
    }
    public void Keybinds()
    {
        keybinds.SetActive(true);
        settings.SetActive(false);
    }
    public void Exit()
    {
        menu.SetActive(false);
        confirmation.SetActive(true);
    }
    public void Menu()
    {
        menu.SetActive(true);
        settings.SetActive(false);
        audiosettings.SetActive(false);
        keybinds.SetActive(false);
        confirmation.SetActive(false);
    }
    public void Leave()
    {
        SceneManager.LoadScene("MainMenu");
    }



    [PunRPC]
    void PlayerAwake()
    {
        input = new InputManager();
        hotbar = new PlayerInventory(defaultIcon, selectedIcon, emptyItem, SlotNumber);
        MyCamera.SetActive(false);
        MyFBOCam.SetActive(false);
        ThisPlayer = new Player(gameObject, head, hotbar, false, input, HandTarget);
        Player1Stats = new StatObserver(ThisPlayer);
        Player1Score = new ScoreObserver(ThisPlayer);
       /*FOR TUTORIAL:*/ //Player1.SetState(new TeachWalkState());
       /*FOR EDITING:*/  ThisPlayer.SetState(new TeachPickupState());
    }
    [PunRPC]
    void UpdatePlayer(float Sanity, float health)
    {
        ThisPlayer.SetSanity(Sanity);
        ThisPlayer.SetHealth(health);
    }
}