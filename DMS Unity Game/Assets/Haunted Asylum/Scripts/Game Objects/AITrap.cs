using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITrap : MonoBehaviour
{
    public bool isActive = false;
    public GameObject AIModel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
 
        if (!isActive)
        {


            if (Player.AllPlayers.Count > 0)
            {
                Player Target = Player.AllPlayers[0];
                Transform Targetstrans = Target.GetObject().transform;
                Transform Casterstrans = gameObject.transform;

                int layerMask = 1 << 11;
                layerMask = ~layerMask;
                RaycastHit hit = new RaycastHit();
                Vector3 Direction = Casterstrans.position - Targetstrans.position;
                float Distance = Vector3.Distance(Casterstrans.position, Targetstrans.position);
                if (Distance <= 10 && !Physics.Raycast(Targetstrans.position, Direction, out hit,
                       Distance, layerMask))
                {
                    AIModel.SetActive(true);
                    isActive = true;
                }

                //
            }

        }
    }
}
