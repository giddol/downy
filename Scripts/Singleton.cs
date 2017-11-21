using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{

    private static T _intance = null;
    public static T Instance { get { return _intance; } }

    void Awake()
    {
        _intance = GetComponent<T>();
    }
}
