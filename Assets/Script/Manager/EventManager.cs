using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{

    private volatile static EventManager instance = null;
    private static readonly object lockHelper = new object();

    private EventManager()
    {
    }

    public static EventManager GetInstance
    {
        get
        {
            if (instance == null)
            {
                lock (lockHelper)
                {
                    if (instance == null)
                    {
                        instance = new EventManager();
                    }
                }
            }
            return instance;
        }
    }

    public delegate void EventHandlerFunction();
    public delegate void DataEventHandlerFunction(object e);
    private Dictionary<int, List<EventHandlerFunction>> listners = new Dictionary<int, List<EventHandlerFunction>>();
    private Dictionary<int, List<DataEventHandlerFunction>> dataListners = new Dictionary<int, List<DataEventHandlerFunction>>();

    /// <summary>
    ///  Add Listener's Without Parameters
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="handler"></param>
    public void addEventListener(string eventName, EventHandlerFunction handler)
    {
        addEventListener(eventName.GetHashCode(), handler, eventName);
    }
    private void addEventListener(int eventID, EventHandlerFunction handler, string eventGraphName)
    {

        if (listners.ContainsKey(eventID))
        {
            listners[eventID].Add(handler);
        }
        else
        {
            List<EventHandlerFunction> handlers = new List<EventHandlerFunction>();
            handlers.Add(handler);
            listners.Add(eventID, handlers);
        }
    }

    /// <summary>
    /// Add Listener's With Parameters
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="handler"></param>
    public void addEventListener(string eventName, DataEventHandlerFunction handler)
    {
        addEventListener(eventName.GetHashCode(), handler, eventName);
    }
    private void addEventListener(int eventID, DataEventHandlerFunction handler, string eventGraphName)
    {
        if (dataListners.ContainsKey(eventID))
        {
            dataListners[eventID].Add(handler);
        }
        else
        {
            List<DataEventHandlerFunction> handlers = new List<DataEventHandlerFunction>();
            handlers.Add(handler);
            dataListners.Add(eventID, handlers);
        }
    }

    //--------------------------------------
    // REMOVE LISTENER'S
    //--------------------------------------

    public void removeEventListener(string eventName, EventHandlerFunction handler)
    {
        removeEventListener(eventName.GetHashCode(), handler, eventName);
    }
    private void removeEventListener(int eventID, EventHandlerFunction handler, string eventGraphName)
    {
        if (listners.ContainsKey(eventID))
        {
            List<EventHandlerFunction> handlers = listners[eventID];
            handlers.Remove(handler);

            if (handlers.Count == 0)
            {
                listners.Remove(eventID);
            }
        }
    }

    public void removeEventListener(string eventName, DataEventHandlerFunction handler)
    {
        removeEventListener(eventName.GetHashCode(), handler, eventName);
    }

    private void removeEventListener(int eventID, DataEventHandlerFunction handler, string eventGraphName)
    {

        if (dataListners.ContainsKey(eventID))
        {
            List<DataEventHandlerFunction> handlers = dataListners[eventID];
            handlers.Remove(handler);

            if (handlers.Count == 0)
            {
                dataListners.Remove(eventID);
            }
        }
    }

    //--------------------------------------
    // DISPATCH 
    //--------------------------------------

    public void dispatch(string eventName)
    {
        dispatch(eventName.GetHashCode(), null, eventName);
    }


    public void dispatch(string eventName, object data)
    {
        dispatch(eventName.GetHashCode(), data, eventName);
    }
    private void dispatch(int eventID, object data, string eventName)
    {     
        if (dataListners.ContainsKey(eventID))
        {
            List<DataEventHandlerFunction> handlers = dataListners[eventID];
            int len = handlers.Count;
            for (int i = 0; i < len; i++)
            {
                handlers[i](data);
            }
        }

        if (listners.ContainsKey(eventID))
        {
            List<EventHandlerFunction> handlers = listners[eventID];
            int len = handlers.Count;
            for (int i = 0; i < len; i++)
            {
                handlers[i]();
            }
        }
    }

    //--------------------------------------
    // CLEAR
    //--------------------------------------
    public void clearEvents()
    {
        listners.Clear();
        dataListners.Clear();
    }


}