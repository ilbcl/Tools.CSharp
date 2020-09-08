using System;
using System.Collections.Generic;
using System.Reflection;

namespace Tools
{
    [Serializable]
    public sealed class EventsBag
    {
        #region Fields

        private readonly Dictionary<Object, Delegate> _events;

        #endregion //Fields

        #region Constructors

        public EventsBag()
        {
            _events = new Dictionary<Object, Delegate>();
        }

        public EventsBag(Int32 eventsCount)
        {
            _events = new Dictionary<Object, Delegate>(eventsCount);
        }

        #endregion //Constructors

        #region Public Methods

        public void AddEvent(Object eventKey, Delegate handler)
        {
            if (eventKey == null)
            {
                return;
            }

            if (handler != null)
            {
                lock (_events)
                {
                    Delegate eventHandlers;
                    _events.TryGetValue(eventKey, out eventHandlers);
                    _events[eventKey] = Delegate.Combine(eventHandlers, handler);
                }
            }
        }

        public void RemoveEvent(Object eventKey, Delegate handler)
        {
            if (eventKey == null)
            {
                return;
            }

            if (handler != null)
            {
                lock (_events)
                {
                    Delegate eventHandlers;
                    if (_events.TryGetValue(eventKey, out eventHandlers))
                    {
                        eventHandlers = Delegate.Remove(eventHandlers, handler);
                        if (eventHandlers != null)
                        {
                            _events[eventKey] = eventHandlers;
                        }
                        else
                        {
                            _events.Remove(eventKey);
                        }
                    }
                }
            }
        }

        public void RaiseEvent(Object eventKey, Object sender, EventArgs e)
        {
            if (eventKey == null)
            {
                return;
            }

            Delegate eventHandlers;
            lock (_events)
            {
                _events.TryGetValue(eventKey, out eventHandlers);
            }

            if (eventHandlers != null)
            {
                try
                {
                    eventHandlers.DynamicInvoke(sender, e);
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            }
        }

        #endregion //Public Methods
    }
}
