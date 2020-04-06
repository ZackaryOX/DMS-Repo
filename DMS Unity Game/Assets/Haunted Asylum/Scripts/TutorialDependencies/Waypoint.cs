using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public GameObject player;
    public bool LastWaypoint = false;
    // Start is called before the first frame update
    void Start()
    {


    }

    void Update()
    {


        if (Player.AllPlayers.Count == 0)
        {

        }
        else if (player == null && Player.AllPlayers.Count > 0)
        {
            player = Player.AllPlayers[0].GetObject();
        }
        else if (player && Vector3.Distance(player.transform.position, this.transform.position) < 3)
        {
            Player.AllPlayers[0].AdvanceLevel();
            Player.AllPlayers[0].AdvanceLevel();
            if (LastWaypoint)
            {
                Player.AllPlayers[0].Escaped = true;
                NetworkHandler.PlayerWon = true;

                GameObject NetworkManager = GameObject.Find("NetworkManager");
                NetworkWrapper ThisWrapper = NetworkManager.GetComponent<NetworkWrapper>();
                Timer ThisTimer = NetworkManager.GetComponent<Timer>();
                string Msg = "";
                Msg += "ADDSCORE";
                Msg += '\n';
                Msg += ThisTimer.ReturnTime();

                ThisWrapper.SendServerMessage(Msg);
            }
            this.gameObject.SetActive(false);
        }
    }
}
