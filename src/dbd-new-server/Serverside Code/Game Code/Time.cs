using System;
using System.Collections.Generic;

namespace MushroomsUnity3DExample
{
    public class Time
    {
        public Time()
        {
        }

        private int _value;
		
        private Dictionary<Action, int> _timers = new Dictionary<Action, int>();
        private List<Action> _actions = new List<Action>();


        private List<Action> _actionsForInvoke = new List<Action>();
        public void TickSecond()
        {
            _value++;
			
            foreach (var action in _actions)
                _timers[action]--;

			
            foreach (var kvp in _timers)
                if (kvp.Value<=0)
                    _actionsForInvoke.Add(kvp.Key);
			
            foreach (var action in _actionsForInvoke)
            {
                action.Invoke();
                _actions.Remove(action);
                _timers.Remove(action);
            }
            _actionsForInvoke.Clear();
        }
        public void Invoke(Action action, int ms)
        {
            if (_actions.Contains(action)) return;
            _actions.Add(action);
            _timers.Add(action,ms);
        }
    }
}