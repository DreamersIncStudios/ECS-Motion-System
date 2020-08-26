using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OBJPooler
{
    public class ObjPoolingProj : MonoBehaviour
    {
        public static ObjPoolingProj current;
        public List<PoolObjBase> Pools;

        // Start is called before the first frame update
        private void Awake()
        {
            current = this;

        }
        void Start()
        {
            if (Pools.Count > 0)
            {
                foreach (PoolObjBase _pool in Pools)
                {
                    CreatePool(_pool);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void CreatePool(PoolObjBase pool) {
            pool.Pool = new List<GameObject>();
            pool.Base = new GameObject();
            pool.Base.name = "Sub Pool";
            pool.Base.transform.parent = this.gameObject.transform;
            for (int cnt = 0; cnt < pool.NumberOfObj; cnt++)
            {
                GameObject Obj = Instantiate(pool.OBjToPool, pool.Base.transform);
                Obj.SetActive(false);
                pool.Pool.Add(Obj);
            }

        }
        public GameObject GetPooledObjects(PoolObjBase Pooler) {
            for (int cnt = 0; cnt < Pooler.Pool.Count; cnt++) {
                if (!Pooler.Pool[cnt].activeInHierarchy)
                {
                    return Pooler.Pool[cnt];
                }
            }
            if (Pooler.WillGrow) {
                GameObject Obj = Instantiate(Pooler.OBjToPool,Pooler.Base.transform);
                Obj.SetActive(false);
                Pooler.Pool.Add(Obj);
                return Obj;
            }
            return null;
        }
    }

    [System.Serializable]
    public struct PoolObjBase {
        public string PoolName;
        public GameObject OBjToPool;
        [Range(0, 100)]
        public int NumberOfObj;
        public List<GameObject> Pool;
        public bool WillGrow;
        public GameObject Base;
    }
}
