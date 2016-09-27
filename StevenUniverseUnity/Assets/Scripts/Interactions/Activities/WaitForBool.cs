using UnityEngine;
using StevenUniverse.FanGame.Entities;
using StevenUniverse.FanGame.Util;

namespace StevenUniverse.FanGame.Interactions.Activities
{
    [System.Serializable]
    public class WaitForBool : Activity
    {
        [SerializeField] private string dataGroupName;
        [SerializeField] private string dataBoolName;
        [SerializeField] private bool targetValue;

        private Player player;

        public WaitForBool(int id, bool allowsControl, string dataGroupName, string dataBoolName, bool targetValue)
            : base(id, allowsControl)
        {
            this.dataGroupName = dataGroupName;
            this.dataBoolName = dataBoolName;
            this.targetValue = targetValue;
        }

        public override void StartActivity()
        {
            base.StartActivity();

            player = GameController.Instance.CurrentPlayer.SourcePlayer;

            EvaluateCompleteness();
        }

        public override void UpdateActivity()
        {
            base.UpdateActivity();
            EvaluateCompleteness();
        }

        private void EvaluateCompleteness()
        {
            if (player.SavedData.GetDataBoolValue(dataGroupName, dataBoolName) == targetValue)
            {
                IsComplete = true;
            }
        }
    }
}