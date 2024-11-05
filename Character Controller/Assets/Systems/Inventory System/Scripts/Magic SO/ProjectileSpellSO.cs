
using DreamersInc.MagicSystem;
using Stats.Entities;
using Unity.Entities;
using UnityEngine;

namespace Dreamers.InventorySystem
{
    public class ProjectileSpellSO : SpellSO
    {
        [SerializeField] private float speed=1.0f;
        [SerializeField] GameObject vfxPrefab;
        private Transform spawnAt;
       
        
        public override void Deactivate(WeaponSO weaponSo, BaseCharacterComponent player, Entity entity)
        {
            throw new System.NotImplementedException();
        }


        
        public override void Activate(WeaponSO weaponSO, BaseCharacterComponent player, Entity entity)
        {
            if (player.CurMana <ManaCost) return; //Todo create failed spell?
            player.AdjustMana(ManaCost);
                spawnAt =player.GORepresentative.GetComponentInChildren<VFXSpawnPoint>().transform;
            var spellEntity = new SpellBuilder(name)
                .WithVFX(vfxPrefab, spawnAt, 3)
                .WithDamage(player)
                .WithTrajectory(speed)
                .Build();
            
        }
    }
}