using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    void Update()
    {


        if(Player.AllPlayers.Count == 0)
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
            this.gameObject.SetActive(false);
        }
    }
}
