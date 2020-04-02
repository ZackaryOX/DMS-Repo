using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Inventory
{

    //Constructor:
    public Inventory(Image Slot, Image Selected, Image emptyIcon, Text slotNumber, GameObject tempHand)
    {
        //Set defaut images
        noItemIcon = emptyIcon;
        notSelectedIcon = Slot;
        selectedIcon = Selected;
        Hand = tempHand;
        //Create hotbar
        for (int i = 0; i < 10; i++)
        {
            //Create background slots
            Slots.Add(GameObject.Instantiate(Slot, Slot.transform.parent));
            Slots[i].transform.localPosition = new Vector3(-500 + 110 * i, -450, 0);
            Text temp = GameObject.Instantiate(slotNumber, slotNumber.transform.parent);
            temp.transform.localPosition = new Vector3(-470 + 110 * i, -480, 0);
            int tempNumber = i + 1;
            if (tempNumber == 10)
                tempNumber = 0;
            temp.text = tempNumber.ToString();
            //Create item slots
            ItemIcons.Add(GameObject.Instantiate(emptyIcon, emptyIcon.transform.parent));
            ItemIcons[i].transform.localPosition = new Vector3(-500 + 110 * i, -450, 0);
        }
        //Set selected slot to "Selected" sprite
        Slots[selected].GetComponent<Image>().sprite = selectedIcon.sprite;
    }

    //Public:

    public virtual void Select(int temp)
    {
    }

    public virtual void SelectNext()
    {
    }

    public virtual void SelectPrevious()
    {
    }

    public virtual void UpdateSelected()
    {
    }

    public void Update()
    {
        UpdateSelected();
    }

    //Private:

    protected List<Image> Slots = new List<Image>() { };
    protected Image selectedIcon;
    protected Image notSelectedIcon;

    protected List<Image> ItemIcons = new List<Image>() { };
    protected Image noItemIcon;
    protected GameObject Hand;

    protected int selected = 0;
}

public class PlayerInventory : Inventory
{
    public PlayerInventory(Image Slot, Image Selected, Image emptyIcon, Text Number, GameObject tempHand) : base(Slot, Selected, emptyIcon, Number, tempHand)
    {
        for (int i = 0; i < 10; i++)
            Items.Add(null);
    }

    public bool PickupItem(PickUp item)
    {
        if (item.getCanBePicked() == true)
        {
            for (int i = 0; i < 10; i++)
            {
                if (Items[i] == null)
                {
                    item.SetPosition(new Vector3(-100, -100, -100));
                    item.SetPicked(true);
                    Items[i] = item;
                    //Change Hotbar sprite to item's sprite
                    ItemIcons[i].GetComponent<Image>().sprite = Items[i].GetIcon();
                    return true;
                }
            }
        }
        return false;
    }
    public bool UseItem(PickUp temp)
    {
        if (Items[selected] == temp)
        {
            Items[selected].SetActive(false);
            Items[selected] = null;
            //Change image to none
            ItemIcons[selected].GetComponent<Image>().sprite = noItemIcon.sprite;
            return true;
        }
        else
            return false;
    }
    public override void Select(int temp)
    {
        //Change previously selected to none
        if (Items[selected] != null)
            Items[selected].SetPosition(new Vector3(-100, -100, -100));
        Slots[selected].GetComponent<Image>().sprite = notSelectedIcon.sprite;
        selected = temp;
        //Change new selected to "selected" sprite
        Slots[selected].GetComponent<Image>().sprite = selectedIcon.sprite;
    }
    public override void SelectNext()
    {
        //Change previously selected to none
        if (Items[selected] != null)
            Items[selected].SetPosition(new Vector3(-100, -100, -100));
        Slots[selected].GetComponent<Image>().sprite = notSelectedIcon.sprite;
        selected++;
        if (selected > 9)
            selected = 0;
        //Change new selected to "selected" sprite
        Slots[selected].GetComponent<Image>().sprite = selectedIcon.sprite;
    }
    public override void SelectPrevious()
    {
        //Change previously selected to none
        if (Items[selected] != null)
            Items[selected].SetPosition(new Vector3(-100, -100, -100));
        Slots[selected].GetComponent<Image>().sprite = notSelectedIcon.sprite;
        selected--;
        if (selected < 0)
            selected = 9;
        //Change new selected to "selected" sprite
        Slots[selected].GetComponent<Image>().sprite = selectedIcon.sprite;
    }
    public override void UpdateSelected()
    {
        if (Items[selected] != null)
        {
            //Items[selected].SetPosition(GameObject.Find("mixamorig:RightHandIndex1").transform.position);
            //Items[selected].SetRotationEuler(GameObject.Find("mixamorig:RightHandIndex1").transform.rotation.eulerAngles);
            Items[selected].SetPosition(Hand.transform.position);
            Items[selected].SetRotationEuler(Hand.transform.rotation.eulerAngles);
        }
    }
    public bool IsItemInHand()
    {
        if(Items[selected] == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public PickUp GetCurrent()
    {
        return Items[selected];
    }

    private List<PickUp> Items = new List<PickUp>() { };
}

public class GhostInventory : Inventory
{
    public GhostInventory(Image Slot, Image Selected, Image emptyIcon, Text Number, GameObject tempHand) : base(Slot, Selected, emptyIcon, Number, tempHand)
    {
        for (int i = 0; i < 10; i++)
            Traps.Add(null);
    }

    public override void Select(int temp)
    {
        Slots[selected].GetComponent<Image>().sprite = notSelectedIcon.sprite;
        selected = temp;
        //Change new selected to "selected" sprite
        Slots[selected].GetComponent<Image>().sprite = selectedIcon.sprite;
    }
    public override void SelectNext()
    {
        Slots[selected].GetComponent<Image>().sprite = notSelectedIcon.sprite;
        selected++;
        if (selected > 9)
            selected = 0;
        //Change new selected to "selected" sprite
        Slots[selected].GetComponent<Image>().sprite = selectedIcon.sprite;
    }
    public override void SelectPrevious()
    {
        Slots[selected].GetComponent<Image>().sprite = notSelectedIcon.sprite;
        selected--;
        if (selected < 0)
            selected = 9;
        //Change new selected to "selected" sprite
        Slots[selected].GetComponent<Image>().sprite = selectedIcon.sprite;
    }
    public Trap GetTrap()
    {
        return Traps[selected];
    }
    public void AddTrap(Trap temp,Sprite icontemp)
    {
        for (int i = 0; i < 10; i++)
        {
            if (Traps[i] == null)
            {
                Traps[i] = temp;
                ItemIcons[i].GetComponent<Image>().sprite = icontemp;
                break;
            }
        }
    }
    public bool PlaceTrap(TrapNode temp)
    {
        if (Traps[selected] != null)
        {
            //temp.Slot = Traps[selected];
            return true;
        }
        return false;
    }

    private List<Trap> Traps = new List<Trap>() { };
}