using UnityEngine;

namespace DefaultNamespace.Relics
{
    public class InkBottle : AbstractRelic
    {
        [SerializeField] private int _cardsBeforeDraw;

        protected new void Awake()
        {
            base.Awake();
            
            EventManagerScript.Instance.StartListening(EventManagerScript.EVENT__PLAY_CARD, OnCardPlay);
            if (_resetEveryTurn)
                EventManagerScript.Instance.StartListening(EventManagerScript.EVENT__START_TURN, ResetCounter);
            if (_resetEveryCombat)
                EventManagerScript.Instance.StartListening(EventManagerScript.EVENT__START_COMBAT, ResetCounter);

            _maxCounterValue = _cardsBeforeDraw - 1;
        }

        private void OnCardPlay(object obj)
        {
            if (IncrementCounter() == 0)
            {
                Activate();
            }
        }


        protected override void Activate()
        {
            base.Activate();
            PlayerTurn.Instance.DrawCard();
        }
    }
}