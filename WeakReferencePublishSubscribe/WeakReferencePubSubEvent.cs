using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace WeakReferencePublishSubscribe
{
    public abstract class WeakReferencePubSubEvent
    {
        private readonly ConditionalWeakTable<object, List<Action>>
            _conditionalWeakTable = new ConditionalWeakTable<object, List<Action>>();

        public void Publish()
        {
            foreach ((_, List<Action> actions) in _conditionalWeakTable)
            foreach (Action action in actions)
                action.Invoke();
        }

        public bool Remove(object subscriber)
        {
            return _conditionalWeakTable.Remove(subscriber);
        }

        public void RemoveAll()
        {
            _conditionalWeakTable.Clear();
        }

        public void Subscribe(object subscriber, Action action)
        {
            MethodInfo methodInfo = action.Method;
            if (methodInfo.IsStatic)
                throw new InvalidOperationException("无法为静态委托添加订阅");
            if (_conditionalWeakTable.TryGetValue(subscriber, out var list))
                list.Add(action);
            else
                _conditionalWeakTable.Add(subscriber, new List<Action> {action});
        }
    }

    public abstract class WeakReferencePubSubEvent<T>
    {
        private readonly ConditionalWeakTable<object, List<Action<T>>>
            _conditionalWeakTable = new ConditionalWeakTable<object, List<Action<T>>>();

        public void Publish(T arg)
        {
            foreach ((_, List<Action<T>> actions) in _conditionalWeakTable)
            foreach (Action<T> action in actions)
                action.Invoke(arg);
        }

        public bool Remove(object subscriber)
        {
            return _conditionalWeakTable.Remove(subscriber);
        }

        public void RemoveAll()
        {
            _conditionalWeakTable.Clear();
        }

        public void Subscribe(object subscriber, Action<T> action)
        {
            MethodInfo methodInfo = action.Method;
            if (methodInfo.IsStatic)
                throw new InvalidOperationException("无法为静态委托添加订阅");
            if (_conditionalWeakTable.TryGetValue(subscriber, out var list))
                list.Add(action);
            else
                _conditionalWeakTable.Add(subscriber, new List<Action<T>> {action});
        }
    }
}