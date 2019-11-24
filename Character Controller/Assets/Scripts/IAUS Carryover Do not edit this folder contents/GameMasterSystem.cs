using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class GameMasterSystem : MonoBehaviour
{
    public static GameMasterSystem GMS;
    public int MaxParty;
    public int PlayerIndex;
    [SerializeField] public List<Entity> Party = new List<Entity>();
    // Start is called before the first frame update
    void Start()
    {
        if (GMS == null)
        {
            GMS = this;
        }
        else if (GMS != this) {
            Destroy(this.gameObject);
        }
        Party = new List<Entity>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Party.Count > MaxParty)
        {
            Debug.LogError("More party members in Party then allowed",this);
        }


        if (Input.GetKeyUp(KeyCode.P))
        {

            if (PlayerIndex >= Party.Count - 1)
            {
                PlayerIndex = 0;
            }
            else
                PlayerIndex++;
        }

    }
}
