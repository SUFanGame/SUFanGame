using UnityEngine;
using StevenUniverse.FanGame.Util;

namespace StevenUniverse.FanGame.Interactions.Activities
{
    [System.Serializable]
    public class SetData : Activity
    {
        [SerializeField] private string dataGroupName;
        [SerializeField] private string dataBoolName;
        [SerializeField] private bool newStatus;

        public SetData(int id, bool allowsControl, string dataGroupName, string dataBoolName, bool newStatus)
            : base(id, allowsControl)
        {
            this.dataGroupName = dataGroupName;
            this.dataBoolName = dataBoolName;
            this.newStatus = newStatus;
        }

        public override void StartActivity()
        {
            base.StartActivity();

            GameController.Instance.CurrentPlayer.SourcePlayer.SavedData.SetDatBoolValue(dataGroupName, dataBoolName, newStatus);

            IsComplete = true;
        }
    }
}