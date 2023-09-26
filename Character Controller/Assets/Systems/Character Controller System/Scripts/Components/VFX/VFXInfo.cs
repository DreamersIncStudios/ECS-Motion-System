using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace DreamersInc.CharacterControllerSys.VFX
{
    [System.Serializable]
    public class VFXInfo
    {
        public int ID;
        public GameObject PoolObject;
        List<GameObject> instances;
        private GameObject parent;
        public int Count => instances.Count;
        public bool PoolCanGrow;

        public void CreatePool(GameObject parent)
        {
            instances = new List<GameObject>();
            this.parent = parent;
            GameObject go = GameObject.Instantiate(PoolObject, parent.transform);
            go.SetActive(false);
            instances.Add(go);
        }
        public void GrowPool()
        {
            var go = GameObject.Instantiate(PoolObject, parent.transform);
            instances.Add(go);

        }
        public async void Play(Vector3 pos, Vector3 rot, float DelayStart, float lifeTime)
        {

            bool played = false;
            //Start:
            for (int i = 0; i < instances.Count; i++)
            {
                if (!instances[i].activeSelf)
                {
                    var vfx = instances[i];
                    vfx.transform.SetPositionAndRotation(pos, Quaternion.Euler(rot));
                    ParticleSystem PS = vfx.GetComponent<ParticleSystem>();
                    vfx.SetActive(true);
                    await Task.Delay(TimeSpan.FromMilliseconds(DelayStart));
                    PS.Play(true);
                    //  TriggerPlay;
                    await Task.Delay(TimeSpan.FromSeconds(lifeTime));
                    PS.Stop(true);
                    vfx.SetActive(false);
                    played = true;
                    break;
                }
            }
            if (!played)
            {
                GrowPool();
                var vfx = instances.Last();
                vfx.transform.SetPositionAndRotation(pos, Quaternion.Euler(rot));
                ParticleSystem PS = vfx.GetComponent<ParticleSystem>();
                vfx.SetActive(true);
                await Task.Delay(TimeSpan.FromMilliseconds(DelayStart));
                PS.Play(true);
                //  TriggerPlay;
                await Task.Delay(TimeSpan.FromSeconds(lifeTime));
                PS.Stop(true);
                vfx.SetActive(false);
                played = true;
            }
        }
        /// <summary>
        /// Play Instanace of VFX at give position
        /// </summary>
        /// <param name="pos"> Position to Play VFX</param>
        /// <param name="lifeTime">Duration of VFX</param>
        public async void Play(Vector3 pos, float lifeTime = 5.0f)
        {

            bool played = false;
            //Start:
            for (int i = 0; i < instances.Count; i++)
            {
                if (!instances[i].activeSelf)
                {
                    var vfx = instances[i];
                    vfx.transform.position = pos;
                    ParticleSystem PS = vfx.GetComponent<ParticleSystem>();
                    vfx.SetActive(true);
                    PS.Play(true);
                    //  TriggerPlay;
                    await Task.Delay(TimeSpan.FromSeconds(lifeTime));
                    PS.Stop(true);
                    vfx.SetActive(false);
                    played = true;
                    break;
                }
            }
            if (!played)
            {
                GrowPool();
                var vfx = instances.Last();
                vfx.transform.position = pos;
                ParticleSystem PS = vfx.GetComponent<ParticleSystem>();
                vfx.SetActive(true);
                PS.Play(true);
                //  TriggerPlay;
                await Task.Delay(TimeSpan.FromSeconds(lifeTime));
                PS.Stop(true);
                vfx.SetActive(false);
                played = true;
            }
        }
    }
}