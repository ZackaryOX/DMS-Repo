using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawer : Interactable
{
    public static Dictionary<string, Drawer> AllDrawers = new Dictionary<string, Drawer>();

    public Drawer(GameObject Cabinet, GameObject drawer, int NetID) : base(Cabinet)
    {
        ThisDrawer = drawer;
        ClosePosition = drawer.transform.position;
        OpenPosition = ClosePosition + new Vector3(3, 0, 0);
        this.SetNetworkID(NetID);

        DrawerName = "Drawer" + NetworkID;
        if (AllDrawers.ContainsKey(DrawerName))
        {
            Debug.Log(DrawerName + " EXISTS MORE THAN ONCE");
            DrawerName = "Drawer" + ID;
            Debug.Log(DrawerName + " RENAMED DUE TO DOUBLE EXISTENCE");
        }
        this.ThisDrawer.transform.name = DrawerName;
        AllDrawers.Add(DrawerName, this);

    }


    public GameObject GetDrawer()
    {
        return ThisDrawer;
    }
    public string GetDrawerName()
    {
        return DrawerName;
    }
    public void Interact()
    {
        if (!IsLerping)
        {
            IsLerping = true;
        }

    }

    public override void Update()
    {

        if (IsLerping)
        {
            if (!Open)
            {
                LerpParam += 1 * Time.deltaTime;

                if (LerpParam >= 1)
                {
                    LerpParam = 1;
                    IsLerping = false;
                    Open = true;
                }
            }
            else
            {
                LerpParam -= 1 * Time.deltaTime;
                if (LerpParam <= 0)
                {
                    LerpParam = 0;
                    IsLerping = false;
                    Open = false;
                }
            }
            ThisDrawer.transform.position = Vector3.Lerp(ClosePosition, OpenPosition, LerpParam);
        }
    }


    public bool GetIsOpen()
    {
        return Open;
    }

    private GameObject ThisDrawer;
    Vector3 OpenPosition;
    Vector3 ClosePosition;
    bool Open = false;
    bool IsLerping = false;
    float LerpParam = 0;
    string DrawerName;
}