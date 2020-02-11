using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForDrawer : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject OutterDrawer;
    Drawer ThisDrawer;
    PhotonView DrawerView;

    [FMODUnity.EventRef]
    public string musicEventName = "";
    FMOD.Studio.EventInstance musiceventinstance;

    private float CurrentState = 0;
    void Awake()
    {
        ThisDrawer = new Drawer(OutterDrawer, gameObject);
    }
    private void Start()
    {
        DrawerView = GetComponent<PhotonView>();
        //Instantiate the FMOD instance
        musiceventinstance = FMODUnity.RuntimeManager.CreateInstance(musicEventName);
        //Attach the event to the object
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(musiceventinstance, GetComponent<Transform>(), GetComponent<Rigidbody>());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && GetComponent<mouseHovor>().mouseOver == true)
        {
            DrawerView.RPC("DrawerInteract", RpcTarget.AllBuffered, null);
        }

        ThisDrawer.Update();
    }


    [PunRPC]
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