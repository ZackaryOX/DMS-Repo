using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostInput
{
    private CharacterController _controller;
    private Transform _Headtransform;
    private Transform _transform;
    private Animator _animator;
    //Constructor 
    private float lerpSmoothingTime = 0.05f;
    private InputManager input;
    private GhostInventory inventory;
    public GhostInput(GameObject thisobject, GameObject tempHead, InputManager tempInput, GhostInventory tempInv)
    {
        inventory = tempInv;
        input = tempInput;
        _controller = thisobject.GetComponent<CharacterController>();
        _Headtransform = tempHead.GetComponent<Transform>();
        _transform = thisobject.GetComponent<Transform>();
        _animator = thisobject.GetComponent<Animator>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }




    //For Walking
    private float Speed = 18f;
    private float DefaultSpeed = 18.0f;
    private float StartIncrement = 10;
    private float StopIncrement = 5;
    private float VerticalVelocity = 0;
    private float HorizontalVelocity = 0;

    private float MaxVelocity = 1;
    private float MinVelocity = -1;

    //For Jumping
    private float YVelocity = 0;
    private float JumpVelocity = 10.0f;
    private float FallMultiplier = 1.2f;
    private bool IsJumping = false;
    private bool Adjusted = false;
    private int Jump = 0;
    private float StaminaDeduction = 7.5f;

    public float JumpMult = 2.5f;
    public float lowJumpMult = 2f;
    //For Gravity
    private Vector3 BaseGravityForce = new Vector3(0, -9.8f * 2.8f, 0);
    private Vector3 AdjustedGravityForce = new Vector3(0, -9.8f * 2.8f, 0);

    //For Camera
    private float speedH = 2.0f;
    private float speedV = 2.0f;
    private float MaxPitch = 65.0f;
    private float MaxYaw = 90.0f;
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    Quaternion OldRot = Quaternion.Euler(new Vector3(0, 0, 0.0f));


    public void SetWalkSpeed(float temp)
    {
        Speed = temp;
    }
    public float GetWalkSpeed()
    {
        return Speed;
    }
    public float GetDefaultSpeed()
    {
        return DefaultSpeed;
    }
    Vector3 RotateCamera(PlayerState state)
    {

        float ForYaw = state.GetLook() ? input.GetMouseX() : 0;
        float ForPitch = state.GetLook() ? input.GetMouseY() : 0;



        yaw += speedH * ForYaw;

        pitch -= speedV * ForPitch;

        pitch = pitch > MaxPitch ? MaxPitch : pitch;
        pitch = pitch < -MaxPitch ? -MaxPitch : pitch;

        return new Vector3(pitch, yaw, 0.0f);
    }

    private void ComputeJump(Stamina playerstamina)
    {
        int TempJump = 0;
        //End of jump
        if (IsJumping && _controller.isGrounded)
        {
            YVelocity = 0;
            IsJumping = false;
            Adjusted = false;
            AdjustedGravityForce = BaseGravityForce;
        }
        //During jump
        else if (YVelocity > 0 && IsJumping)
        {

            YVelocity += AdjustedGravityForce.y * Time.deltaTime;

            if (!Adjusted && (int)YVelocity == -(int)BaseGravityForce.y)
            {
                Adjusted = true;
                AdjustedGravityForce = BaseGravityForce * FallMultiplier;
            }
        }


        //If conditions are met and spacebar is pressed 
        if (_controller.isGrounded && !IsJumping)
        {

            TempJump += GetInput("jump");

            //Was SpaceBar Pressed? If so initiate jump
            if (TempJump > 0 && playerstamina.GetStamina() >= StaminaDeduction)
            {
                IsJumping = true;
                YVelocity += (AdjustedGravityForce.y * -1) + JumpVelocity;
                float NewStamina = playerstamina.GetStamina() - StaminaDeduction;
                playerstamina.SetStamina(NewStamina);
            }
        }





    }
    private int GetInput(string temp)
    {
        if (input.GetKey(temp))
        {
            return 1;
        }

        return 0;
    }
    private float CheckVelocity(float Velocity)
    {
        if (Velocity < 0.1f && Velocity > -0.1f && Velocity != 0.0f)
        {
            return 0;
        }
        return Velocity;
    }
    private float GradualMovement(int rootaxisval, float max, float min, float cur, float startinc, float stopinc)
    {
        if ((rootaxisval > 0 && cur < 0) || (rootaxisval < 0 && cur > 0))
        {
            if (cur > 0.0f)
            {
                return min / stopinc;
            }
            else if (cur < 0.0f)
            {
                return max / stopinc;
            }
        }
        //Start slowing down
        if ((rootaxisval == 0.0f) && (cur != 0.0f))
        {
            if (cur > 0.0f)
            {
                return min / stopinc;
            }
            else if (cur < 0.0f)
            {
                return max / stopinc;
            }
            return 0.0f;
        }
        //Start Speeding up
        if ((cur > min) && (cur < max))
        {
            return rootaxisval / startinc;
        }

        return 0.0f;
    }
    private void InventoryInput()
    {
        //Scroll up
        if (input.GetScrollWheel() > 0f)
        {
            inventory.SelectPrevious();
        }
        //Scroll down
        else if (input.GetScrollWheel() < 0f)
        {
            inventory.SelectNext();
        }

        if (input.GetKey("one"))
            inventory.Select(0);
        else if (input.GetKey("two"))
            inventory.Select(1);
        else if (input.GetKey("three"))
            inventory.Select(2);
        else if (input.GetKey("four"))
            inventory.Select(3);
        else if (input.GetKey("five"))
            inventory.Select(4);
        else if (input.GetKey("six"))
            inventory.Select(5);
        else if (input.GetKey("seven"))
            inventory.Select(6);
        else if (input.GetKey("eight"))
            inventory.Select(7);
        else if (input.GetKey("nine"))
            inventory.Select(8);
        else if (input.GetKey("zero"))
            inventory.Select(9);
    }



    public void Update(Stamina playerstam, PlayerState currentstate)
    {
        InventoryInput();

        int Vertical = 0;
        int Horizontal = 0;
        int Running = 0;

        float CurrentSpeed = 0;
        Vector3 Tempjumpvec;

        if (currentstate.GetWalk())
        {
            Vertical += GetInput("forward");
            Vertical -= GetInput("backward");
            Horizontal += GetInput("right");
            Horizontal -= GetInput("left");
        }

        if (currentstate.GetRun() && Vertical > 0)
            Running += GetInput("run");

        if (currentstate.GetJump())
            ComputeJump(playerstam);

        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    Debug.Log("HEAD ROT: " + _Headtransform.eulerAngles);
        //    Debug.Log("BODY ROT: " + _transform.eulerAngles);
        //    Debug.Log("CAM ROT: " + GameObject.Find("PlayerCamera").transform.eulerAngles);
        //}

        Vector3 RawRotation = RotateCamera(currentstate);
        //_transform.rotation = 
        //_Headtransform.rotation = 
        //_transform.Rotate(Vector3.right * RawRotation.x);
        Quaternion NewRot = Quaternion.Euler(new Vector3(RawRotation.x, RawRotation.y, 0));
        Quaternion FinalRotation = Quaternion.Lerp(OldRot, NewRot, Time.deltaTime / lerpSmoothingTime);

        _transform.eulerAngles = new Vector3(0, FinalRotation.eulerAngles.y, 0);
        _Headtransform.eulerAngles = new Vector3(FinalRotation.eulerAngles.x, FinalRotation.eulerAngles.y, 0);
        OldRot = NewRot;

        HorizontalVelocity += GradualMovement(Horizontal, MaxVelocity, MinVelocity, HorizontalVelocity, StartIncrement, StopIncrement);
        VerticalVelocity += GradualMovement(Vertical, MaxVelocity, MinVelocity, VerticalVelocity, StartIncrement, StopIncrement);
        // Debug.Log("AFTER" + VerticalVelocity);

        //Reduces diagonal speed if A/D + W/S are pressed together
        CurrentSpeed = Vertical != 0 && Horizontal != 0 ? Speed / 2 : Speed;


        CurrentSpeed = (Running > 0) && IsJumping == false ? CurrentSpeed * playerstam.DecreaseStam(Time.deltaTime) : CurrentSpeed;

        if (Running == 0 && !IsJumping)
            playerstam.IncreaseStam(Time.deltaTime, Mathf.Abs(Vertical) + Mathf.Abs(Horizontal));


        HorizontalVelocity = CheckVelocity(HorizontalVelocity);
        VerticalVelocity = CheckVelocity(VerticalVelocity);

        if (Horizontal != 0 || Vertical != 0)
        {
            _animator.SetBool("IsWalking", true);

        }
        else
        {
            _animator.SetBool("IsWalking", false);
        }

        Vector3 VerticalMovement = _Headtransform.forward * VerticalVelocity;
        Vector3 HorizontalMovement = _Headtransform.right * HorizontalVelocity;





        Tempjumpvec = Vector3.up * YVelocity * Time.deltaTime;




        Vector3 MovementFinal = (HorizontalMovement + VerticalMovement) * CurrentSpeed * Time.deltaTime;
        MovementFinal.y = 0;
        Vector3 GravityFinal = AdjustedGravityForce * Time.deltaTime;


        Vector3 Final = MovementFinal + GravityFinal + Tempjumpvec;
        _controller.Move(Final);
    }
}
