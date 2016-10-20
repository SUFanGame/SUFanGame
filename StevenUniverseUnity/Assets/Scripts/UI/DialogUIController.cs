using UnityEngine;
using UnityEngine.UI;
using StevenUniverse.FanGame.Extensions;

// This class needs heavy overhaul to work with new Support classes

namespace StevenUniverse.FanGame.UI
{
    public class DialogUIController : MonoBehaviour
    {
        private Text currentDialogText;
        private Text currentSpeakerText;
        private int charCounter;
        private bool finishedPrinting;
        private bool speakerAssigned;

        // Use this for initialization
        void Start()
        {
        }

        protected void OnEnable()
        {
            if (currentDialogText == null)
            {
                currentDialogText =
                    gameObject.FindChildWithName("Dialog Panel")
                        .FindChildWithName("Current Dialog")
                        .GetComponent<Text>();
            }
            if (currentSpeakerText == null)
            {
                currentSpeakerText =
                    gameObject.FindChildWithName("Speaker Panel")
                        .FindChildWithName("Current Speaker")
                        .GetComponent<Text>();
            }
            Reset();
        }

        //public void SkipToEnd()
        //{
        //    charCounter = CurrentDialog.CurrentMessage.Length;
        //}

        public bool FinishedPrinting
        {
            get { return finishedPrinting; }
            private set { finishedPrinting = value; }
        }

        private void Reset()
        {
            currentDialogText.text = "";
            currentSpeakerText.text = "";
            charCounter = 0;
            FinishedPrinting = false;
            speakerAssigned = false;
        }

        //private Dialog CurrentDialog
        //{
        //    get { return GameController.Instance.GetCurrentActivity() as Dialog; }
        //}
    }
}