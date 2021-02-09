using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Combo {
    public bool Unlocked;
    [HideInInspector] public bool AddedToAnimator;
    public List<AnimState> AnimStates;
}

[System.Serializable]
public struct AnimState {
    public InputKeys GetButtom;
    public AnimationClip anim;
    public float ExitTime;
}
public enum InputKeys { }