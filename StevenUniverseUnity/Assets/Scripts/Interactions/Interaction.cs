using UnityEngine;

namespace StevenUniverse.FanGame.Interactions
{
    [System.Serializable]
    public abstract class Interaction
    {
        [SerializeField] private int interactionID;

        public Interaction(int interactionID)
        {
            this.interactionID = interactionID;
        }

        public int InteractionID
        {
            get { return interactionID; }
        }

        public abstract int GetNextInteractionID();
    }
}