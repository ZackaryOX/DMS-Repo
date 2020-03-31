using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class GhostAbilities
{
    protected float TimeActivated = 0;
    protected float TimeActive = 0;
    protected float Cooldown = 0;
    protected float TimeTillCooldown = 0;
    protected Ghost Caster;
    protected Player Target;
    protected bool Active = false;
    protected FMOD.Studio.EventInstance AbilityInstance;

    protected GhostAbilities(Ghost tempghost, Player tempplayer, float cd, float timeactive, FMOD.Studio.EventInstance tempinstance)
    {
        Caster = tempghost;
        Target = tempplayer;
        Cooldown = cd;
        TimeActive = timeactive;
        AbilityInstance = tempinstance;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }
    public virtual void Update(PhotonView temp)
    {

    }
    public virtual bool Activate()
    {

        return false;
    }

    public float GetCoolDown()
    {
        return TimeTillCooldown;
    }
}


public class Materialise : GhostAbilities
{

    //THIS IS PER SECOND DAMAGE
    private float AOEDamage = 2.0f;
    private float LookDamage = 2.0f;
    private float AOERadius;
    private bool IsTransforming = false;
    private float TransformAlbedo = 0.1f;
    private float TransformSpeed = 1.0f;
    private float OldCasterSpeed;
    private float NewCasterSpeed;
    private float Divider = 2.0f;
    public Materialise(Ghost tempghost, Player tempplayer, float cd, float timeactive, float AOERad, float AOEDmg, float Look, FMOD.Studio.EventInstance tempinstance)
        : base(tempghost, tempplayer, cd, timeactive, tempinstance)
    {
        AOEDamage = AOEDmg;
        AOERadius = AOERad;
        LookDamage = Look;
    }


    public override bool Activate()
    {
        if (TimeTillCooldown == 0.0f)
        {
            TimeActivated = Timer.ElapsedTime;
            TimeTillCooldown = Cooldown;
            Active = true;
            IsTransforming = true;
        }


        return true;
    }

