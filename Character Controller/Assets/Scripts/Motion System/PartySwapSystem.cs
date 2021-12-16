using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace ControllerSwap
{
    public class PartySwapSystem : MonoBehaviour
    {
        public static PartySwapSystem GMS;
        public int MaxParty { get { return 3; } } // Change Value if you want to increase party size.
                                                  //Value of 3 selected as player will change characters using Dpad on xbox controller
        public int PlayerIndex { get; set; }
        [SerializeField] public List<Entity> Party = new List<Entity>();
        public bool IKGlobal = true;  // consider making an enum to make player only or everyyone????? Will this be run Server side??
                                      // Start is called before the first frame updat
        private void Awake()
        {
            DontDestroyOnLoad(this);
            Party = new List<Entity>();
            if (GMS == null)
                GMS = this;
            else 
                Destroy(this.gameObject);
            PlayerIndex = new int();
        }

        void Update()
        {
            if (Party.Count > MaxParty)
            {
                Debug.LogError("More party members in Party then allowed", this);
            }
        }


    }



}



