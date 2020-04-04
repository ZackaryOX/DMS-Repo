using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fusebox : MonoBehaviour
{
    NetworkWrapper ThisWrapper;
    private bool Activated = false;

    [FMODUnity.EventRef]
    public string _SwitchAudio;
    // Start is called before the first frame update
    void Start()
    {
        ThisWrapper = GameObject.Find("NetworkManager").GetComponent<NetworkWrapper>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<mouseHovor>().mouseOver == true && Input.GetKeyDown(KeyCode.E) && !Activated )
        {
            FMODUnity.RuntimeManager.PlayOneShot(_SwitchAudio, GetComponent<Transform>().position);
            ForDoor Door1 = GameObject.Find("Door250").GetComponent<ForDoor>();
            Door1.DoorUnlock();
            Door1.DoorInteract();
            Activated = true;
            string Msg = "UPDDOOR";
            Msg += "\n";
            Msg += 250;
            ThisWrapper.SendServerMessage(Msg);

            ForDoor Door2 = GameObject.Find("Door251").GetComponent<ForDoor>();
            Door2.DoorUnlock();
            Door2.DoorInteract();
            string Msg2 = "UPDDOOR";
            Msg += "\n";
            Msg += 251;
            ThisWrapper.SendServerMessage(Msg2);
        }
    }
}
