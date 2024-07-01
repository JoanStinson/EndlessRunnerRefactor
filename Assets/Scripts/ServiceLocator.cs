using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    private static ServiceLocator _instance;
    public static ServiceLocator Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("ServiceLocator");
                _instance = go.AddComponent<ServiceLocator>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private Dictionary<Type, object> _services = new Dictionary<Type, object>();

    public void AddService<T>(T service)
    {
        var type = typeof(T);
        if (!_services.ContainsKey(type))
        {
            _services[type] = service;
        }
    }

    public T GetService<T>()
    {
        var type = typeof(T);
        if (_services.TryGetValue(type, out var service))
        {
            return (T)service;
        }
        else
        {
            Debug.LogError($"Service of type {type} not found.");
            return default;
        }
    }
}
