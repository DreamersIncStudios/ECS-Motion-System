using DreamersInc;
using DreamersInc.BestiarySystem;
using DreamersIncStudios.MoonShot;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Utilities;

public class LevelManager : MonoBehaviour
{
    GameMaster GM;
    // Start is called before the first frame update
    void Start()
    {
        GM= GameMaster.Instance;
        BestiaryDB.SpawnPlayer(2,new Vector3(0,0,35));
        BestiaryDB.SpawnNPC(0, new Vector3(0,1,25));


        for (int i = 0; i < 5; i++)
        {
         var pos = new Vector3(100,0,0+15*i);
            BestiaryDB.SpawnNPC(3, pos);
         
        }

        for (int i = 0; i < 5; i++)
        {
            var pos = new Vector3(80, 0.1f, 0 + 15 * i);
            BestiaryDB.SpawnNPC(4, pos);

        }
    }

   public virtual void LoadLevel() {
        BestiaryDB.SpawnPlayer(2,new Vector3(0,1,25));

    }
}
