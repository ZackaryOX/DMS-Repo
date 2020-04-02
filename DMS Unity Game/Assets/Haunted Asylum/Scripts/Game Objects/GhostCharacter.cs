using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GhostCharacter : MonoBehaviour
{
    Ghost ThisPlayer;
    public GameObject head;
    public GameObject Hand;
    public GameObject MyCamera;
    public GameObject MyFBOCam;
    public Image defaultIcon;
    public Image selectedIcon;
    public Image emptyItem;
    public Text SlotNumber;
    public Image StaminaBar;
    public Image HealthBar;
    public GameObject Surfaces;
    public GameObject Sheet;
    GhostInventory hotbar;
    GhostStatObserver Player1Stats;
    GhostScoreObserver Player1Score;
    

    public Sprite CrawlerIcon;
    public bool IsClone = false;

    private InputManager input;
    private PausedState PauseMenu;
    private PlayerState OldState;
    public AudioManager ThisAudioManager;

    public GameObject menu;
    public GameObject settings;
    public GameObject audiosettings;
    public GameObject keybinds;
    public GameObject confirmation;
    public GameObject UIElements;
    private bool DoorsDeleted = false;
    private NetworkWrapper ThisWrapper;

    public Materialise TestAbility;

    [FMODUnity.EventRef]
    public string[] SFXEventNames;

    [FMODUnity.EventRef]
    public string[] MusicEventNames;

    [FMODUnity.EventRef]
    public string AbilityAudio;
    FMOD.Studio.EventInstance AbilityInstance;
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 144;
        PauseMenu = new PausedState();
        UIElements.SetActive(false);
        PlayerAwake();

        ThisAudioManager = new AudioManager(SFXEventNames, MusicEventNames, head);




        ThisPlayer.AddRenderer(Surfaces.GetComponent<SkinnedMeshRenderer>());
        ThisPlayer.AddRenderer(Sheet.GetComponent<SkinnedMeshRenderer>());
        ThisPlayer.SetTransparency(0.2f);


    }
    private void Start()
    {
        AbilityInstance = FMODUnity.RuntimeManager.CreateInstance(AbilityAudio);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(AbilityInstance, head.GetComponent<Transform>(), head.GetComponent<Rigidbody>());
        ThisWrapper = GameObject.Find("NetworkManager").GetComponent<NetworkWrapper>();
    }
    void Update()
    {

    }

    void LateUpdate()
    {
        ThisPlayer.SetIsClone(IsClone);
        if (Player.AllPlayers.Count == 0)
        {
            Debug.Log("waiting for players");
        }
        else if (TestAbility == null && Player.AllPlayers.Count > 0)
        {
            float LookDamage = 7.5f;
            float AOEDamage = 7.5f;
            float AOERadius = 10f;
            float Cooldown = 10.0f;
            float ActiveTime = 5.0f;

            TestAbility = new Materialise(ThisPlayer, Player.AllPlayers[0], Cooldown, ActiveTime, AOERadius, AOEDamage, LookDamage, AbilityInstance);
            Debug.Log("setting ability");
        }
        else if (!IsClone)
        {
            if (!DoorsDeleted && Door.AllDoors.Count > 0)
            {
                DoorsDeleted = true;

                foreach (KeyValuePair<string, Door> entry in Door.AllDoors)
                {
                    Destroy(entry.Value.GetObject().GetComponent<MeshCollider>());
                    Debug.Log("deleting door colliders");
                }
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
            if (Input.GetKeyDown(KeyCode.N))
            {
                string Msg = "ACTT1";
                ThisWrapper.SendServerMessage(Msg);
                //TestAbility.Activate();
            }
            ThisPlayer.Update();
            hotbar.Update();
            input.Update();
            Sheet.SetActive(false);
            MyCamera.SetActive(true);
            //MyFBOCam.SetActive(true);
            UIElements.SetActive(true);
            Vector2 Data = Player1Stats.GetData();
            StaminaBar.transform.localScale = new Vector3(Data.y / 100, 1, 1);
            HealthBar.transform.localScale = new Vector3(Data.x / 100, 1, 1);
            TestAbility.Update();

        }
        else if (IsClone)
        {
            ThisPlayer.Update();
            MyCamera.SetActive(false);
            TestAbility.Update();
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
    public void ActivateMaterialiseAbility()
    {
        TestAbility.Activate();
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

    public Ghost GetGhostObj()
    {
        return ThisPlayer;
    }

    [PunRPC]
    void PlayerAwake()
    {
        input = new InputManager();
        hotbar = new GhostInventory(defaultIcon, selectedIcon, emptyItem, SlotNumber, Hand);
        MyCamera.SetActive(false);
        //MyFBOCam.SetActive(false);
        ThisPlayer = new Ghost(gameObject, head, hotbar, false, input);
        Player1Stats = new GhostStatObserver(ThisPlayer);
        Player1Score = new GhostScoreObserver(ThisPlayer);
        /*FOR TUTORIAL:*///ThisPlayer.SetState(new TeachWalkState());
        /*FOR EDITING:*/
        ThisPlayer.SetState(new TeachPickupState());

        CrawlerTrap crawlertemp = new CrawlerTrap();
        hotbar.AddTrap(crawlertemp, CrawlerIcon);
    }
    [PunRPC]
    void SetPosition()
    {
        Debug.Log("THIS IS HOW MANY PLAYERS: " + Player.AllPlayers.Count + " FROM PLAYER " + ThisPlayer.GetName());
        //Player1.SetPosition(GameObject.Find("Spawn0").transform.position);
    }

    [PunRPC]
    public void SetCasterTransparency(float albedo)
    {
        Ghost.AllGhosts[0].SetTransparency(albedo);
    }

    [PunRPC]
    public void SetTargetSanity(float newSanity)
    {
        Player.AllPlayers[0].SetSanity(newSanity);
    }

    [PunRPC]
    public void SetTargetSpeed(float newspeed)
    {
        Player.AllPlayers[0].SetWalkSpeed(newspeed);
    }

    [PunRPC]
    public void SetCasterSpeed(float newspeed)
    {
        Ghost.AllGhosts[0].SetWalkSpeed(newspeed);
    }

    [PunRPC]
    public void PlayGhostAudio()
    {
        AbilityInstance.start();
    }

    [PunRPC]
    public void StopGhostAudio()
    {
        AbilityInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
