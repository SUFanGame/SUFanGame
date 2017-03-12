using UnityEngine;
using System.IO;
using SUGame.Util;

namespace SUGame.Interactions
{
    public static class SupportLoader
    {

        public static ScriptLine[] ImportSupport(string supportName)
        {

            //use supportName to find the correct json file
            // Note you must omit the file extension
            var asset = Resources.Load<TextAsset>("Supports/" + supportName);
            if (asset == null)
            {
                throw new UnityException("Support " + supportName + " was not found.");
            }

            ScriptLine[] parsedLines = JsonHelper.Deserialize<ScriptLine>(asset.text);
            //Debug.Log("Successfully parsed lines");
            return parsedLines;
        }
    }

    /// <summary>
    /// Individual lines that are used in the Support class.
    /// </summary>
    [System.Serializable]
    public class ScriptLine
    {
        [SerializeField]
        private string line;
        [SerializeField]
        private string currentSpeaker; //What goes on nameplate
        [SerializeField]
        private string leftSpeaker; //Will be used as the folder to look into for images
        [SerializeField]
        private string rightSpeaker;
        [SerializeField]
        private string leftExpr; //Will be used as image name
        [SerializeField]
        private string rightExpr;

        public ScriptLine(
            string line, 
            string currentSpeaker, 
            string leftSpeaker,
            string rightSpeaker,
            string leftExpr, 
            string rightExpr) 
        {
            this.line = line;
            this.currentSpeaker = currentSpeaker;
            this.leftSpeaker = leftSpeaker;
            this.rightSpeaker = rightSpeaker;
            this.leftExpr = leftExpr;
            this.rightExpr = rightExpr;
        }

        public string Line
        {
            get { return line; }
            set { line = value; }
        }

        public string CurrentSpeaker
        {
            get { return currentSpeaker; }
            set { currentSpeaker = value; }
        }

        public string LeftSpeaker
        {
            get { return leftSpeaker; }
            set { leftSpeaker = value; }
        }

        public string RightSpeaker
        {
            get { return rightSpeaker; }
            set { rightSpeaker = value; }
        }

        public string LeftExpr
        {
            get { return leftExpr; }
            set { leftExpr = value; }
        }

        public string RightExpr
        {
            get { return rightExpr; }
            set { rightExpr = value; }
        }

    }
}
