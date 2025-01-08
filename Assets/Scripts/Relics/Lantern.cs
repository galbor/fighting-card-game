using Managers;

namespace Relics
{
    /**
     * adds 1 energy at the start of combat
     */
    public class Lantern : AbstractRelic
    {
        private bool _started;
        
        protected new void Awake()
        {
            base.Awake();
            EventManager.Instance.StartListening(EventManager.EVENT__START_COMBAT, obj => _started = true);
            EventManager.Instance.StartListening(EventManager.EVENT__START_TURN, obj => { if (_started) Activate(); });
        }
        
        protected override void Activate(object obj = null)
        {
            base.Activate();
            PlayerTurn.Instance.Energy++;
            _started = false;
        }
    }
}