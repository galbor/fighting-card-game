using UnityEngine;

namespace DefaultNamespace.Relics
{
    /**
     * inflicts _bleed bleed on the attacker body part when the player attacks with _bodyPart
     */
    public class SpikedGauntlet : AbstractRelic
    {
        [SerializeField] private Person.BodyPartEnum _bodyPart;
        [SerializeField] private int _bleed;
        
        protected new void Awake()
        {
            base.Awake();
            
            EventManagerScript.Instance.StartListening(EventManagerScript.EVENT__HIT, InflictBleed);
            
            Description = MyUtils.ReplaceFirstOccurrence(Description, "bleed", _bleed.ToString());
        }

        /**
         * inflicts bleed if player is the attacker and the attacking part is _bodyPart
         */
        private void InflictBleed(object obj)
        {
            var attack = (EventManagerScript.AttackStruct)obj;
            if (!attack._playerAttacker || attack._playerPart != _bodyPart) return;
            attack._enemy.Bleed(attack._enemyPart, _bleed);
            
            Activate();
        }
    }
}