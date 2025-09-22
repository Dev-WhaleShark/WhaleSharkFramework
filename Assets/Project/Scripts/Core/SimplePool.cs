using System.Collections.Generic;
using UnityEngine;

namespace WhaleShark.Core
{
    public interface IPoolable
    {
        void OnSpawned();
        void OnDespawned();
    }

    public class SimplePool : MonoBehaviour
    {
        [SerializeField] GameObject prefab;
        [SerializeField] int warmCount = 8;
        readonly Queue<GameObject> q = new Queue<GameObject>();

        void Awake()
        {
            WarmUp(warmCount);
        }

        public void WarmUp(int count)
        {
            for(int i = 0; i < count; i++)
            {
                var go = Instantiate(prefab, transform);
                go.SetActive(false);
                q.Enqueue(go);
            }
        }

        public GameObject Spawn(Vector3 pos, Quaternion rot)
        {
            var go = q.Count > 0 ? q.Dequeue() : Instantiate(prefab, transform);
            go.transform.SetPositionAndRotation(pos, rot);
            go.SetActive(true);
            go.GetComponent<IPoolable>()?.OnSpawned();
            return go;
        }

        public void Despawn(GameObject go)
        {
            go.GetComponent<IPoolable>()?.OnDespawned();
            go.SetActive(false);
            go.transform.SetParent(transform, false);
            q.Enqueue(go);
        }
    }
}