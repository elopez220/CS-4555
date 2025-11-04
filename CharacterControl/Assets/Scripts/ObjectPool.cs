using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public Queue<GameObject> pool = new Queue<GameObject>();
    public GameObject objectToPool;
    TrailRenderer trail;

    public GameObject GetObject()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();

            trail = obj.GetComponent<TrailRenderer>();
            if (trail)
            {
                trail.Clear();
            }
            obj.transform.position = Vector3.zero;
            obj.transform.rotation = Quaternion.identity;
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            obj.SetActive(true);
            return obj;
        }

        return Instantiate(objectToPool);
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}