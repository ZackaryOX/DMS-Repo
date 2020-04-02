﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerTrapNode : TrapNode
{
    public GameObject StartPos;
    public GameObject EndPos;
    public GameObject Prefab;
    private GhostmouseHover MyMouse;
    private bool TrapOver = false;
    public int LerpTime;
    CrawlerTrap Slot;

    [FMODUnity.EventRef]
    public string DamageEvent = "";
    FMOD.Studio.EventInstance Damageeventinstance;

    [FMODUnity.EventRef]
    public string scream = "";
    FMOD.Studio.EventInstance screamEvent;

    public bool _spawn = true;
    // Start is called before the first frame update
    void Start()
    {
        MyMouse = GetComponent<GhostmouseHover>();
        Slot = new CrawlerTrap(Prefab, StartPos, EndPos);





        Damageeventinstance = FMODUnity.RuntimeManager.CreateInstance(DamageEvent);
        //Attach the event to the object


    }

    // Update is called once per frame
    void Update()
    {
        if (Player.AllPlayers.Count > 0 &&  Ghost.AllGhosts.Count > 0)
        {



            isActive = true;

                
        }

        if (isActive)
        {
           

            
            if (Player.AllPlayers.Count > 0)
            {
                Player Target = Player.AllPlayers[0];
                Transform Targetstrans = Target.GetObject().transform;
                Transform Casterstrans = gameObject.transform;

                int layerMask = 1 << 11;
                layerMask = ~layerMask;
                RaycastHit hit = new RaycastHit();
                Vector3 Direction =  Casterstrans.position - Targetstrans.position;
                float Distance = Vector3.Distance(Casterstrans.position, Targetstrans.position);
                if (Distance <= 50 && !Physics.Raycast(Targetstrans.position, Direction, out hit,
                       Distance, layerMask))
                {
                    ActivateTrap();
                   
                       
                        isActive = false;
                }

                //
            }
            
        }
        if (Slot != null && !TrapOver)
        {
            TrapOver = Slot.Update();
        }
    }

    public override void ActivateTrap()
    {
        Slot.Initiate(LerpTime, Damageeventinstance);

        Slot.passScream(scream);
    }


    [PunRPC]
    public void PlayAudio()
    {
        Damageeventinstance.start();
    }

    [PunRPC]
    public void StopAudio()
    {
        Damageeventinstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
