using System;
using System.Collections.Generic;
using System.Linq;

public abstract class EventBase
{
    private List<Action> _actions;

    /// <summary>
    /// 场景切换时，是否移除该事件对象的订阅方法
    /// </summary>
    public bool KeepOnLevelChanging { get; protected set; }

    /// <summary>
    /// 发布事件
    /// </summary>
    public void Publish()
    {
        if (_actions == null) return;

        foreach (var action in _actions)
        {
            action();
        }
    }

    /// <summary>
    /// 订阅事件
    /// </summary>
    public void Subscribe(Action action)
    {
        if (_actions == null)
        {
            _actions = new List<Action>();
        }

        if (!_actions.Contains(action))
        {
            _actions.Add(action);
        }
    }

    /// <summary>
    /// 取消订阅
    /// </summary>
    public void Unsubscribe(Action action)
    {
        if (_actions == null)
        {
            return;
        }

        if (_actions.Contains(action))
        {
            _actions.Remove(action);
        }
    }

    /// <summary>
    /// 移除所有订阅内容
    /// </summary>
    public void Clear()
    {
        if (_actions == null)
        {
            return;
        }

        _actions.Clear();
    }
}

/// <summary>
/// 接收参数的事件模型
/// </summary>
public abstract class EventBase<T> : EventBase
{
    private List<Action<T>> _actions;

    public void Publish(T payload)
    {
        if (_actions == null) return;

        foreach (var action in _actions)
        {
            action(payload);
        }
    }

    public void Subscribe(Action<T> action)
    {
        if (_actions == null)
        {
            _actions = new List<Action<T>>();
        }

        if (!_actions.Contains(action))
        {
            _actions.Add(action);
        }
    }

    public void Unsubscribe(Action<T> action)
    {
        if (_actions == null)
        {
            return;
        }

        if (_actions.Contains(action))
        {
            _actions.Remove(action);
        }
    }

    public new void Clear()
    {
        if (_actions == null)
        {
            return;
        }

        _actions.Clear();
        base.Clear();
    }
}

public class EventService
{
    private readonly static EventService _instance = new EventService();

    // 存放事件对象的集合
    private readonly Dictionary<Type, EventBase> _eventBases = new Dictionary<Type, EventBase>();

    public static EventService Instance
    {
        get { return _instance; }
    }

    private EventService()
    {
    }

    /// <summary>
    /// 获取一个事件对象，该对象用于事件订阅或发布
    /// </summary>
    public T GetEvent<T>() where T : EventBase
    {
        Type eventType = typeof(T);
        if (!_eventBases.ContainsKey(eventType))
        {
            // 如果事件对象不存在，则创建一个该类型的对象
            T e = Activator.CreateInstance<T>();
            _eventBases.Add(eventType, e);
        }

        return (T)_eventBases[eventType];
    }

    /// <summary>
    /// 移除所有事件对象
    /// </summary>
    public void ClearAll()
    {
        foreach (EventBase e in _eventBases.Values)
        {
            e.Clear();
        }

        _eventBases.Clear();
    }

    /// <summary>
    /// 移除所有KeepOnLevelChanging标记为false的事件对象的订阅方法，并不移除该事件对象
    /// 该方法应该与Application.LoadLevel() 一起调用（如果有必要）
    /// </summary>
    public void ClearOnLevelChanging(int newLevelId)
    {
        foreach (EventBase e in _eventBases.Values.Where(e => !e.KeepOnLevelChanging))
        {
            e.Clear();
        }
    }
}