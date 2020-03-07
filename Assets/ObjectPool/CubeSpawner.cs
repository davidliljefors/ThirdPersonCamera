using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    ObjectPooler pooler;
    private void Start()
    {
        pooler = ObjectPooler.Instance;
    }

    private void FixedUpdate()
    {
        ObjectPooler.Instance.SpawnFromPool("cube", transform.position, Quaternion.identity);
    }
}
