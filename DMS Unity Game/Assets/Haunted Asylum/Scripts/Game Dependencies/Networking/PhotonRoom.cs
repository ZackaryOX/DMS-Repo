using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    //Room info
    public static PhotonRoom room;
    private PhotonView PV;

    public bool IsGameLoaded;
    public int CurrentScene;
    //Player info
    private Photon.Realtime.Player[] photonplayers;
    public int PlayersInRoom;
    public int myNumberInRoom;

    public static int playersInGame = 0;

    public Text players;
    public Text time;
    public GameObject Loading;

    //Delayed Start
    private bool readyToCount;
    private bool readyToStart;
    public float startingTime;
    private float lessThanMaxPlayers;
    private float atMaxPlayer;
    private float timeToStart;

    void Awake()
    {
        if (PhotonRoom.room == null)
        {
            PhotonRoom.room = this;
        }
        else
        {
            if (PhotonRoom.room != this)
            {
                Debug.Log("NOT THE RIGHT ROOM");
                Destroy(PhotonRoom.room.gameObject);
                PhotonRoom.room = this;
            }

        }
        //DontDestroyOnLoad(this.gameObject);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    void Start()
    {
        PV = GetComponent<PhotonView>();
        readyToCount = false;
        readyToStart = false;
        lessThanMaxPlayers = startingTime;
        atMaxPlayer = 6;
        timeToStart = startingTime;
    }

    void Update()
    {
        if (MultiplayerSettings.multiplayerSettings.delaystart)
        {
            if (PlayersInRoom == 1)
            {
                RestartTimer();
            }
            if (!IsGameLoaded)
            {
                if (readyToStart)
                {
                    atMaxPlayer -= Time.deltaTime;
                    lessThanMaxPlayers = atMaxPlayer;
                    timeToStart = atMaxPlayer;
                }
                else if (readyToCount)
                {
                    lessThanMaxPlayers -= Time.deltaTime;
                    timeToStart = lessThanMaxPlayers;
                }
                time.text = timeToStart.ToString();
                if(timeToStart <= 0)
                {
                    StartGame();
                }
            }
        }
        if(players != null)
            players.text = PlayersInRoom + "/" + MultiplayerSettings.multiplayerSettings.maxplayers;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("We are now in a room");
        photonplayers = PhotonNetwork.PlayerList;
        PlayersInRoom = photonplayers.Length;
        myNumberInRoom = PlayersInRoom;
        PhotonNetwork.NickName = myNumberInRoom.ToString();
        if (MultiplayerSettings.multiplayerSettings.delaystart)
        {
            if(PlayersInRoom >= 1)
            {
                readyToCount = true;
            }
            if(PlayersInRoom == MultiplayerSettings.multiplayerSettings.maxplayers)
            {
                readyToStart = true;
                if (!PhotonNetwork.IsMasterClient)
                    return;
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
        else
        {
            StartGame();
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        photonplayers = PhotonNetwork.PlayerList;
        PlayersInRoom++;
        if (MultiplayerSettings.multiplayerSettings.delaystart)
        {
            if (PlayersInRoom > 1)
            {
                readyToCount = true;
            }
            if (PlayersInRoom == MultiplayerSettings.multiplayerSettings.maxplayers)
            {
                readyToStart = true;
                if (!PhotonNetwork.IsMasterClient)
                    return;
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
    }

    void StartGame()
    {
        IsGameLoaded = true;
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (MultiplayerSettings.multiplayerSettings.delaystart)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        Loading.SetActive(true);
        PhotonNetwork.LoadLevel(MultiplayerSettings.multiplayerSettings.multiplayerScene);
    }
    void RestartTimer()
    {
        lessThanMaxPlayers = startingTime;
        timeToStart = startingTime;
        atMaxPlayer = 6;
        readyToCount = false;
        readyToStart = false;
    }
    void OnSceneFinishedLoading(Scene tempscene, LoadSceneMode tempmode)
    {
        CurrentScene = tempscene.buildIndex;
        if (CurrentScene == MultiplayerSettings.multiplayerSettings.multiplayerScene)
        {
            IsGameLoaded = true;
            if (MultiplayerSettings.multiplayerSettings.delaystart)
            {
                //PV.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
                RPC_CreatePlayer();
            }
            else
            {
                //RPC_CreatePlayer();

                RPC_CreatePlayer();
                //PV.RPC("RPC_CreatePlayer", RpcTarget.AllBuffered);
                
            }
        }
    }
    [PunRPC]
    private void RPC_LoadedGameScene()
    {
        
        if(playersInGame == PhotonNetwork.PlayerList.Length)
        {
            PV.RPC("RPC_CreatePlayer", RpcTarget.All);
        }
    }
    [PunRPC]
    private void RPC_CreatePlayer()
    {
        //Debug.Log("SERVER PLAYER NUMBER: " + PhotonRoom.playersInGame);

        Debug.Log("MYNUMBER: " + myNumberInRoom);
        if(myNumberInRoom == 1/*PhotonNetwork.CountOfPlayersInRooms == 0*/)
        {
            GameObject tempobj = GameObject.Find("Spawn0");

            PhotonNetwork.Instantiate(Path.Combine("Prefabs", "PlayerPrefab"), tempobj.transform.position, tempobj.transform.rotation, 0);
        }
        else
        {
            GameObject tempobj = GameObject.Find("Spawn1");

            PhotonNetwork.Instantiate(Path.Combine("Prefabs", "GhostPrefab"), tempobj.transform.position, tempobj.transform.rotation, 0);
        }
       

        PlayersInRoom++;
        

    }

}
