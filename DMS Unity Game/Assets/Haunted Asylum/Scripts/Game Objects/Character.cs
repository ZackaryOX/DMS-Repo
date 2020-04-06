using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Character : MonoBehaviour
{
    Player ThisPlayer;
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

    public bool IsClone = false;

    bool _Pmove = false;
    bool _localStamina = false;
    public const float _Tile = 0;
    public const float _Stairs = 1;

    public const float _Walk = 0;
    public const float _Run = 1;

    float walkspeed = 0.5f;

    private bool _heartSound = true;

    private bool playVideo = true;



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
    public GameObject interact;
    public GameObject youlost;
    public GameObject youwon;


    [FMODUnity.EventRef]
    public string[] SFXEventNames;


    [FMODUnity.EventRef]
    public string[] MusicEventNames;

    FMOD.Studio.EventInstance _Heartbeat;

    void Awake()
    {
        InvokeRepeating("playFootsteps", 0, walkspeed);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 144;
        PauseMenu = new PausedState();
        UIElements.SetActive(false);
        PlayerAwake();
        ThisAudioManager = new AudioManager(SFXEventNames, MusicEventNames, head);
        Debug.Log("CHARACTER CREATED");

    }

    void Update()
    {
        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            walkspeed = 0.2f;

        }
        else
        {
            walkspeed = 0.5f;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {

            _Pmove = true;

        }
        else
        {
            _Pmove = false;

            // _Footsteps.setVolume(0);
        }
        

        if (ThisPlayer.GetStamina() <= 0.5)
        {
            StartCoroutine(PlayStamina(ThisAudioManager.SFXEventNames[2]));
        }

        if (ThisPlayer._pickup == true)
        {
            //Debug.Log("Pickup Sound");
            FMODUnity.RuntimeManager.PlayOneShot(ThisAudioManager.SFXEventNames[4], GetComponent<Transform>().position);
            ThisPlayer._pickup = false;
        }

        if (ThisPlayer.GetSanity() < 50)
        {
            if (_heartSound == true)
            {
                _Heartbeat = FMODUnity.RuntimeManager.CreateInstance(ThisAudioManager.SFXEventNames[3]);
                _Heartbeat.start();
                
                Debug.Log("Heartbeat");
                _heartSound = false;
            }
            _Heartbeat.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.gameObject));

        }


        //else if(ThisPlayer.GetSanity() > 50)
        //{
        //    if (IsPlaying(_Heartbeat))
        //    {
        //        _Heartbeat.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        //        _Heartbeat.release();
        //        _heartSound = true;
        //    }
        //}


    }
    bool IsPlaying(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }


    IEnumerator PlayStamina(string _eventAudio)
    {
        if (_localStamina == true)
        {
            yield break;
        }
        _localStamina = true;
        FMODUnity.RuntimeManager.PlayOneShot(_eventAudio, GetComponent<Transform>().position);
       // Debug.Log("StaminaSound");
        yield return new WaitForSeconds(3);
        _localStamina = false;
        yield return null;
    }

    void OnAnimatorIK()
    {

        ThisPlayer.PutHandOut();
    }


    void LateUpdate()
    {
        ThisPlayer.SetIsClone(IsClone);
        if (!IsClone)
        {
            if (ThisPlayer.CoroutinesToFire.Count > 0)
            {
                int Amount = ThisPlayer.CoroutinesToFire.Count;
                for (int i = 0; i < Amount; i++)
                {
                    IEnumerator Temp = ThisPlayer.CoroutinesToFire.Dequeue();
                    StartCoroutine(Temp);
                }

            }

            Vector3 Data = Player1Stats.GetData();

            if (!PlayedMusic)
            {
                PlayedMusic = true;
                ThisAudioManager.PlayMusic();
            }
            if (Data.x <= 0 && Time.timeSinceLevelLoad > 10 && !ThisPlayer.Escaped)
            {
                NetworkHandler.GhostWon = true;
                ThisPlayer.SetState(PauseMenu);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                youlost.SetActive(true);
                if (playVideo)
                {
                    youlost.gameObject.GetComponent<VideoPlayer>().Play();
                    playVideo = false;
                }
            }
            else if (ThisPlayer.Escaped)
            {
                ThisPlayer.SetState(PauseMenu);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                youwon.SetActive(true);
                ThisPlayer.Escaped = true;
                if (playVideo)
                {
                    youwon.gameObject.GetComponent<VideoPlayer>().Play();
                    playVideo = false;
                }
            }
            else if (input.GetKeyDown("escape"))
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
            if (ThisPlayer.CanInteract)
                interact.SetActive(true);
            else
                interact.SetActive(false);
            ThisPlayer.CanInteract = false;

            ThisPlayer.Update();
            hotbar.Update();
            input.Update();
            MyCamera.SetActive(true);
            //MyFBOCam.SetActive(true);
            UIElements.SetActive(true);
            StaminaBar.transform.localScale = new Vector3(Data.y / 100, 1, 1);

            if (Input.GetKey(KeyCode.P))
                ThisPlayer.Escaped = true;

            var tempColor = HealthBar.color;
            tempColor.a = (100 - Data.x) / 100;
            HealthBar.color = tempColor;

            tempColor = SanityBar.color;
            tempColor.a = (100 - Data.z) / 100;
            SanityBar.color = tempColor;

            ThisAudioManager.Update(Data.z);

        }
        else if (IsClone)
        {
            ThisPlayer.Update();
            MyCamera.SetActive(false);
        }
    }

    void playFootsteps()
    {
        if (_Pmove == true)
        {
           //   Debug.Log("Footsteps");
            FMODUnity.RuntimeManager.PlayOneShot(ThisAudioManager.SFXEventNames[1], GetComponent<Transform>().position);
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
    public Player GetPlayerObj()
    {
        return ThisPlayer;
    }
    
    void PlayerAwake()
    {
        input = new InputManager();
        hotbar = new PlayerInventory(defaultIcon, selectedIcon, emptyItem, SlotNumber, HandTarget);
        MyCamera.SetActive(false);
        //MyFBOCam.SetActive(false);
        ThisPlayer = new Player(gameObject, head, hotbar, false, input, HandTarget);
        Player1Stats = new StatObserver(ThisPlayer);
        Player1Score = new ScoreObserver(ThisPlayer);
        /*FOR TUTORIAL:*/ //Player1.SetState(new TeachWalkState());
        /*FOR EDITING:*/
        ThisPlayer.SetState(new TeachPickupState());
    }
    void UpdatePlayer(float Sanity, float health)
    {
        ThisPlayer.SetSanity(Sanity);
        ThisPlayer.SetHealth(health);
    }
}