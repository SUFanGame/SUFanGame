using UnityEngine;
using System.Collections.Generic;
using StevenUniverse.FanGame.Characters;

// Lotta abstract non-functional framework
// Should GUI control be in here? Shouldn't be bad to spawn a box and two portraits here

namespace StevenUniverse.FanGame.Interactions
{
    public class Support
    {

        private List<ScriptLine> dialog;
        private int count = 0;

        public void CreateSupportGUI()
        {
            //Spawn the box the dialog is spoken in
            //Display the first line in dialog
            count++;
        }

        public void NextLine()
        {
            if (count < dialog.Count)
            {
                ScriptLine curDialog = dialog[count];
                //Update the dialog
                //Update the portraits
                UpdateSupportGUI(curDialog.CurrentSpeaker, curDialog.Line, curDialog.IsSpeakerOnRight);

                UpdatePortraits(curDialog.LeftSpeaker, curDialog.LeftExpr);
                UpdatePortraits(curDialog.RightSpeaker, curDialog.RightExpr);

                count++;
            }
            else
            {
                DestroySupportGUI();
            }

            //yield control to game, wait for next button press
        }

        public void UpdatePortraits(Character replacee, string newExpression)
        {
            //Replacee should be a reference to the object holding the portrait image
            //Swap the replacee's face with the image corresponding to newExpression
        }

        public void UpdateSupportGUI(string curSpeaker, string line, bool isSpeakerOnRight)
        {
            //Change the text and the name of the speaker
            //Update the nameplate if speaker changed sides
        }

        public void DestroySupportGUI()
        {
            //Clear the GUI from the screne and return control
        }

    }
}
