﻿using UnityEngine;
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

		public const string EVENT__DRAW_CARD = "event_drawCard";
		public const string EVENT__PLAY_CARD = "event_playCard";
		public const string EVENT__DISCARD_CARD = "event_dicardCard";
		public const string EVENT__EXHAUST_CARD = "event_exhaustCard";

		public const string EVENT__HIT = "event_hit";

		// public const string EVENT__TAKE_DAMAGE = "event_takeDamage";
		// public const string EVENT__DEAL_DAMAGE = "event_dealDamage";
		public const string EVENT__KILL_ENEMY = "event_killEnemy";

		public const string EVENT__REMOVE_HEALTH = "event_removeHealth";

		// public const string EVENT__BREAK_ENEMY_BLOCK = "event_breakEnemyBlock";
		// public const string EVENT__BLOCK_BROKEN = "event_breakEnemyBlock";
		public const string EVENT__PLAYER_DEATH = "event_die";
		public const string EVENT__START_TURN = "event_startTurn";
		public const string EVENT__END_TURN = "event_endTurn";
		public const string EVENT__START_COMBAT = "event_startCombat";
		public const string EVENT__END_COMBAT = "event_endCombat";

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