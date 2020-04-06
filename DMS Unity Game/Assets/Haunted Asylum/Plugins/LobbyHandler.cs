using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class LobbyHandler : MonoBehaviour
{
    public string Name = "";
    public bool ClientRunning = false;
    bool AssignedName = false;
    bool StartUp = false;
    float RefreshTime = 0.1f;
    float ClientReadinessUpdate = 0.3f;

    public GameObject Loading;
    public GameObject Main;
    public GameObject Lobby;
    public GameObject ReadyButton;
    public GameObject ForceStartButton;
    public Text playerReady;
    public Text ghostReady;
    public Text Timer;

    float time = 5;

    bool PlayerReady = false;
    bool GhostReady = false;

    // Start is called before the first frame update
    void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Loading.SetActive(false);
    }

    IEnumerator SendCoroutine()
    {
        while (ClientRunning)
        {
            NetworkWrapper.SendPacket();
            yield return new WaitForSeconds(RefreshTime);
        }
    }
    IEnumerator ReceiveCoroutine()
    {
        while (ClientRunning)
        {
            NetworkWrapper.RecievePacket();
            yield return new WaitForSeconds(RefreshTime);
        }
    }
    IEnumerator HandleCoroutine()
    {
        while (ClientRunning)
        {
            NetworkWrapper.HandlePacket();
            yield return new WaitForSeconds(RefreshTime);
        }
    }

    IEnumerator CheckStatus()
    {
        while (ClientRunning)
        {
            string Msg = "GETSTAT";
            NetworkWrapper.AddMessage(Msg);
            yield return new WaitForSeconds(ClientReadinessUpdate);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if(PlayerReady)
            playerReady.text = "Player Status: Ready";
        else
            playerReady.text = "Player Status: Not Ready";

        if (GhostReady)
            ghostReady.text = "Ghost Status: Ready";
        else
            ghostReady.text = "Ghost Status: Not Ready";

        if(PlayerReady && GhostReady)
        {
            ForceStartButton.SetActive(false);
            time -= Time.deltaTime;
            Timer.text = time.ToString();

            if(time <= 0)
            {
                StartGame();
            }
        }

        ClientRunning = NetworkWrapper.GetStatus();
        if (!StartUp && ClientRunning)
        {

            StartCoroutine(SendCoroutine());
            StartCoroutine(ReceiveCoroutine());
            StartCoroutine(HandleCoroutine());
            StartCoroutine(CheckStatus());
            StartUp = true;
        }
        if (!AssignedName)
        {
            Name = NetworkWrapper.GetName();
        }

        HandleMessage();
    }


    void HandleMessage()
    {
        string Message = NetworkWrapper.GetInboundMsg();
        StringReader strRdr = new StringReader(Message);

        string Header = strRdr.ReadLine();

        if (Header == "CS")
        {
            string Type = strRdr.ReadLine();

            if(Type == "SETSTAT")
            {
                string PlayerStatus = strRdr.ReadLine();
                string GhostStatus = strRdr.ReadLine();
                if(GhostStatus == "READY")
                {
                    GhostReady = true;
                }
                else
                {
                    GhostReady = false;
                }
                if (PlayerStatus == "READY")
                {
                    PlayerReady = true;
                }
                else
                {
                    PlayerReady = false;
                }
            }
        }


        NetworkWrapper.RemoveInboundMsg();
    }

    public void Ready()
    {
        string Msg = "READY";
        Msg += '\n';
        Msg += NetworkWrapper.GetName();
        NetworkWrapper.AddMessage(Msg);

        ReadyButton.SetActive(false);
        ForceStartButton.SetActive(true);
    }
    public void Play()
    {
        NetworkWrapper.JoinGame();
        Main.SetActive(false);
        Lobby.SetActive(true);
    }
    public void StartGame()
    {
        Loading.SetActive(true);
        SceneManager.LoadScene("HauntedAsylum");
    }
    public void Exit()
    {
        Application.Quit();
    }
}
