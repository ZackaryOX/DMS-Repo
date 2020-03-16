using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForDrawer : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject OutterDrawer;
    Drawer ThisDrawer;

    [FMODUnity.EventRef]
    public string musicEventName = "";
    FMOD.Studio.EventInstance musiceventinstance;

    private float CurrentState = 0;
    public int NetworkID;
    NetworkWrapper ThisWrapper;
    void Awake()
    {
        ThisDrawer = new Drawer(OutterDrawer, gameObject, NetworkID);

    }
    private void Start()
    {
        //Instantiate the FMOD instance
        musiceventinstance = FMODUnity.RuntimeManager.CreateInstance(musicEventName);
        //Attach the event to the object
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(musiceventinstance, GetComponent<Transform>(), GetComponent<Rigidbody>());
        ThisWrapper = GameObject.Find("NetworkManager").GetComponent<NetworkWrapper>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && GetComponent<mouseHovor>().mouseOver == true)
        {
            string Msg = "UPDRAWER";
            Msg += "\n";
            Msg += this.NetworkID.ToString();
            ThisWrapper.SendServerMessage(Msg);
            DrawerInteract();
        }

        ThisDrawer.Update();
    }


    public void DrawerInteract()
    {



        ThisDrawer.Interact();
        if (ThisDrawer.GetIsOpen() == false)
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