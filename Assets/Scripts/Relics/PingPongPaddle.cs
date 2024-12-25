using cards;
using DefaultNamespace.Utility;
using Managers;
using UnityEngine;

namespace Relics
{
    /**
     * deal _damage extra damage when attacking from opposite sides
     */
    public class PingPongPaddle : AbstractRelic{

        [SerializeField] private int _damage;

        private Person.SideEnum _lastSide;



        protected new void Awake()
        {
            base.Awake();
            
            EventManager.Instance.StartListening(EventManager.EVENT__HIT, OnHit);
            if (_resetEveryTurn)
                EventManager.Instance.StartListening(EventManager.EVENT__START_TURN, ResetSide);
            if (_resetEveryCombat)
                EventManager.Instance.StartListening(EventManager.EVENT__START_COMBAT, ResetSide);

            ResetSide(null);
            Description = MyUtils.ReplaceAllBrackets(Description, "damage", _damage.ToString());
        }

        private void OnHit(object obj)
        {
            BasicAttackCard.AttackStruct attack = (BasicAttackCard.AttackStruct)obj;
            if (!attack._playerAttacker) return;
            Person.SideEnum prevSide = _lastSide;
            _lastSide = Person.GetSide(attack._playerPart);

            if (!MyUtils.OppositeSides(prevSide, _lastSide)) return;
            attack._enemy.GetHealthBar(attack._enemyPart).RemoveHealth(_damage);
        }


        private void ResetSide(object obj)
        {
            _lastSide = Person.SideEnum.NONE;
        }
    }
}