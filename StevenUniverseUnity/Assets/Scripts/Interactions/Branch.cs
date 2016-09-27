using UnityEngine;
using StevenUniverse.FanGame.Util.Logic;

namespace StevenUniverse.FanGame.Interactions
{
    [System.Serializable]
    public class Branch : Interaction
    {
        [SerializeField]
        private Conditional branchCondition;
        [SerializeField]
        private int trueNextInteractionID;
        [SerializeField]
        private int falseNextInteractionID;

        public Branch(int interactionID, Conditional branchCondition, int trueNextInteractionID, int falseNextInteractionID) : base(interactionID)
        {
            this.branchCondition = branchCondition;
            this.trueNextInteractionID = trueNextInteractionID;
            this.falseNextInteractionID = falseNextInteractionID;
        }

        public override int GetNextInteractionID()
        {
            return branchCondition.CheckStatus() ? trueNextInteractionID : falseNextInteractionID;
        }
    }
}