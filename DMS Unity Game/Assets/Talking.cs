using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talking : MonoBehaviour
{
    public GameObject _Trigger;
    [FMODUnity.EventRef]
    public string[] SFXCellGuy;

    float _matchToAnim = 5.0f;

 private bool stopUpdate = false;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("CellBanging", 0, _matchToAnim);
    }

    // Update is called once per frame
    void Update()
    {

    }

  public void Talk() { 
        FMODUnity.RuntimeManager.PlayOneShot(SFXCellGuy[1], GetComponent<Transform>().position);
    }

    public void Talk2()
    {
        FMODUnity.RuntimeManager.PlayOneShot(SFXCellGuy[2], GetComponent<Transform>().position);
    }


    void CellBanging()
    {
       FMODUnity.RuntimeManager.PlayOneShot(SFXCellGuy[0], GetComponent<Transform>().position);
    }

    public void cancelInv()
    {
        CancelInvoke();
    }

}
