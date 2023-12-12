using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : IServiceLocator
{
    public static IServiceLocator Instance { get; protected set; }


    private readonly Dictionary<Type, Service> services = new();

    public ServiceLocator()
    {
        //if (Instance != null)
        //{
        //    Debug.LogWarning("Other instance of a service locator already exists.");
            
        //    return;
        //}
        Instance = this;
    }

    public void Add(Service _service, Type _key = null)
    {
        if (_key == null) { _key = _service.GetType(); }

        if (services.ContainsKey(_key))
        {
            Debug.LogWarning($"Key: {_key} already present in the service pool.");
            return;
        }
        services.Add(_key, _service);
    }

    public void Remove(Type _key)
    {
        if (!services.ContainsKey(_key))
        {
            Debug.LogWarning($"Key: {_key} is not present in the service pool.");
            return;
        }
        services.Remove(_key);
    }

    public T Get<T>() where T : Service
    {
        Type key = typeof(T);

        if (services.ContainsKey(key))
        {
            return (T)services[key];
        }
        else
        {
            Debug.LogError($"Key: {key} did not return a valid service.");
            return default;
        }
    }

    public void OnSceneLoaded()
    {
        foreach (Service service in services.Values)
        {
            service.OnSceneLoad();
        }
    }

    public void FixedUpdate()
    {
        foreach (Service service in services.Values)
        {
            service.OnFixedUpdate();
        }
    }
}
