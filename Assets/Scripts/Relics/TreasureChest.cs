using Managers;

namespace DefaultNamespace.Relics
{
    /**
     * gets a relic after every combat
     */
    public class TreasureChest : AbstractRelic
    {
        protected new void Awake()
        {
            base.Awake();
            
            EventManager.Instance.StartListening(EventManager.EVENT__END_COMBAT, GetRelic);
        }

        private void GetRelic(object obj)
        {
            RelicManager.Instance.AddRandomUnseenRelic();
        }
    }
}