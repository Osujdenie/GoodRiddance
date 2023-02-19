using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPool<T> : MonoBehaviour where T : Component
{
    [SerializeField] private T platformPrefab;

    private Queue<T> platformShots = new Queue<T>();
    public static ObjectPool<T> Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public T Get()
    {
        if (platformShots.Count == 0)
        {
            AddShots(1);
        }
        return platformShots.Dequeue();
    }

    private void AddShots(int count)
    {
        for (int i = 0; i < count; i++)
        {
            T platformShot = Instantiate(platformPrefab);
            platformShot.gameObject.SetActive(false);
            platformShots.Enqueue(platformShot);
        }
    }

    public void ReturnToPull(T platformShot)
    {
        platformShot.gameObject.SetActive(false);
        platformShots.Enqueue(platformShot);
    }
}
