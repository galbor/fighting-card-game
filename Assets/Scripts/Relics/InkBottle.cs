using UnityEngine;

namespace DefaultNamespace.Relics
{
    public class InkBottle : AbstractRelic
    {
        [SerializeField] private int _cardsBeforeDraw;

        private int _cardCount = 0;
        void Awake()
        {
            EventManagerScript.Instance.StartListening(EventManagerScript.EVENT__PLAY_CARD, OnCardPlay);
            if (_resetEveryTurn)
                EventManagerScript.Instance.StartListening(EventManagerScript.EVENT__START_TURN, ResetCount);
            if (_resetEveryCombat)
                EventManagerScript.Instance.StartListening(EventManagerScript.EVENT__START_COMBAT, ResetCount);
        }

        private void OnCardPlay(object obj)
        {
            _cardCount = (_cardCount + 1) % _cardsBeforeDraw;
            if (_cardCount == 0)
            {
                Activate();
            }
        }

        private void ResetCount(object obj)
        {
            _cardCount = 0;
        }


        protected override void Activate()
        {
            base.Activate();
            PlayerTurn.Instance.DrawCard();
        }
    }
}