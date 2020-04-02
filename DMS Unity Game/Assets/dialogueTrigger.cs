using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class dialogueTrigger : MonoBehaviour
{
    static bool _Once = false;
    public GameObject _AI;

    public GameObject _Player;

    public GameObject _TriggerCell;
    public GameObject _TriggerDoor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == _Player.name)
        {
            if (_Once == false)
            {
                Debug.Log("Triggered");
                _AI.GetComponent<Talking>().Talk();
                _Once = true;
            }
            else
            {
                if (other.name == _Player.name)
                {
                    if (_Once == true)
                    {
                        Debug.Log("Triggered2");
                        _AI.GetComponent<Talking>().Talk2();
                        _TriggerDoor.SetActive(false);
                        _AI.GetComponent<Talking>().cancelInv();
                        _AI.SetActive(false);
                    }
                }
            }
        }

    }

   private void OnTriggerExit(Collider other)
    {
        if (_Once == true)
        {
            _TriggerCell.SetActive(false);
        }
        
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
