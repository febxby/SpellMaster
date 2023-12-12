using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IEvent
{
    IUnRegister Register(Action action);
}
public interface IUnRegister
{
    void UnRegister();
}
public struct CustomUnRegister : IUnRegister
{
    private Action mAction;
    public CustomUnRegister(Action action) => mAction = action;
    public void UnRegister()
    {
        mAction?.Invoke();
        mAction = null;
    }
}
public class Event : IEvent
{
    private Action mEvent = () => { };
    public IUnRegister Register(Action action)
    {
        mEvent += action;
        return new CustomUnRegister(() => UnRegister(action));
    }
    public void UnRegister(Action action) => mEvent -= action;

    public void Invoke() => mEvent.Invoke();
}
public class Event<T> : IEvent
{
    private Action<T> mEvent = (t) => { };
    public IUnRegister Register(Action<T> action)
    {
        mEvent += action;
        return new CustomUnRegister(() => UnRegister(action));
    }
    public void UnRegister(Action<T> action) => mEvent -= action;

    public void Invoke(T t) => mEvent.Invoke(t);
    IUnRegister IEvent.Register(Action action)
    {
        return Register((T _) => action());
    }
}
public class UnRegisterOnDestroyTrigger : MonoBehaviour
{
    private readonly HashSet<IUnRegister> mUnRegisters = new HashSet<IUnRegister>();
    public void AddUnRegister(IUnRegister unRegister)
    {
        mUnRegisters.Add(unRegister);
    }
    public void RemoveUnRegister(IUnRegister unRegister)
    {
        mUnRegisters.Remove(unRegister);
    }
    private void OnDestroy()
    {
        foreach (var unRegister in mUnRegisters)
        {
            unRegister.UnRegister();
        }
        mUnRegisters.Clear();
    }
}
public static class UnRegisterExtension
{
    public static IUnRegister UnRegisterWhenGameObjectDestroy(this IUnRegister unRegister, GameObject gameObject)
    {
        var trigger = gameObject.GetComponent<UnRegisterOnDestroyTrigger>();
        if (!trigger)
        {
            trigger = gameObject.AddComponent<UnRegisterOnDestroyTrigger>();
        }
        trigger.AddUnRegister(unRegister);
        return unRegister;
    }
}
public class MEventSystem : Singleton<MEventSystem>
{
    // Start is called before the first frame update
    private readonly Dictionary<Type, IEvent> mEvents = new();
    private T AddEvent<T>() where T : IEvent, new()
    {
        var eType = typeof(T);
        if (mEvents.TryGetValue(eType, out var e))
        {
            return (T)e;
        }
        var newEvent = new T();
        mEvents.Add(eType, newEvent);
        return newEvent;
    }
    private T GetEvent<T>() where T : IEvent
    {
        return mEvents.TryGetValue(typeof(T), out var e) ? (T)e : default;
    }

    public IUnRegister Register<T>(Action<T> action)
    {
        return AddEvent<Event<T>>().Register(action);
    }
    public void UnRegister<T>(Action<T> action)
    {
        GetEvent<Event<T>>()?.UnRegister(action);
    }
    public void Send<T>() where T : new()
    {
        GetEvent<Event<T>>()?.Invoke(new T());
    }
    public void Send<T>(T t)
    {
        GetEvent<Event<T>>()?.Invoke(t);
    }
}
