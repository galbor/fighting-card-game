using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Managers;
using DefaultNamespace.UI;

namespace Managers
{
	public class EventManager : Singleton<EventManager>
	{
		protected EventManager()
		{
			Init();
		} // guarantee this will be always a singleton only - can't use the constructor!

		public class FloatEvent : UnityEvent<object>
		{
		} //empty class; just needs to exist

		private const string event_prefix = "event_";

		public const string EVENT__DRAW_CARD = event_prefix + "drawCard";
		public const string EVENT__PLAY_CARD = event_prefix + "playCard";
		public const string EVENT__DISCARD_CARD = event_prefix + "dicardCard";
		public const string EVENT__EXHAUST_CARD = event_prefix + "exhaustCard";

		public const string EVENT__HIT = event_prefix + "hit";

		// public const string EVENT__TAKE_DAMAGE = event_prefix + "takeDamage";
		// public const string EVENT__DEAL_DAMAGE = event_prefix + "dealDamage";
		public const string EVENT__KILL_ENEMY = event_prefix + "killEnemy";

		public const string EVENT__REMOVE_HEALTH = event_prefix + "removeHealth";

		// public const string EVENT__BREAK_ENEMY_BLOCK = event_prefix + "breakEnemyBlock";
		// public const string EVENT__BLOCK_BROKEN = event_prefix + "breakEnemyBlock";
		public const string EVENT__PLAYER_DEATH = event_prefix + "die";
		public const string EVENT__START_TURN = event_prefix + "startTurn";
		public const string EVENT__END_TURN = event_prefix + "endTurn";
		public const string EVENT__START_COMBAT = event_prefix + "startCombat";
		public const string EVENT__END_COMBAT = event_prefix + "endCombat";

        public const string EVENT__ADD_RELIC = event_prefix + "addRelic";

        private Dictionary<string, FloatEvent> eventDictionary;

		[SerializeField] public Transform _TextParent;

		private void Init()
		{
			eventDictionary ??= new Dictionary<string, FloatEvent>();
		}

		public void StartListening(string eventName, UnityAction<object> listener)
		{
			FloatEvent thisEvent = null;
			if (eventDictionary.TryGetValue(eventName, out thisEvent))
			{
				thisEvent.AddListener(listener);
			}
			else
			{
				thisEvent = new FloatEvent();
				thisEvent.AddListener(listener);
				eventDictionary.Add(eventName, thisEvent);
			}
		}

		public void StopListening(string eventName, UnityAction<object> listener)
		{
			FloatEvent thisEvent = null;
			if (eventDictionary.TryGetValue(eventName, out thisEvent))
			{
				thisEvent.RemoveListener(listener);
			}
		}

		public void TriggerEvent(string eventName, object obj)
		{
			FloatEvent thisEvent = null;
			if (eventDictionary.TryGetValue(eventName, out thisEvent))
			{
				thisEvent.Invoke(obj);
			}
		}
	}
}