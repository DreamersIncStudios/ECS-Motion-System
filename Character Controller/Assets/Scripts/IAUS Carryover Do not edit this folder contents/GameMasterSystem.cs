using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;


public class GameMasterSystem : MonoBehaviour
{
    public static GameMasterSystem GMS;
    public int MaxParty { get { return 3; } } // Change Value if you want to increase party size.
    //Value of 3 selected as player will change characters using Dpad on xbox controller
    public int PlayerIndex { get; set; }
    [SerializeField] public List<Entity> Party = new List<Entity>();
    [SerializeField] public InputSettings InputSettings = new InputSettings();
    public bool IKGlobal = true;  // consider making an enum to make player only or everyyone????? Will this be run Server side??
    // Start is called before the first frame update
    private void Awake()
    {
        DontDestroyOnLoad(this);
#if UNITY_STANDALONE_WIN

        InputSettings.TargetPlatform = PlatformOptions.PC;

#endif
#if UNITY_XBOXONE
        InputSettings.TargetPlatform = PlatformOptions.XBOX;
#endif
#if UNITY_PS4
       InputSettings.TargetPlatform = PlatformOptions.PC;
#endif

        InputSettings.Controller = true;
        InputSettings.SetUp();
    }

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



    }
}
