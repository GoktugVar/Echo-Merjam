using UnityEngine;
using System.Collections.Generic;

public class PoolManager : Singleton<PoolManager>
{
    public List<GameObject> objectPrefabs = new();
    public List<GameObject>[] pooledObjects;
    public List<int> amountToBuffer = new();
    public int defaultBufferAmount = 3;
    protected GameObject containerObject;

    [SerializeField] int next_index;
    public override void Awake()
    {
        base.Awake();
        next_index = Random.Range(0, objectPrefabs.Count);
        StartPool();
    }

    #region[Pool]
    void StartPool()
    {
        containerObject = new GameObject("ObjectPool");

        pooledObjects = new List<GameObject>[objectPrefabs.Count];

        int i = 0;
        foreach (GameObject objectPrefab in objectPrefabs)
        {
            objectPrefab.SetActive(false);
            pooledObjects[i] = new List<GameObject>();

            int bufferAmount;

            if (i < amountToBuffer.Count) bufferAmount = amountToBuffer[i];
            else
                bufferAmount = defaultBufferAmount;

            for (int n = 0; n < bufferAmount; n++)
            {
                GameObject newObj = Instantiate(objectPrefab) as GameObject;
                newObj.name = objectPrefab.name;
                Despawn(newObj);
            }

            i++;
            objectPrefab.SetActive(true);
        }
    }

    public GameObject Spawn(string objectType, bool onlyPooled)
    {
        for (int i = 0; i < objectPrefabs.Count; i++)
        {
            GameObject prefab = objectPrefabs[i];
            if (prefab.name == objectType)
            {

                if (pooledObjects[i].Count > 0)
                {
                    GameObject pooledObject = pooledObjects[i][0];
                    pooledObjects[i].RemoveAt(0);
                    pooledObject.transform.parent = null;
                    pooledObject.SetActive(true);

                    return pooledObject;

                }
                else if (!onlyPooled)
                {
                    return Instantiate(objectPrefabs[i]) as GameObject;
                }

                break;

            }
        }

        return null;
    }

    public GameObject Spawn(string objectType, Vector3 objectPosition, Quaternion objectRotation, bool onlyPooled)
    {
        for (int i = 0; i < objectPrefabs.Count; i++)
        {
            GameObject prefab = objectPrefabs[i];
            if (prefab.name == objectType)
            {
                if (pooledObjects[i].Count > 0)
                {
                    GameObject pooledObject = pooledObjects[i][0];
                    pooledObjects[i].RemoveAt(0);
                    if (pooledObject.GetComponent<RectTransform>() != null)
                    {
                        pooledObject.GetComponent<RectTransform>().SetParent(containerObject.transform, false);
                    }
                    else
                    {
                        pooledObject.transform.parent = containerObject.transform;
                    }
                    pooledObject.transform.SetPositionAndRotation(objectPosition, objectRotation);
                    pooledObject.SetActive(true);

                    return pooledObject;

                }
                else if (!onlyPooled)
                {
                    GameObject pooledObject = Instantiate(prefab);
                    pooledObject.name = prefab.name;
                    if (pooledObject.GetComponent<RectTransform>() != null)
                    {
                        pooledObject.GetComponent<RectTransform>().SetParent(containerObject.transform, false);
                    }
                    else
                    {
                        pooledObject.transform.parent = containerObject.transform;
                    }
                    pooledObject.transform.SetPositionAndRotation(objectPosition, objectRotation);

                    return pooledObject;
                }

                break;

            }
        }

        return null;
    }

    public void Despawn(GameObject obj)
    {
        for (int i = 0; i < objectPrefabs.Count; i++)
        {
            if (objectPrefabs[i].name == obj.name)
            {
                obj.SetActive(false);
                if (obj.GetComponent<RectTransform>() != null)
                {
                    obj.GetComponent<RectTransform>().SetParent(containerObject.transform, false);
                }
                else
                {
                    obj.transform.parent = containerObject.transform;
                }
                pooledObjects[i].Add(obj);
                return;
            }
        }
    }
    #endregion
}