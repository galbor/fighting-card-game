using DefaultNamespace.Utility;
using UnityEngine;

namespace DefaultNamespace.Relics
{
    /**
     * Draws a card every n cards played.
     */
    public class InkBottle : AbstractRelic
    {
        
        [SerializeField] private int _cardsBeforeDraw;

        protected new void Awake()
        {
            base.Awake();
            
            EventManager.Instance.StartListening(EventManager.EVENT__PLAY_CARD, OnCardPlay);
            if (_resetEveryTurn)
                EventManager.Instance.StartListening(EventManager.EVENT__START_TURN, ResetCounter);
            if (_resetEveryCombat)
                EventManager.Instance.StartListening(EventManager.EVENT__START_COMBAT, ResetCounter);

            _counterCycleLength = _cardsBeforeDraw;

            Description = MyUtils.ReplaceAllBrackets(Description, "draw", _cardsBeforeDraw.ToString());
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