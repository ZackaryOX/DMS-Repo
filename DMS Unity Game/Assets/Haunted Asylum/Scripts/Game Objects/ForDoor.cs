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
    public bool _PermaLocked;
    NetworkWrapper ThisWrapper;
    public int NetID;


    [FMODUnity.EventRef]
    public string musicEventName = "";
    FMOD.Studio.EventInstance musiceventinstance;

    [FMODUnity.EventRef]
    public string _LockedSoundPath = "";

    private float CurrentState = 0;



    private void Awake()
    {

        ThisDoor = new Door(gameObject, locked || false, NetID);
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
        if (GetComponent<mouseHovor>())
        {
            if (GetComponent<mouseHovor>().mouseOver == true && Input.GetKeyDown(KeyCode.E))
            {
                TestDoor();
            }
            else if (_PermaLocked == false && locked && ThisKey.GetPicked() == true && GetComponent<mouseHovor>().mouseOver == true && Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (Player.AllPlayers[0].UseItemInInventory(ThisKey))
                {
                    DoorUnlock();
                }

            }
        }
        ThisDoor.Update();
    }




    public void DoorInteract()
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

    void TestDoor()
    {
        if (ThisDoor.GetIsLocked() == false && ThisDoor.IsSlerping == false)
        {
            string Msg = "UPDDOOR";
            Msg += "\n";
            Msg += this.NetID;
            ThisWrapper.SendServerMessage(Msg);
            DoorInteract();

        }

        if (ThisDoor.GetIsLocked() == true)
        {
            if (_LockedSoundPath != null)
            {
                FMODUnity.RuntimeManager.PlayOneShot(_LockedSoundPath, GetComponent<Transform>().position);
            }
        }
    }

    public void DoorUnlock()
    {
        ThisDoor.UnlockDoor();

    }
}