    public override void Update()
    {
        //Start of trap, beginning transformation and playing music
        if (Active && IsTransforming)
        {
            TransformAlbedo += TransformSpeed * Time.deltaTime;
            if (TransformAlbedo >= 1.0f)
            {
                TransformAlbedo = 1.0f;
                IsTransforming = false;
                //temp.RPC("SetCasterSpeed", RpcTarget.AllBuffered, Caster.GetDefaultSpeed() / Divider);
                Caster.SetWalkSpeed(Caster.GetDefaultSpeed() / Divider);
                //temp.RPC("PlayGhostAudio", RpcTarget.AllBuffered);
                AbilityInstance.start();
            }

            //temp.RPC("SetCasterTransparency", RpcTarget.AllBuffered, TransformAlbedo);
            Caster.SetTransparency(TransformAlbedo);
        }
        //End of trap, transforming back to invisible and stopping music
        else if (!Active && IsTransforming)
        {
            TransformAlbedo -= TransformSpeed * Time.deltaTime;
            if (TransformAlbedo <= 0.1f)
            {
                TransformAlbedo = 0.1f;
                IsTransforming = false;
                //temp.RPC("StopGhostAudio", RpcTarget.AllBuffered);
                AbilityInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
            //temp.RPC("SetCasterTransparency", RpcTarget.AllBuffered, TransformAlbedo);
            Caster.SetTransparency(TransformAlbedo);
        }

        //Checks for player and if so reduces their walkspeed and does damage to them
        if (Active && Timer.ElapsedTime - TimeActivated <= TimeActive)
        {
            Transform TargetsHeadtrans = Target.GetHead().transform;
            Transform Targetstrans = Target.GetObject().transform;
            Transform Casterstrans = Caster.GetObject().transform;

            int layerMask = 1 << 11;
            layerMask = ~layerMask;
            RaycastHit hit;
            Vector3 Direction = Targetstrans.position - Casterstrans.position;
            float Distance = Vector3.Distance(Casterstrans.position, Targetstrans.position);
            if (!Physics.Raycast(Targetstrans.position, Direction, out hit,
               Distance, layerMask))
            {
                float thisFramesDamage = 0;

                if (Vector3.Angle(TargetsHeadtrans.forward, Casterstrans.position - TargetsHeadtrans.position) <= 60)
                {

                    //Debug.Log("doing look");
                    thisFramesDamage += LookDamage * Time.deltaTime;

                }

                if (Distance <= AOERadius)
                {
                    //Debug.Log("doing aoe");
                    thisFramesDamage += AOEDamage * Time.deltaTime;

                    if(Target.GetWalkSpeed() != Target.GetDefaultSpeed() / Divider)
                    {
                        //temp.RPC("SetTargetSpeed", RpcTarget.AllBuffered, Target.GetDefaultSpeed() / Divider); - FIX
                        Target.SetWalkSpeed(Target.GetDefaultSpeed() / Divider);
                    }
                    
                }
                else if(Target.GetWalkSpeed() != Target.GetDefaultSpeed())
                {
                    //temp.RPC("SetTargetSpeed", RpcTarget.AllBuffered, Target.GetDefaultSpeed()); - FIX
                    Target.SetWalkSpeed(Target.GetDefaultSpeed());
                }


                if(thisFramesDamage > 0)
                {
                    float CurrentSanityToSet = 0;
                    float SanityToTest = Target.GetSanity() - thisFramesDamage;
                    if (SanityToTest > 0.0f)
                    {
                        CurrentSanityToSet = SanityToTest;
                    }
                    //temp.RPC("SetTargetSanity", RpcTarget.AllBuffered, CurrentSanityToSet); - FIX
                    Target.SetSanity(CurrentSanityToSet);
                }
            }

        }
        //Starts to disable the trap after the time active has been reached.
        else if (Active && Timer.ElapsedTime - TimeActivated >= TimeActive)
        {
            //temp.RPC("SetCasterSpeed", RpcTarget.AllBuffered, Caster.GetDefaultSpeed()); - FIX
            Caster.SetWalkSpeed(Caster.GetDefaultSpeed());

            if (Target.GetWalkSpeed() != Target.GetDefaultSpeed())
            {
                //temp.RPC("SetTargetSpeed", RpcTarget.AllBuffered, Target.GetDefaultSpeed()); - FIX
                Target.SetWalkSpeed(Target.GetDefaultSpeed());
            }
            Active = false;
            IsTransforming = true;
        }


        if (TimeTillCooldown > 0.0f)
        {
            TimeTillCooldown -= Time.deltaTime;
            if (TimeTillCooldown < 0.0f)
            {
                TimeTillCooldown = 0.0f;
            }
            //Debug.Log("TIMETILL COOLDOWN"+TimeTillCooldown);
        }

    }
    //public override void Update()
    //{

    //    if(Active && IsTransforming)
    //    {
    //        TransformAlbedo += TransformSpeed * Time.deltaTime;
    //        if(TransformAlbedo >= 1.0f)
    //        {
    //            TransformAlbedo = 1.0f;
    //            IsTransforming = false;
    //        }
    //        Caster.SetTransparency(TransformAlbedo);
    //    }
    //    else if(!Active && IsTransforming)
    //    {
    //        TransformAlbedo -= TransformSpeed * Time.deltaTime;
    //        if (TransformAlbedo <= 0.1f)
    //        {
    //            TransformAlbedo = 0.1f;
    //            IsTransforming = false;
    //        }
    //        Caster.SetTransparency(TransformAlbedo);
    //    }


    //    if (Active && Timer.ElapsedTime - TimeActivated <= TimeActive)
    //    {
    //        Transform TargetsHeadtrans = Target.GetHead().transform;
    //        Transform Targetstrans = Target.GetObject().transform;
    //        Transform Casterstrans = Caster.GetObject().transform;

    //        int layerMask = 1 << 8;
    //        layerMask = ~layerMask;
    //        RaycastHit hit;
    //        Vector3 Direction = Targetstrans.position - Casterstrans.position;
    //        float Distance = Vector3.Distance(Casterstrans.position, Targetstrans.position);
    //        if (!Physics.Raycast(Casterstrans.position, Direction, out hit,
    //           Distance, layerMask)) 
    //        {


    //            if (Vector3.Angle(TargetsHeadtrans.forward, Casterstrans.position - TargetsHeadtrans.position) <= 55)
    //            {

    //                Debug.Log("LOOKING AND CAN SEE");
    //                float CurrentDamage = LookDamage * Time.deltaTime;
    //                float CurrentSanityToSet = 0;
    //                float SanityToTest = Target.GetSanity() - CurrentDamage;
    //                if (SanityToTest > 0.0f)
    //                {
    //                    CurrentSanityToSet = SanityToTest;
    //                }


    //                Target.SetSanity(CurrentSanityToSet);



    //            }

    //            if(Distance <= AOERadius)
    //            {
                    
    //                float CurrentDamage = AOEDamage * Time.deltaTime;
    //                float CurrentSanityToSet = 0;
    //                float SanityToTest = Target.GetSanity() - CurrentDamage;
    //                if (SanityToTest > 0.0f)
    //                {
    //                    CurrentSanityToSet = SanityToTest;
    //                }


    //                Target.SetSanity(CurrentSanityToSet);
    //            }

    //        }




    //    }
    //    else if(Active && Timer.ElapsedTime - TimeActivated >= TimeActive)
    //    {
    //        Active = false;
    //        IsTransforming = true;
    //    }


    //    if(TimeTillCooldown > 0.0f)
    //    {
    //        TimeTillCooldown -= Time.deltaTime;
    //        if(TimeTillCooldown < 0.0f)
    //        {
    //            TimeTillCooldown = 0.0f;
    //        }
    //    }
    //}


}



