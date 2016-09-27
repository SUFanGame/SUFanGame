using UnityEngine;

namespace StevenUniverse.FanGame.Interactions
{
    [System.Serializable]
    public class ActivityBlock : Interaction
    {
        [SerializeField] private int[] activityIDs;
        [SerializeField] private int nextInteractionID;

        public ActivityBlock(int interactionID, int[] activityIDs, int nextInteractionID) : base(interactionID)
        {
            this.activityIDs = activityIDs;
            this.nextInteractionID = nextInteractionID;
        }

        public int[] ActivityIDs
        {
            get { return activityIDs; }
        }

        public override int GetNextInteractionID()
        {
            return nextInteractionID;
        }
    }
}