using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    public static GameSetup GS;
    public Transform[] spawnpoints;
    // Start is called before the first frame update
    public void Awake()
    {

        if (GameSetup.GS == null)
        {
            GameSetup.GS = this;
        }
    }
}
