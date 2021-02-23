using System;
using System.Collections.Generic;

public class MessageSystem
{
    public  delegate void EventDelegate<T>(T e) where T : Message;

    private static Dictionary<Type, Delegate> triggers = new Dictionary<Type, Delegate>();

    public static void Subscribe<T>(EventDelegate<T> del) where T : Message
    {
        if (triggers.TryGetValue(typeof(T), out Delegate tempDel))
        {
            triggers[typeof(T)] = Delegate.Combine(tempDel, del);
        }
        else
        {
            triggers[typeof(T)] = del;
        }        
    }

    public static void Unsubscribe<T>(EventDelegate<T> del) where T : Message
    {
        if (triggers.ContainsKey(typeof(T)))
        {
            var currentDel = System.Delegate.Remove(triggers[typeof(T)], del);

            if (currentDel == null)
            {
                triggers.Remove(typeof(T));
            }
            else
            {
                triggers[typeof(T)] = currentDel;
            }
        }
    }

    public static void Trigger(Message message)
    {
        if(triggers.TryGetValue(message.GetType(), out Delegate del))
        {
            del.DynamicInvoke(message);
        }
    }
}

public class Message
{

}

public class OpenMenuEvent : Message
{

}

public class CloseMenuEvent : Message
{

}

public class PointOfInterestOpenEvent : Message
{
    public PointOfInterest PointOfInterest;

    public PointOfInterestOpenEvent(PointOfInterest PointOfInterest)
    {
        this.PointOfInterest = PointOfInterest;
    }
}

public class PointOfInterestCloseEvent : Message
{
    public PointOfInterest PointOfInterest;

    public PointOfInterestCloseEvent(PointOfInterest PointOfInterest)
    {
        this.PointOfInterest = PointOfInterest;
    }
}

public class HotspotInitializeEvent : Message
{
    public Hotspot Hotspot;

    public HotspotInitializeEvent(Hotspot Hotspot)
    {
        this.Hotspot = Hotspot;
    }
}

public class StartMovingToHotspotEvent : Message
{
    public Hotspot Hotspot;

    public StartMovingToHotspotEvent(Hotspot Hotspot)
    {
        this.Hotspot = Hotspot;
    }
}

public class EndMovingToHotspotEvent : Message
{
    public Hotspot Hotspot;

    public EndMovingToHotspotEvent(Hotspot Hotspot)
    {
        this.Hotspot = Hotspot;
    }
}

public class ChangeTimeEvent : Message
{
    public bool Day;

    public ChangeTimeEvent(bool Day)
    {
        this.Day = Day;
    }
}