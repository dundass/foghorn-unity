using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(this);
            return;
        }
        Instance = GetComponent<T>();
    }
}