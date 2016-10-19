using UnityEngine;
using System.Collections.Generic;
using StevenUniverse.FanGame.Characters;

// Lotta abstract non-functional framework

namespace StevenUniverse.FanGame.Interactions
{
    public class SupportLoader
    {
        public void FindScriptJSON()
        {
            //Use the utilities to find right script name
        }

        public List<ScriptLine> ParseJSON()
        {
            List<ScriptLine> dialog = new List<ScriptLine>();

            //parse the json, add for every line
            //dialog.Add(new Scriptline());

            return dialog;
        }
    }

    public class ScriptLine
    {
        //TO-DO: replace Character with the reference to the portrait's holder. We don't need the entire character

        private string line;
        private string currentSpeaker;
        private Character leftSpeaker;
        private Character rightSpeaker;
        private string leftExpr;
        private string rightExpr;
        private bool isSpeakerOnRight; //If true, nameplate on the right

        public ScriptLine(
            string line, 
            string currentSpeaker, 
            Character leftSpeaker,
            Character rightSpeaker,
            string leftExpr, 
            string rightExpr) 
        {
            this.line = line;
            this.currentSpeaker = currentSpeaker;
            this.leftSpeaker = leftSpeaker;
            this.rightSpeaker = rightSpeaker;
            this.leftExpr = leftExpr;
            this.rightExpr = rightExpr;
            this.isSpeakerOnRight = (rightSpeaker.Name == currentSpeaker);
        }

        public string Line
        {
            get { return line; }
        }

        public string CurrentSpeaker
        {
            get { return currentSpeaker; }
        }

        public Character LeftSpeaker
        {
            get { return leftSpeaker; }
        }

        public Character RightSpeaker
        {
            get { return rightSpeaker; }
        }

        public string LeftExpr
        {
            get { return leftExpr; }
        }

        public string RightExpr
        {
            get { return rightExpr; }
        }

        public bool IsSpeakerOnRight
        {
            get { return isSpeakerOnRight; }
        }

    }
}
