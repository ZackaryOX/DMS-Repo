using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForDoor : MonoBehaviour
{
    // Start is called before the first frame update
   public Door ThisDoor;
    public GameObject Keyobject;
    PhotonView DoorView;
    private PickUp ThisKey;
    public bool locked;

    [FMODUnity.EventRef]
    public string musicEventName = "";
    FMOD.Studio.EventInstance musiceventinstance;

    private float CurrentState = 0;



    private void Awake()
    {

        ThisDoor = new Door(gameObject, locked || false);
    }
    void Start()
    {
        //Instantiate the FMOD instance
        musiceventinstance = FMODUnity.RuntimeManager.CreateInstance(musicEventName);
        //Attach the event to the object
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(musiceventinstance, GetComponent<Transform>(), GetComponent<Rigidbody>());


        if(locked == true)
        ThisKey = PickUp.AllItems[Keyobject.name];
        
        ThisDoor.SetKey(ThisKey);
        DoorView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<mouseHovor>().mouseOver == true && Input.GetKeyDown(KeyCode.E))
        {
            DoorView.RPC("DoorInteract", RpcTarget.AllBuffered, null);
            //ThisDoor.Interact();

            
        }
        else if (locked && ThisKey.GetPicked() == true && GetComponent<mouseHovor>().mouseOver == true && Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Player.AllPlayers[0].UseItemInInventory(ThisKey))
            {
                DoorView.RPC("DoorUnlock", RpcTarget.AllBuffered, null);
            }
                
        }
        ThisDoor.Update();
    }


    [PunRPC]
    void DoorInteract()
    {
        if (ThisDoor.GetIsLocked() == false && ThisDoor.IsSlerping == false)
        {
            ThisDoor.Interact();
            if (ThisDoor.GetIsOpened() == false)
            {
                CurrentState = 0;
            }
            else
            {
                CurrentState = 1;
            }

            musiceventinstance.setParameterByName("State", CurrentState);
            musiceventinstance.start();
        }
        
    }

    [PunRPC]
    void DoorUnlock()
    {
        ThisDoor.UnlockDoor();

    }
}
