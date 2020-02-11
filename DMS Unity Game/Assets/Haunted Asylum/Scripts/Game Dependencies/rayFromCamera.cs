using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rayFromCamera : MonoBehaviour
{
    public string lookingAt = null; //static so the other script can read the name.
    public string displayObject;
    public RaycastHit obj;

    void Update()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out obj))
        { //Sends vector from the position of camera forward and grabs the first object it hits.
            lookingAt = obj.transform.gameObject.name; //Outputs the name of the object the camera is looking at to send to our mouseHover script.
            displayObject = lookingAt; //Outputs the name of the object the camera is looking at for debugging purposes.
        }
    }


}
