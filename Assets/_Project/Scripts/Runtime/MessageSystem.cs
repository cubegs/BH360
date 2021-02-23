using System;
using System.Collections.Generic;

namespace BHPanorama
{
    public class MessageSystem
    {
        public delegate void EventDelegate<T>(T e) where T : Message;

        private static Dictionary<Type, Delegate> s_TriggerDictionary = new Dictionary<Type, Delegate>();

        public static void Subscribe<T>(EventDelegate<T> del) where T : Message
        {
            if (s_TriggerDictionary.TryGetValue(typeof(T), out Delegate tempDel))
            {
                s_TriggerDictionary[typeof(T)] = Delegate.Combine(tempDel, del);
            }
            else
            {
                s_TriggerDictionary[typeof(T)] = del;
            }
        }

        public static void Unsubscribe<T>(EventDelegate<T> del) where T : Message
        {
            if (s_TriggerDictionary.ContainsKey(typeof(T)))
            {
                var currentDel = System.Delegate.Remove(s_TriggerDictionary[typeof(T)], del);

                if (currentDel == null)
                {
                    s_TriggerDictionary.Remove(typeof(T));
                }
                else
                {
                    s_TriggerDictionary[typeof(T)] = currentDel;
                }
            }
        }

        public static void Trigger(Message message)
        {
            if (s_TriggerDictionary.TryGetValue(message.GetType(), out Delegate del))
            {
                del.DynamicInvoke(message);
            }
        }
    }

    public class Message { }

    public class OpenMenuEvent : Message { }

    public class CloseMenuEvent : Message { }

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
}