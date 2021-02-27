using System.Collections.Generic;
using UnityEngine;
public interface ICombos
{
    List<AnimationCombo> ComboList { get; }

    void UnlockCombo(ComboNames Name);
    GameObject DisplayCombo();

}