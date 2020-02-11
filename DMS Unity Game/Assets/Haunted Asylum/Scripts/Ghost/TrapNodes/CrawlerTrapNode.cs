using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerTrapNode : TrapNode
{
    public GameObject StartPos;
    public GameObject EndPos;
    public GameObject Prefab;
    private GhostmouseHover MyMouse;
    public int LerpTime;
    CrawlerTrap Slot;
    PhotonView MyView;

    [FMODUnity.EventRef]
    public string DamageEvent = "";
    FMOD.Studio.EventInstance Damageeventinstance;
    // Start is called before the first frame update
    void Start()
    {
        MyMouse = GetComponent<GhostmouseHover>();
        MyView = GetComponent<PhotonView>();
        Slot = new CrawlerTrap(Prefab, StartPos, EndPos);





        Damageeventinstance = FMODUnity.RuntimeManager.CreateInstance(DamageEvent);
        //Attach the event to the object
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.AllPlayers.Count > 0 &&  Ghost.AllGhosts.Count > 0)
        {

            if (Ghost.AllGhosts[0].GetTrap() != null && Ghost.AllGhosts[0].GetTrap().type == TrapType.CRAWLER && MyMouse.mouseOver && Input.GetKeyDown(KeyCode.E))
            {

                isActive = true;

            }
                
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
        if (Slot != null)
        {
            Slot.Update(MyView);
        }
    }

    public override void ActivateTrap()
    {
        Slot.Initiate(LerpTime, Damageeventinstance);
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
