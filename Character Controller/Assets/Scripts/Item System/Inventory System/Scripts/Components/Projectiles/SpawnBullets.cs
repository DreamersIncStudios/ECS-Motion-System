using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Dreamers.InventorySystem;
//[DisableAutoCreation]
public class SpawnBullets : ComponentSystem
{
  
 protected override void OnUpdate()
    {
        Entities.ForEach(( ShooterComponent shoot) => {
            if (shoot.Wait)
            {
                shoot.LastTimeShot -= Time.DeltaTime;
                return;
            }    
            
            if (shoot.RoundsLeftToSpawn > 0 ) {
                LocalToWorld localToWorld = GetComponentDataFromEntity<LocalToWorld>()[shoot.ShootFromHere];
                GameObject bullet = Object.Instantiate(shoot.ProjectileGameObject, localToWorld.Position+shoot.Offset, localToWorld.Rotation);
                bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * shoot.NormalSpeed;
                if (shoot.HasShotBeenCharge) {
                    bullet.transform.localScale *= 3;
                    shoot.HasShotBeenCharge = false;
                }

                Object.Destroy(bullet, 10);
                shoot.RoundsLeftToSpawn--;
                shoot.LastTimeShot +=60.0f/(float)shoot.RoundsPerMin;
            }

        });
    }

   
}
