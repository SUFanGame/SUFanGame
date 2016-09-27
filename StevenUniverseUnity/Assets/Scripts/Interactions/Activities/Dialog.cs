using UnityEngine;
using StevenUniverse.FanGame.UI;
using StevenUniverse.FanGame.Util;

namespace StevenUniverse.FanGame.Interactions.Activities
{
    [System.Serializable]
    public class Dialog : Activity
    {
        //Instance variables
        [SerializeField] private string currentMessage;
        [SerializeField] private string currentSpeaker;

        //Instance references
        private DialogUIController dialogUIController;

        public Dialog(int id, bool allowsControl, string message, string speaker) : base(id, allowsControl)
        {
            CurrentMessage = message;
            CurrentSpeaker = speaker;
        }

        public override void StartActivity()
        {
            base.StartActivity();

            GameController.Instance.ToggleDialogUI(true);
            dialogUIController = GameController.Instance.DialogUIController;
        }

        public override void UpdateActivity()
        {
            base.UpdateActivity();

            if (!IsComplete && Input.GetKeyDown(KeyCode.P))
            {
                if (!dialogUIController.FinishedPrinting)
                {
                    dialogUIController.SkipToEnd();
                }
                else
                {
                    IsComplete = true;
                    GameController.Instance.ToggleDialogUI(false);
                }
            }
        }

        public string CurrentMessage
        {
            get { return currentMessage; }
            private set { currentMessage = value; }
        }

        public string CurrentSpeaker
        {
            get { return currentSpeaker; }
            private set { currentSpeaker = value; }
        }
    }
}