using UnityEngine;
using UnityEngine.UI;
using StevenUniverse.FanGame.Extensions;
using StevenUniverse.FanGame.Interactions.Activities;
using StevenUniverse.FanGame.Util;

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

        protected void FixedUpdate()
        {
            if (!speakerAssigned)
            {
                currentSpeakerText.text = CurrentDialog.CurrentSpeaker;
                speakerAssigned = true;
            }
            if (!FinishedPrinting)
            {
                currentDialogText.text = CurrentDialog.CurrentMessage.Substring(0, charCounter);

                if (charCounter < CurrentDialog.CurrentMessage.Length)
                {
                    charCounter++;
                }
                else
                {
                    FinishedPrinting = true;
                }
            }
        }

        public void SkipToEnd()
        {
            charCounter = CurrentDialog.CurrentMessage.Length;
        }

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

        private Dialog CurrentDialog
        {
            get { return GameController.Instance.GetCurrentActivity() as Dialog; }
        }
    }
}