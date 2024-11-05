using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.UI
{
    public class PlannedAttackDisplay : MonoBehaviour
    {
        [SerializeField] private Image _attackingPart;
        [SerializeField] private Image _affectedPart;
        [SerializeField] private TMP_Text _damage;

        /**
         * sets the displayed damage
         */
        public void SetDamage(int damage)
        {
            _damage.text = damage.ToString();
        }

        /**
         * sets the display sprite of the attacking part
         */
        public void SetAttackingPart(Sprite attackingPart)
        {
            _attackingPart.sprite = attackingPart;
        }

        /**
         * sets the display sprite of the attacking part
         */
        public void SetAffectedPart(Sprite affectedPart)
        {
            _affectedPart.sprite = affectedPart;
        }
    }
}