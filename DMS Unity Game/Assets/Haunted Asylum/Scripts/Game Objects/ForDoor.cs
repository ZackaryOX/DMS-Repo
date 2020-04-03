using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForDoor : MonoBehaviour
{
    // Start is called before the first frame update
    public Door ThisDoor;
    public GameObject Keyobject;
    private PickUp ThisKey;
    public bool locked;
    NetworkWrapper ThisWrapper;


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


        if (locked == true)
            ThisKey = PickUp.AllItems[Keyobject.name];

        ThisDoor.SetKey(ThisKey);

        ThisWrapper = GameObject.Find("NetworkManager").GetComponent<NetworkWrapper>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<mouseHovor>().mouseOver == true && Input.GetKeyDown(KeyCode.E))
        {
            DoorInteract();


        }
        else if (locked && ThisKey.GetPicked() == true && GetComponent<mouseHovor>().mouseOver == true && Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Player.AllPlayers[0].UseItemInInventory(ThisKey))
            {
                DoorUnlock();
            }

        }
        ThisDoor.Update();
    }


    void DoorInteract()
    {
        if (ThisDoor.GetIsLocked() == false && ThisDoor.IsSlerping == false)
        {
            //string Msg = "UPDRAWER";
            //Msg += "\n";
            ////Msg += this.NetworkID;
            //ThisWrapper.SendServerMessage(Msg);

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

    void DoorUnlock()
    {
        ThisDoor.UnlockDoor();

    }
}
