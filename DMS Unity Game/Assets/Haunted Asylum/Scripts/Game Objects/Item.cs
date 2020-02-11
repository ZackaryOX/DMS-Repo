using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Sprite ItemImage;
    public PickUp ThisItem;
    // Start is called before the first frame update
    void Awake()
    {

        ThisItem = new PickUp(gameObject, ItemImage);

    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<mouseHovor>().mouseOver == true && Player.AllPlayers[0].input.GetKey("interact"))
        {
            //Debug.Log(ThisItem.GetPosition());
            if (Player.AllPlayers[0].AddItemToInventory(ThisItem.GetName()))
            {
                transform.parent = null;
                GetComponent<MeshCollider>().isTrigger = true;
                GetComponent<mouseHovor>().enabled = false;
                GetComponent<mouseHovor>().mouseOver = false;
            }
        }
    }
}
