using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class NetworkedPlayer : MonoBehaviour
{
    public NetworkWrapper ThisWrapper;


    private Queue<Vector3> InboundRot = new Queue<Vector3>();
    private Queue<Vector3> InboundPos = new Queue<Vector3>();
    private Queue<Vector3> InboundAnim = new Queue<Vector3>();
    private float WaitTime = 0.2f;
    private float LerpTime = 0;
    private Vector3 StartPos = new Vector3(0, 0, 0);
    private Vector3 StartRot = new Vector3(0, 0, 0);
    Character ThisCharacter;
    GhostCharacter ThisGhost;
    Animator ThisAnim;


    public void AddUpdate(Vector3 Pos, Vector3 Rot, Vector3 Anim)
    {
        InboundPos.Enqueue(Pos);
        InboundRot.Enqueue(Rot);
        InboundAnim.Enqueue(Anim);

    }

    public void SetWaitTime(float time)
    {
        WaitTime = time;
    }

    public void LerpAndSlerp(GameObject Obj, Vector3 _StartPos, Vector3 _EndPos, Vector3 _StartRot, Vector3 _EndRot, float Time)
    {
        Quaternion StartQuat = Quaternion.Euler(_StartRot);
        Quaternion EndQuat = Quaternion.Euler(_EndRot);

        Obj.transform.position = Vector3.Lerp(_StartPos, _EndPos, Time);
        Obj.transform.rotation = Quaternion.Lerp(StartQuat, EndQuat, Time);
    }

    // Start is called before the first frame update
    void Start()
    {
        ThisWrapper = GameObject.Find("NetworkManager").GetComponent<NetworkWrapper>();
        ThisAnim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (InboundPos.Count > 0 && InboundRot.Count > 0 && InboundAnim.Count > 0)
        {
            //transform.position = InboundPos.Dequeue();
            //transform.rotation = Quaternion.Euler(InboundRot.Dequeue());
            if (LerpTime == 0)
            {

                Vector3 Animvec = InboundAnim.Peek();
                bool walking = Animvec.x == 1 ? true : false;
                bool running = Animvec.y == 1 ? true : false;
                ThisAnim.SetBool("IsWalking", walking);
                ThisAnim.SetBool("IsRunning", running);
                StartPos = gameObject.transform.position;
                StartRot = gameObject.transform.eulerAngles;
                LerpTime += Time.deltaTime / WaitTime;
                LerpAndSlerp(gameObject, StartPos, InboundPos.Peek(), StartRot, InboundRot.Peek(), LerpTime);
            }
            else if (LerpTime >= 1.0f)
            {
                LerpTime = 0;
                Vector3 FinalPos = InboundPos.Dequeue();
                Vector3 FinalRot = InboundRot.Dequeue();
                LerpAndSlerp(gameObject, StartPos, FinalPos, StartRot, FinalRot, 1.0f);
                InboundAnim.Dequeue();
            }
            else if (LerpTime > 0 && LerpTime < 1.0f)
            {
                LerpTime += Time.deltaTime / WaitTime;
                LerpAndSlerp(gameObject, StartPos, InboundPos.Peek(), StartRot, InboundRot.Peek(), LerpTime);
            }
        }
    }
}
