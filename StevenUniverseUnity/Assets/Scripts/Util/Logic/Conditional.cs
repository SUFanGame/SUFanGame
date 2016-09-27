using UnityEngine;
using StevenUniverse.FanGame.Data.DataTypes;

namespace StevenUniverse.FanGame.Util.Logic
{
    [System.Serializable]
    public class Conditional
    {
        [SerializeField]
        private string logic;

        public Conditional(string logic)
        {
            this.logic = logic;
        }

        public bool CheckStatus()
        {
            var tokens = new Tokenizer(logic).Tokenize();
            var parser = new Parser(tokens);

            DataBool result = parser.Parse() as DataBool;

            if (result != null)
            {
                return result.Data;
            }
            else
            {
                throw new UnityException("Non-boolean data returned!");
            }
        }
    }
}