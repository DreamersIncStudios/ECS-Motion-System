using System.Collections.Generic;
using Unity.Entities;
public interface ICombos
{
    List<AnimationCombo> ComboList { get; }

    void Setup(Entity entity);
    void UnlockCombo(ComboNames Name);
}