using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System.Threading.Tasks;
using System.Linq;

namespace DreamerInc.CombatSystem
{
    public class VFXManager : MonoBehaviour
    {
        public  static VFXManager Instance;
        public TextAsset VFXList;
        List<VFXInfo> vfxInfos;
        bool VFXLoaded;
        bool PoolLoaded;
        private void Awake()
        {
            if(Instance == null)
                Instance = this;
            else
                Destroy(this.gameObject);

            loadVFX();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public void loadVFX() { 
            VFXLoaded = true;
            var lines = VFXList.text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            vfxInfos = new List<VFXInfo>();
            for (int i = 0; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');
                
                VFXInfo temp = new VFXInfo();
                temp.ID = int.Parse(parts[0]);
                temp.PoolObject = Resources.Load<GameObject>(parts[1]);
                temp.CreatePool(this.gameObject);
                vfxInfos.Add(temp);
            }

        }
        void DestoryVFXPool() {
            foreach (Transform item in transform)
            {
                Destroy(item.gameObject);
            }
        }
        public void PlayVFX(int ID, Vector3 Pos, Vector3 Rot, float DelayStart = 0.0f, float lifeTime=0.0f) {
            if (!VFXLoaded) {
                DestoryVFXPool();
                loadVFX();
            }
            GetVFX(ID).Play(Pos, Rot, DelayStart ,lifeTime);
        }
        public void PlayVFX(int ID, Vector3 Pos,  float lifeTime = 0.0f)
        {
            if (!VFXLoaded)
            {
                DestoryVFXPool();
                loadVFX();
            }
            GetVFX(ID).Play(Pos,  lifeTime);
        }
        public VFXInfo GetVFX(int id) {
            VFXInfo temp = new VFXInfo();
            foreach (var item in vfxInfos)
            {
                if (item.ID == id)
                {
                    temp = item;
                    return temp;
                }
            }
            return null;

        }


    }
    [System.Serializable]
    public class VFXInfo {
        public int ID;
        public GameObject PoolObject;
        List<GameObject> Instances;
        GameObject parent;
        public int Count =>Instances.Count;
        public bool PoolCanGrow;

        public void CreatePool(GameObject parent) {
            Instances = new List<GameObject>(); 
            this.parent = parent;
            GameObject go =GameObject.Instantiate(PoolObject, parent.transform);
            go.SetActive(false);
            Instances.Add(go);
        }
        public void GrowPool() {
            var go =GameObject.Instantiate(PoolObject, parent.transform);
            Instances.Add(go);

        }
        public async void Play(Vector3 pos, Vector3 rot, float DelayStart, float lifeTime) {

            bool played = false;
            //Start:
            for (int i = 0; i < Instances.Count ; i++)
            {
                if (!Instances[i].activeSelf)
                {
                    var vfx = Instances[i];
                    vfx.transform.SetPositionAndRotation(pos, Quaternion.Euler(rot));
                    ParticleSystem PS = vfx.GetComponent<ParticleSystem>();               
                    vfx.gameObject.SetActive(true);
                    await Task.Delay(TimeSpan.FromMilliseconds(DelayStart));
                    PS.Play(true);
                  //  TriggerPlay;
                    await Task.Delay(TimeSpan.FromSeconds(lifeTime));
                    PS.Stop(true);
                    vfx.gameObject.SetActive(false);
                    played = true;
                    break;
                }
            }
            if (!played)
            {
              GrowPool();
                var vfx = Instances.Last();
                vfx.transform.SetPositionAndRotation(pos, Quaternion.Euler(rot));
                ParticleSystem PS = vfx.GetComponent<ParticleSystem>();
                vfx.gameObject.SetActive(true);
                await Task.Delay(TimeSpan.FromMilliseconds(DelayStart));
                PS.Play(true);
                //  TriggerPlay;
                await Task.Delay(TimeSpan.FromSeconds(lifeTime));
                PS.Stop(true);
                vfx.gameObject.SetActive(false);
                played = true;
            }
        }
        /// <summary>
        /// Play Instanace of VFX at give position
        /// </summary>
        /// <param name="pos"> Position to Play VFX</param>
        /// <param name="lifeTime">Duration of VFX</param>
        public async void Play(Vector3 pos, float lifeTime = 5.0f )
        {

            bool played = false;
            //Start:
            for (int i = 0; i < Instances.Count; i++)
            {
                if (!Instances[i].activeSelf)
                {
                    var vfx = Instances[i];
                    vfx.transform.position = pos;
                    ParticleSystem PS = vfx.GetComponent<ParticleSystem>();
                    vfx.gameObject.SetActive(true);
                    PS.Play(true);
                    //  TriggerPlay;
                    await Task.Delay(TimeSpan.FromSeconds(lifeTime));
                    PS.Stop(true);
                    vfx.gameObject.SetActive(false);
                    played = true;
                    break;
                }
            }
            if (!played)
            {
                GrowPool();
                var vfx = Instances.Last();
                vfx.transform.position = pos;
                ParticleSystem PS = vfx.GetComponent<ParticleSystem>();
                vfx.gameObject.SetActive(true);
                PS.Play(true);
                //  TriggerPlay;
                await Task.Delay(TimeSpan.FromSeconds(lifeTime));
                PS.Stop(true);
                vfx.gameObject.SetActive(false);
                played = true;
            }
        }
    }
}