using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Entity
{
    public static Dictionary<int, Ghost> AllGhosts = new Dictionary<int, Ghost>();
    static int Ghosts = 0;
    //Constructor
    public Ghost(GameObject thisobject, GameObject temphead, GhostInventory tempinv, bool isEditor, InputManager tempInput) : base(thisobject)
    {
        Head = temphead;

        ThisInput = new GhostInput(thisobject, temphead, tempInput, tempinv);
        ThisStamina = new Stamina(100, 12.5f, 40.0f);
        ThisInventory = tempinv;
        thisobject.name = "Ghost" + Ghosts.ToString();
        Health = 100;


        if (!isEditor)
        {
            GhostNumber = Ghosts;
            Ghosts++;
            AllGhosts.Add(GhostNumber, this);
            Added = true;
        }
    }

    ~Ghost()
    {
        if (Added)
        {
            AllGhosts.Remove(GhostNumber);
        }
    }

    //Public
    public float GetStamina()
    {
        return this.ThisStamina.GetStamina();
    }

    public float GetScore()
    {
        return TutorialScore;
    }

    public override void Update()
    {
        if (!IsClone)
        {
            ThisInput.Update(ThisStamina, Mystate);
            TutorialScore = Timer.ElapsedTime;
        }

        foreach (KeyValuePair<int, GhostObserver> entry in Observers)
        {
            entry.Value.Update();
        }

    }

    public void AttachObserver(GhostObserver temp)
    {
        Observers.Add(temp.GetID(), temp);
    }

    public void SetState(PlayerState temp)
    {
        Mystate = temp;
    }
    public PlayerState GetState()
    {
        return Mystate;
    }
    public void AdvanceLevel()
    {
        //Mystate.Advance(this);
    }
    public void DettachObserver(PlayerObserver temp)
    {
        Observers.Remove(temp.GetID());
    }
    public void SetHealth(float temp)
    {
        Health = temp;
    }
    public float GetHealth()
    {
        return Health;
    }
    public GameObject GetHead()
    {
        return Head;
    }
    public int GetGhostNumber()
    {
        return this.GhostNumber;
    }
    public void AddRenderer(SkinnedMeshRenderer temp)
    {
        MyRenderers.Add(temp);
    }
    public Trap GetTrap()
    {
        if (this.ThisInventory.GetTrap() == null)
            return null;

        return ThisInventory.GetTrap();
    }

    public void SetTransparency(float albedo)
    {
        foreach (SkinnedMeshRenderer entry in MyRenderers)
        {
            Color tempcolor = entry.material.GetColor("_BaseColor");
            tempcolor.a = albedo;
            entry.material.SetColor("_BaseColor", tempcolor);
        }
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
    public bool GetIsClone()
    {
        return IsClone;
    }
    public void SetIsClone(bool temp)
    {
        IsClone = temp;
    }


    //Private
    private Dictionary<int, GhostObserver> Observers = new Dictionary<int, GhostObserver>();
    private List<SkinnedMeshRenderer> MyRenderers = new List<SkinnedMeshRenderer>();
    private float TutorialScore = 0;
    private GhostInput ThisInput;
    private float Health;
    PlayerState Mystate;
    private int GhostNumber = 0;
    private GhostInventory ThisInventory;
    private bool Added = false;
    private GameObject Head;
    private Stamina ThisStamina;
    private bool IsClone = false;
}
