using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkController : MonoBehaviourPunCallbacks
{
    //From Zack: If you need any help regarding the networking plugin, use these links
    //DOCUMENTATION: https://doc.photonengine.com/en-us/pun/current/getting-started/pun-intro
    //SCRIPTING API: https://doc-api.photonengine.com/en/pun/v2/index.html


    public static NetworkController ThisLobby;
    private static bool RoomCreated = false;
    RoomOptions roomOps;
    RoomInfo ThisRoom;
    private static bool ConnectedToMaster = false;
    // Start is called before the first frame update
    public Text Status;
    public GameObject JoinRoom;
    public GameObject LeaveRoom;
    public GameObject Room;
    public string roomname = "Room1";
    void Start()
    {
        ThisLobby = this;
        PhotonNetwork.ConnectUsingSettings();//connects to photons master server
        roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)MultiplayerSettings.multiplayerSettings.maxplayers };
    }

    public override void OnConnectedToMaster()
    {
        ConnectedToMaster = true;
        PhotonNetwork.AutomaticallySyncScene = true;
        Status.text = PhotonNetwork.CloudRegion + " server";
        JoinRoom.SetActive(true);
        LeaveRoom.SetActive(false);
    }

    public void OnRoundStart()
    {
        if(PhotonNetwork.JoinRoom(roomname))
        {
            JoinRoom.SetActive(false);
            LeaveRoom.SetActive(true);
        }
    }
    public void OnRoundCancel()
    {
        if(PhotonNetwork.LeaveRoom())
        {
            Room.SetActive(false);
            JoinRoom.SetActive(true);
            LeaveRoom.SetActive(false);
        }
    }
    private void CreateRoom(int roomName)
    {
        if(PhotonNetwork.CreateRoom(roomname, roomOps))
            Debug.Log("Room created");

    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Joining Failed, creating new room");
        if (ConnectedToMaster)
        {
            RoomCreated = true;
            CreateRoom(1);
        }
    }
    public override void OnJoinedRoom()
    {
        Room.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            OnRoundStart();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            OnRoundCancel();
        }
    }
}
