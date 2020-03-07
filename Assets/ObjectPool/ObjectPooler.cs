using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
	[System.Serializable]
	public class Pool
	{
		public string tag;
		public GameObject prefab;
		public uint size;
	}
	public static ObjectPooler Instance;

	#region Singleton
	private void Awake()
	{
		Instance = this;
	}
	#endregion Singleton

	public List<Pool> pools;

	public Dictionary<string, Queue<GameObject>> poolDictionary;

	private void Start()
	{
		poolDictionary = new Dictionary<string, Queue<GameObject>>();

		foreach (Pool p in pools)
		{
			Queue<GameObject> objectPool = new Queue<GameObject>();
			for (int i = 0; i < p.size; i++)
			{
				GameObject obj = Instantiate(p.prefab);
				obj.SetActive(false);
				objectPool.Enqueue(obj);
			}
			poolDictionary.Add(p.tag, objectPool);
		}
	}

	public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
	{
		if (poolDictionary.ContainsKey(tag))
		{
			GameObject obj = poolDictionary[tag].Dequeue();
			obj.transform.position = position;
			obj.transform.rotation = rotation;
			obj.SetActive(true);
			obj.GetComponent<IPooledObject>().OnObjectSpawn();

			poolDictionary[tag].Enqueue(obj);
			return obj;
		}
		else
		{
			Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
			return null;
		}

	}


}
