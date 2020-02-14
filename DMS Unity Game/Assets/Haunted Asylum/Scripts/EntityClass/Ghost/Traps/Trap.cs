using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Trap
{
    public enum Requirements { Hallway, Doorway, Window }

    //CONSTRUCTORS:
    public Trap()
    {

    }
    //PUBLIC:
    public virtual void Initiate()
    {

    }

    //PRIVATE:
    public void AddRequirement(Requirements temp)
    {
        Require.Add(temp);
    }
    List<Requirements> Require = new List<Requirements>();
    public TrapType type = TrapType.NONE;

}

public enum TrapType
{
    NONE,
    CRAWLER
}