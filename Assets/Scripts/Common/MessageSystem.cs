using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MessageSystem : Singleton<MessageSystem>
{
    private static Dictionary<string, Action> _eventDictionary = new Dictionary<string, Action>();
    private static Dictionary<string, object> _eventOneParamDictionary = new Dictionary<string, object>();
    private static Dictionary<string, object> _eventTwoParamDictionary = new Dictionary<string, object>();
    private static Dictionary<string, object> _eventThreeParamDictionary = new Dictionary<string, object>();

    public static void StartListening(string eventName, Action listener)
    {

        if (!_eventDictionary.ContainsKey(eventName))
        {
            Action thisEvent = null;
            _eventDictionary.Add(eventName, thisEvent);
        }
        _eventDictionary[eventName] += listener;


    }

    public static void StopListening(string eventName, Action listener)
    {
        if (_eventDictionary.ContainsKey(eventName))
        {
            _eventDictionary[eventName] -= listener; ;
        }

    }

    public static void TriggerEvent(string eventName)
    {
        if (_eventDictionary.ContainsKey(eventName))
        {
            if (_eventDictionary[eventName] != null)
                _eventDictionary[eventName].Invoke();
        }

    }

    #region one param
    public static void StartListening<A>(string eventName, Action<A> listener)
    {
        if (!_eventOneParamDictionary.ContainsKey(eventName))
        {
            Action<A> thisEvent = null;
            _eventOneParamDictionary.Add(eventName, thisEvent);
        }
        var temp = _eventOneParamDictionary[eventName] as Action<A>;
        temp += listener;
        _eventOneParamDictionary[eventName] = temp;
    }

    public static void StopListening<A>(string eventName, Action<A> listener)
    {
        if (_eventOneParamDictionary.ContainsKey(eventName))
        {
            var temp = _eventOneParamDictionary[eventName] as Action<A>;
            temp -= listener; ;
            _eventOneParamDictionary[eventName] = temp;
        }
    }

    public static void TriggerEvent<A>(string eventName, A param)
    {
        if (_eventOneParamDictionary.ContainsKey(eventName))
        {
            var temp = _eventOneParamDictionary[eventName] as Action<A>;
            if (temp == null)
            {
                //Debug.LogError(string.Format("Don't have receive {0} with type {1}", eventName, typeof(A)));
            }
            else
            {
                temp.Invoke(param);
            }
        }

    }
    #endregion one param

    #region two param
    public static void StartListening<A, B>(string eventName, Action<A, B> listener)
    {
        if (!_eventTwoParamDictionary.ContainsKey(eventName))
        {
            Action<A, B> thisEvent = null;
            _eventTwoParamDictionary.Add(eventName, thisEvent);
        }
        var temp = _eventTwoParamDictionary[eventName] as Action<A, B>;
        temp += listener;
        _eventTwoParamDictionary[eventName] = temp;
    }

    public static void StopListening<A, B>(string eventName, Action<A, B> listener)
    {
        if (_eventTwoParamDictionary.ContainsKey(eventName))
        {
            var temp = _eventTwoParamDictionary[eventName] as Action<A, B>;
            temp -= listener; ;
            _eventTwoParamDictionary[eventName] = temp;
        }
    }

    public static void TriggerEvent<A, B>(string eventName, A param1, B param2)
    {
        if (_eventTwoParamDictionary.ContainsKey(eventName))
        {
            var temp = _eventTwoParamDictionary[eventName] as Action<A, B>;
            if (temp == null)
            {
                //Debug.LogError(string.Format("Don't have receive {0} with type {1},{2}", eventName, typeof(A), typeof(B)));
            }
            else
            {
                temp.Invoke(param1, param2);
            }
        }

    }
    #endregion two param

    #region three param
    public static void StartListening<A, B, C>(string eventName, Action<A, B, C> listener)
    {
        if (!_eventThreeParamDictionary.ContainsKey(eventName))
        {
            Action<A, B, C> thisEvent = null;
            _eventThreeParamDictionary.Add(eventName, thisEvent);
        }
        var temp = _eventThreeParamDictionary[eventName] as Action<A, B, C>;
        temp += listener;
        _eventThreeParamDictionary[eventName] = temp;
    }

    public static void StopListening<A, B, C>(string eventName, Action<A, B, C> listener)
    {
        if (_eventThreeParamDictionary.ContainsKey(eventName))
        {
            var temp = _eventThreeParamDictionary[eventName] as Action<A, B, C>;
            temp -= listener; ;
            _eventThreeParamDictionary[eventName] = temp;
        }
    }

    public static void TriggerEvent<A, B, C>(string eventName, A param1, B param2, C param3)
    {
        if (_eventThreeParamDictionary.ContainsKey(eventName))
        {
            var temp = _eventThreeParamDictionary[eventName] as Action<A, B, C>;
            if (temp == null)
            {
                //Debug.LogError(string.Format("Don't have receive {0} with type {1},{2},{3}", eventName, typeof(A), typeof(B), typeof(C)));
            }
            else
            {
                temp.Invoke(param1, param2, param3);
            }
        }
    }
    #endregion three param

}
