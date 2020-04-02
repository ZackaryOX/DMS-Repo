using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Editor : MonoBehaviour
{
    Player ThisPlayer;
    public Canvas localGUI;
    public GameObject head;
    public GameObject MyCamera;
    public Image defaultIcon;
    public Image selectedIcon;
    public Image emptyItem;
    public Image StaminaBar;
    public Image HealthBar;
    public Image SanityBar;
    public Text SlotNumber;
    PlayerInventory hotbar;
    StatObserver Player1Stats;
    ScoreObserver Player1Score;

    int layerMask = 1 << 8;

    public GameObject HandTarget;
    private InputManager input;
    private PausedState PauseMenu;
    private PlayerState OldState;
    public AudioManager ThisAudioManager;
    public GameObject TestBox;
    public GameObject UIElements;
    float weight = 0;
    float x = 27;
    float y = -50;
    float z = -160;
    Animator ThisAnim;

    [FMODUnity.EventRef]
    public string[] SFXEventNames;

    [FMODUnity.EventRef]
    public string[] MusicEventNames;

    void Awake()
    {
        PauseMenu = new PausedState();
        PlayerAwake();
        Application.targetFrameRate = 144;
        ThisAudioManager = new AudioManager(SFXEventNames, MusicEventNames, head);
        ThisAnim = GetComponent<Animator>();
    }

    private void Start()
    {

    }

    void Update()
    {

    }

    void OnAnimatorIK()
    {

        //Animator ThisAnim = this.GetObject().GetComponent<Animator>();
        Vector3 worldpos = new Vector3(0.25f, 0, 1);

        Vector2 mousePos = Input.mousePosition;
        worldpos.x = mousePos.x;
        worldpos.y = mousePos.y;
        Vector3 newVec = Camera.main.ScreenToWorldPoint(worldpos);

        ThisAnim.SetIKPosition(AvatarIKGoal.RightHand, newVec);
        ThisAnim.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);

        ThisAnim.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.Euler(new Vector3(x, y, z) + ThisPlayer.GetRotationEuler()));
        ThisAnim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
    }

    void LateUpdate()
    {

        if (Input.GetKey(KeyCode.UpArrow))
        {
            weight += 1 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            weight -= 1 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Y))
        {
            x += 1 ;
        }
        if (Input.GetKey(KeyCode.U))
        {
            y += 1;
        }
        if (Input.GetKey(KeyCode.I))
        {
            z += 1;
        }

        if (Input.GetKey(KeyCode.H))
        {
            x -= 1;
        }
        if (Input.GetKey(KeyCode.J))
        {
            y -= 1;
        }
        if (Input.GetKey(KeyCode.K))
        {
            z -= 1;
        }
        if (Input.GetKey(KeyCode.P))
        {
            Debug.Log(weight);
        }
        ThisPlayer.Update();
        hotbar.Update();
        MyCamera.SetActive(true);
        localGUI.gameObject.SetActive(true);
        Vector3 Data = Player1Stats.GetData();
        StaminaBar.transform.localScale = new Vector3(Data.y / 100, 1, 1);
        HealthBar.transform.localScale = new Vector3(Data.x / 100, 1, 1);
        SanityBar.transform.localScale = new Vector3(Data.z / 100, 1, 1);
        ThisAudioManager.Update(100 - Data.z);



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
    
    void PlayerAwake()
    {
        layerMask = ~layerMask;
        input = new InputManager();
        //hotbar = new PlayerInventory(defaultIcon, selectedIcon, emptyItem, SlotNumber);
        ThisPlayer = new Player(gameObject, head, hotbar, true, input, HandTarget);
        Player1Stats = new StatObserver(ThisPlayer);
        Player1Score = new ScoreObserver(ThisPlayer);
        /*FOR TUTORIAL:*/ //Player1.SetState(new TeachWalkState());
                          /*FOR EDITING:*/
        ThisPlayer.SetState(new TeachPickupState());
    }
    void SetPosition()
    {
        Debug.Log("THIS IS HOW MANY PLAYERS: " + Player.AllPlayers.Count + " FROM PLAYER " + ThisPlayer.GetName());
        //Player1.SetPosition(GameObject.Find("Spawn0").transform.position);
    }
}