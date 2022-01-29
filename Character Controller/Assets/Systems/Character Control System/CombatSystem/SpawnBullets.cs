using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Dreamers.InventorySystem;

namespace Dreamers.ProjectileSystem
{
    public class SpawnBullets : ComponentSystem
    {

        protected override void OnUpdate()
        {
            Entities.ForEach((ref ShooterComponent shoot, ref LocalToWorld localToWorld) =>
            {
                if (shoot.Wait)
                {
                    shoot.LastTimeShot -= Time.DeltaTime;
                    return;
                }

                if (shoot.RoundsLeftToSpawn > 0)
                {
                    GameObject bullet = Object.Instantiate(shoot.Projectile.GO, localToWorld.Position + (localToWorld.Forward * shoot.Offset), localToWorld.Rotation);
                    bullet.GetComponent<Rigidbody>().velocity =localToWorld.Forward * shoot.NormalSpeed;
                    if (shoot.HasShotBeenCharge)
                    {
                        bullet.transform.localScale *= 3;
                        shoot.HasShotBeenCharge = false;
                    }

                    Object.Destroy(bullet, 10);
                    shoot.RoundsLeftToSpawn--;
                    shoot.LastTimeShot += 60.0f / (float)shoot.RateOfFire;
                }

            });
        }


    }
}