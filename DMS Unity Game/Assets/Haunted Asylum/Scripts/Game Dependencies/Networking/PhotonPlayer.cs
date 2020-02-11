using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PhotonPlayer : MonoBehaviour
{
    private PhotonView playerview;
    public GameObject myAvatar;

    // Start is called before the first frame update
    void Start()
    {
        //playerview = GetComponent<PhotonView>();
        //int spawnPicker = Random.Range(0, GameSetup.GS.spawnpoints.Length);
        //Transform MySpawn = GameSetup.GS.spawnpoints[spawnPicker];
        //if (playerview.IsMine)
        //{
        //    myAvatar = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "PlayerPrefab"), MySpawn.position,MySpawn.rotation, 0);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
