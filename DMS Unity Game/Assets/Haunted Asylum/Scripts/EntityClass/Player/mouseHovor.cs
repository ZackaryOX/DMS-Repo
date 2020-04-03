using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseHovor : MonoBehaviour
{


    public Material NewMat;
    public GameObject Object;
    public GameObject lookingAt;


    MeshRenderer Rend;
    Material OldMat;

    public bool mouseOver = false;
    public static bool MovingStuff = false;
    public static GameObject ObjectMoving;


    void Awake()
    {
        Rend = GetComponent<MeshRenderer>();
        OldMat = Rend.material;
        Object = gameObject;
    }

   

    void Update()
    {
        if(Player.AllPlayers.Count > 0)
        {
            if(Player.AllPlayers[0].GetObject().GetComponentInChildren<rayFromCamera>() == null)
            {

            }
            else
            {
                lookingAt = GameObject.Find(Player.AllPlayers[0].GetObject().GetComponentInChildren<rayFromCamera>().lookingAt);
            }
            if (lookingAt)
            {
                if (Object.name == lookingAt.name && Vector3.Distance(Player.AllPlayers[0].GetObject().transform.position, lookingAt.transform.position) < 10)
                {
                    mouseOver = true;
                    Rend.material = NewMat;
                    Player.AllPlayers[0].CanInteract = true;
                }
                else
                {
                    mouseOver = false;
                    Rend.material = OldMat;
                }

            }

        }
    }
}
