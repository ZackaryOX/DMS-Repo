using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PickUp : Interactable
{
    public static Dictionary<string, PickUp> AllItems = new Dictionary<string, PickUp>();

    //Constructor:
    public PickUp(GameObject thisobject, Sprite tempimg) : base(thisobject)
    {
        this.Name = "PickUp" + ID.ToString();
        ThisObject.name = this.Name;
        SetImage(tempimg);
        SetPicked(false);

        AllItems.Add(this.Name, this);
    }

    //Public:
    public Sprite GetIcon()
    {
        return this.Icon;
    }

    public void SetImage(Sprite temp)
    {
        this.Icon = temp;
    }

    public bool GetPicked()
    {
        return Picked;
    }

    public void SetPicked(bool temp)
    {
        Picked = temp;
    }

    public bool getCanBePicked()
    {
        return CanBePicked;
    }

    public void SetCanBePicked(bool temp)
    {
        CanBePicked = temp;
    }

    //Private:
    private Sprite Icon;
    private bool Picked = false;
    private bool CanBePicked = true;
}
