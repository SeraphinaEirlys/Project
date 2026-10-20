using System;
using System.Collections.Generic;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> services =
        new Dictionary<Type, object>();

    public static void Register<T>(T service) where T : class
    {
        if (service == null)
            return;

        services[typeof(T)] = service;
    }

    public static T Get<T>() where T : class
    {
        if (services.TryGetValue(typeof(T), out object service))
            return service as T;

        return null;
    }
}