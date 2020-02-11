using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina
{
    //CONSTRUCTOR:
    public Stamina(float stamina, float regen, float decrease)
    {
        MaxStam = stamina;
        CurrentStam = stamina;
        Regen = regen;
        Decrease = decrease;
    }
    //PUBLIC:
    public void SetStamina(float temp)
    {
        CurrentStam = temp;
    }
    public void SetRegen(float temp)
    {
        Regen = temp;
    }
    public float GetStamina()
    {
        return CurrentStam;
    }
    public float GetRegen()
    {
        return Regen;
    }

    public float DecreaseStam(float deltatime)
    {
        float Returnval = 0;

        float Deduction = CurrentStam >= Decrease * deltatime ? 1 : 0;

        CurrentStam -= Decrease * deltatime * Deduction;

        Returnval = 2 * Deduction > 0 ? 2 * Deduction : 1;

        return Returnval;
    }
    public void IncreaseStam(float deltatime, int state)
    {
        float CheckStam = CurrentStam < MaxStam ? 1 : 0;

        //State is whether or not the player is moving, if the state is 0
        //that means the player is standing still, if its positive or negative
        //the player is moving. When moving stamina regen is decreased by half.
        float StamIncr = state > 0 || state < 0 ? 1 * deltatime : 2 * deltatime;

        CurrentStam = CurrentStam + StamIncr * Regen;
        CurrentStam = CurrentStam >= MaxStam ? MaxStam : CurrentStam;
    }

    //PRIVATE:

    private float MaxStam;
    private float CurrentStam;
    private float Regen;
    private float Decrease;
}
