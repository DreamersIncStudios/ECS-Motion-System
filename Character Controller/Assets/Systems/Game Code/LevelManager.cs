using DreamersInc;
using DreamersInc.BestiarySystem;
using DreamersIncStudios.MoonShot;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    GameMaster GM;
    // Start is called before the first frame update
    void Start()
    {
        GM= GameMaster.Instance;
        GM.CreateGMEntity();
        Debug.Log((uint)GM.GetPlayerChoice);
        BestiaryDB.SpawnPlayer(2);
        BestiaryDB.SpawnNPC(0, new Vector3(0,1,25));
    }

   public virtual void LoadLevel() {
        BestiaryDB.SpawnPlayer(2);

    }
}
