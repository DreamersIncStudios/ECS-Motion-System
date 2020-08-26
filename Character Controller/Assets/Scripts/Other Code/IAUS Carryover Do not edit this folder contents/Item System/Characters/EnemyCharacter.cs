using UnityEngine;
namespace Stats
{
    public class EnemyCharacter : PlayerCharacter
    {
        public uint EXPgained;
       [SerializeField] public bool Alive { get { return CurHealth > 0.0f; } }
        bool death = false;

        public void OnDeath(float deathDelay) {
            if (!death)
            {
                Destroy(this.gameObject, deathDelay);
                death = true;
            }
        }

    }
}
