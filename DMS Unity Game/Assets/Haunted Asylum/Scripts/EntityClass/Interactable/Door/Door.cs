using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    public static Dictionary<string, Door> AllDoors = new Dictionary<string, Door>();

    //Constructor:
    public Door(GameObject thisobject, bool islocked, int netID, PickUp KeyItem = null) : base(thisobject)
    {
        this.Name = "Door" + netID.ToString();
        ThisObject.name = this.Name;
        Locked = islocked;
        MyKey = KeyItem;
        AllDoors.Add(this.Name, this);
        ClosedQuat = ThisObject.transform.rotation;
        OpenQuat = Quaternion.Euler(0, 90, 0) * ClosedQuat;
    }

    //Public:
    public void OpenDoor()
    {
        if(Locked == false  && IsSlerping == false)
        {
            IsSlerping = true;

            
            
        }
    }
    public void CloseDoor()
    {
        if(Open == true && IsSlerping == false)
        {
            IsSlerping = true;
        }
    }

    public void Interact()
    {
        if(Open == false)
        {
            OpenDoor();
        }
        else if(Open == true)
        {
            CloseDoor();
        }
    }
    public void UnlockDoor()
    {
            Locked = false;
    }

    public override void Update()
    {
        if(IsSlerping == true && Open == false)
        {
            SlerpTime += Time.deltaTime;
            if(SlerpTime >= 1)
            {
                IsSlerping = false;
                Open = true;
                SlerpTime = 1;
            }
            ThisObject.transform.rotation = Quaternion.Slerp(ClosedQuat, OpenQuat, SlerpTime);
        }
        else if (IsSlerping == true && Open == true)
        {
            SlerpTime -= Time.deltaTime;
            if (SlerpTime <= 0)
            {
                IsSlerping = false;
                Open = false;
                SlerpTime = 0;
            }
            ThisObject.transform.rotation = Quaternion.Slerp(ClosedQuat, OpenQuat, SlerpTime);
        }

    }

    public void SetKey(PickUp tempkey)
    {
        MyKey = tempkey;
    }
    public bool GetIsLocked()
    {
        return Locked;
    }
    public bool GetIsOpened()
    {
        return Open;
    }

    //Private:

    private bool Locked = false;
    private bool Open = false;
    public bool IsSlerping = false;
    private PickUp MyKey;

    Quaternion ClosedQuat;
    Quaternion OpenQuat;
    float SlerpTime = 0;
}
