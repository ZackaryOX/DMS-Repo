using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public static Dictionary<int, Player> AllPlayers = new Dictionary<int, Player>();
    public Queue<IEnumerator> CoroutinesToFire = new Queue<IEnumerator>();
    static int Players = 0;
    //Constructor
    public Player(GameObject thisobject, GameObject temphead, PlayerInventory tempinv, bool isEditor, InputManager tempInput, GameObject handtarget) : base(thisobject)
    {
        Head = temphead;

        ThisInput = new PlayerInput(thisobject, temphead, tempInput, tempinv);
        ThisStamina = new Stamina(100, 12.5f, 40.0f);
        ThisInventory = tempinv;
        Name = "Player" + Players.ToString();
        thisobject.name = Name;
        SetDefaultHandTarget(handtarget);
        SetHandTarget(handtarget);
        Health = 100;
        Sanity = 100;
        input = tempInput;

        Rotlist.Add(X);
        Rotlist.Add(Y);
        Rotlist.Add(Z);
        if (!isEditor)
        {
            PlayerNumber = Players;
            Players++;
            AllPlayers.Add(PlayerNumber, this);
            Added = true;
        }
    }

    ~Player()
    {
        if(Added)
        {
            AllPlayers.Remove(PlayerNumber);
        }
    }

    //Public
    public PlayerState GetState()
    {
        return Mystate;
    }
    public float GetHealth()
    {
        return Health;
    }
    public float GetSanity()
    {
        return Sanity;
    }
    public float GetStamina()
    {
        return this.ThisStamina.GetStamina();
    }

    public void SetHealth(float temp)
    {
        Health = temp;
    }
    public float GetScore()
    {
        return TutorialScore;
    }

    public void SetSanity(float temp)
    {
        Sanity = temp;
    }
    public override void Update()
    {

        ThisInput.Update(ThisStamina, Mystate, CurrentHandTarget);
        TutorialScore = Timer.ElapsedTime;

        if(GetSanity() < 100)
        {
            Sanity += SanityRegen * Time.deltaTime;
            if(Sanity > 100.0f)
            {
                Sanity = 100;
            }

            if(Health > 0.0f)
            {
                float Multiplier = 0;
                if(Sanity < 75)
                {
                    Multiplier = 1;
                }
                if (Sanity < 50)
                {
                    Multiplier = 2;
                }
                if(Sanity < 25)
                {
                    Multiplier = 3;
                }

                Health -= Multiplier * HealthDamage * Time.deltaTime;
            }

            GetObject().GetComponent<PhotonView>().RPC("UpdatePlayer", RpcTarget.OthersBuffered, GetSanity(), GetHealth());
        }

        foreach(KeyValuePair<int, PlayerObserver> entry in Observers)
        {
            entry.Value.Update();
        }
        if (CanHold)
        {
            ThisInventory.Update();
        }
    }
    public IEnumerator PickUpCoroutine()
    {
        Animator ThisAnim = ThisObject.GetComponent<Animator>();

        //CanHold = false;
        ThisAnim.SetBool("IsPicking", true);
        yield return new WaitForSeconds(2.3f);

        ThisAnim.SetBool("IsPicking", false);
        //CanHold = true;
    }

    public bool AddItemToInventory(string pickupname) {
        if (Mystate.GetPickup())
        {
            
            return this.ThisInventory.PickupItem(PickUp.AllItems[pickupname]);
        }
        return false;
    }
    public bool UseItemInInventory(PickUp tempitem)
    {
        return this.ThisInventory.UseItem(tempitem);
    }

    public void AttachObserver(PlayerObserver temp)
    {
        Observers.Add(temp.GetID(), temp);
    }

    public void SetState(PlayerState temp)
    {
        Mystate = temp;
    }

    public void AdvanceLevel()
    {
        Mystate.Advance(this);
    }
    public void DettachObserver(PlayerObserver temp)
    {
        Observers.Remove(temp.GetID());
    }
    public GameObject GetHead()
    {
        return Head;
    }
    public int GetPlayerNumber()
    {
        return this.PlayerNumber;
    }
    public void SetWalkSpeed(float temp)
    {
        ThisInput.SetWalkSpeed(temp);
    }
    public float GetWalkSpeed()
    {
        return ThisInput.GetWalkSpeed();
    }
    public float GetDefaultSpeed()
    {
        return ThisInput.GetDefaultSpeed();
    }
    public void SetHandTarget(GameObject newhand)
    {
        CurrentHandTarget = DefaultHandTarget;
    }
    public void SetDefaultHandTarget(GameObject newhand)
    {
        DefaultHandTarget = newhand;
    }

    private float X = 2;//15;
    private float Y = -369;//-90;
    private float Z = 3;//-180;
    int index = 0;

    List<float> Rotlist = new List<float>();
    private float lerpSmoothingTime = 0.05f;
    Quaternion OldRot = Quaternion.Euler(new Vector3(0, 0, 0.0f));
    Quaternion NewRot = Quaternion.Euler(new Vector3(0, 0, 0.0f));
    Vector3 OldPos = new Vector3(0,0,0);
    public void PutHandOut()
    {
        if (CanHold && ThisInventory.IsItemInHand())
        {
            
            if (Input.GetKeyDown(KeyCode.X))
            {
                index = 0;
                
            }
            else if (Input.GetKeyDown(KeyCode.Y))
            {
                index = 1;
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                index = 2;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Rotlist[index] += 1;

            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Rotlist[index] -= 1;
                Debug.Log("doing");

            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Log("X: " + X + " Y: " + Y + " Z: " + Z);
                
            }
            
            X = Rotlist[0];
            Y = Rotlist[1];
            Z = Rotlist[2];
            Animator ThisAnim = this.GetObject().GetComponent<Animator>();
            Vector3 worldpos = new Vector3(X, Y, Z);
            float YTilt = (ThisInput.GetPitch() / 65) * 3;
            Vector2 mousePos = Input.mousePosition;
            worldpos.x += mousePos.x;
            worldpos.y += mousePos.y;

            Vector3 newVec = Camera.main.ScreenToWorldPoint(worldpos) + new Vector3(0, -YTilt,0);
            Vector3 NewPos = Vector3.Lerp(OldPos, newVec, Time.deltaTime/lerpSmoothingTime);
            OldPos = newVec;
            ThisAnim.SetIKPosition(AvatarIKGoal.RightHand, NewPos);
            ThisAnim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0.42f);


            Vector3 First = new Vector3(0, -90 + ThisInput.GetNewRot().y, -180 - ThisInput.GetNewRot().x);
            
            
            NewRot = Quaternion.Euler(First);
            Quaternion NewTemp = Quaternion.Lerp(OldRot, NewRot, Time.deltaTime/lerpSmoothingTime);
            ThisAnim.SetIKRotation(AvatarIKGoal.RightHand, NewTemp);
            OldRot = NewRot;
            ThisAnim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        }
    }

    public void AddCoroutineToFire(IEnumerator CoRoutine)
    {
        this.CoroutinesToFire.Enqueue(CoRoutine);
    }
    public InputManager input;
    public bool CanInteract = false;

    //Private
    private Dictionary<int, PlayerObserver> Observers = new Dictionary<int, PlayerObserver>();
    public float Health = 100;
    private float Sanity;
    PlayerState Mystate;
    private GameObject DefaultHandTarget;
    private GameObject CurrentHandTarget;
    private float TutorialScore = 0;
    private PlayerInput ThisInput;
    private PlayerInventory ThisInventory;
    private GameObject Head;
    private int PlayerNumber;
    private bool Added = false;
    private Stamina ThisStamina;
    private float SanityRegen = 0.75f;
    private float HealthDamage = 0.5f;
    private bool CanHold = true;
}