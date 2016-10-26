using UnityEngine;
using System;
using System.IO;
using StevenUniverse.FanGame.Util;

namespace StevenUniverse.FanGame.Interactions
{
    public static class SupportLoader
    {

        public static ScriptLine[] ImportSupport(string supportName)
        {

            //use supportName to find the correct json file
            //Not sure how assets are handled in build, so check if this is the correct way to search for a file
            string absolutePath = Utilities.ConvertAssetPathToAbsolutePath("Assets/Resources/Supports/" + supportName + ".json");
            if (!File.Exists(absolutePath))
            {
                throw new UnityException("Support " + supportName + " was not found.");
            }

            ScriptLine[] parsedLines = JsonHelper.FromJson<ScriptLine>( File.ReadAllText(absolutePath) );

            return parsedLines;
        }
    }

    public static class JsonHelper //Helper class to deserialize Array Json
    {

        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }

    [System.Serializable]
    public class ScriptLine
    {
        [SerializeField]
        private string line;
        [SerializeField]
        private string currentSpeaker; //What goes on nameplate
        [SerializeField]
        private string leftSpeaker; //Only the names, the character instance will have to be found
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
        }

        public string CurrentSpeaker
        {
            get { return currentSpeaker; }
        }

        public string LeftSpeaker
        {
            get { return leftSpeaker; }
        }

        public string RightSpeaker
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
        
    }
}
