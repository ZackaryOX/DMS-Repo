using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class PlayerState
{
    protected bool CanLook = false;
    protected bool CanWalk = false;
    protected bool CanRun = false;
    protected bool CanJump = false;
    protected bool CanPickup = false;
    


    public bool GetWalk()
    {
        return CanWalk;
    }
    public bool GetRun()
    {
        return CanRun;
    }
    public bool GetJump()
    {
        return CanJump;
    }
    public bool GetLook()
    {
        return CanLook;
    }
    public bool GetPickup()
    {
        return CanPickup;
    }

    public virtual void Advance(Player tempplayer)
    {

    }
}


public class PausedState : PlayerState
{
    public PausedState()
    {
        CanLook = false;
        CanWalk = true;
        CanRun = false;
        CanJump = false;
        CanPickup = false;
    }

    public override void Advance(Player tempplayer)
    {
        //NOT SET UP YET
    }

}


    public class TeachWalkState : PlayerState
{

    public TeachWalkState()
    {
        CanLook = true;
        CanWalk = true;
        CanRun = false;
        CanJump = false;
        CanPickup = false;
    }

    public override void Advance(Player tempplayer)
    {
        tempplayer.SetState(new TeachRunState());
    }
}

public class TeachRunState : PlayerState
{

    public TeachRunState()
    {
        CanLook = true;
        CanWalk = true;
        CanRun = true;
        CanJump = false;
        CanPickup = false;
    }

    public override void Advance(Player tempplayer)
    {
        tempplayer.SetState(new TeachJumpState());
    }
}

public class TeachJumpState : PlayerState
{

    public TeachJumpState()
    {
        CanLook = true;
        CanWalk = true;
        CanRun = true;
        CanJump = true;
        CanPickup = false;
    }

    public override void Advance(Player tempplayer)
    {
        tempplayer.SetState(new TeachPickupState());
    }
}

public class TeachPickupState : PlayerState
{

    public TeachPickupState()
    {
        CanLook = true;
        CanWalk = true;
        CanRun = true;
        CanJump = true;   
        CanPickup = true;
    }

    public override void Advance(Player tempplayer)
    {
        //There is no other states to advance to!!!
    }
}