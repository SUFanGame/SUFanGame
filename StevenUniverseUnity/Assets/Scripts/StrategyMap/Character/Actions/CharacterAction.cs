using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.StrategyMap;

namespace StevenUniverse.FanGame.StrategyMap
{
    public class CharacterAction : MonoBehaviour
    {
        /// <summary>
        /// The character performing this action.
        /// </summary>
        protected MapCharacter actor_;

        /// <summary>
        /// The name that will show up in the in-game UI.
        /// </summary>
        [SerializeField]
        string uiName_;

        public string UIName { get { return uiName_; } }
        public MapCharacter Actor { get { return actor_; } }

        void Awake()
        {
            actor_ = GetComponent<MapCharacter>();
        }

        /// <summary>
        /// Allows actions to be restricted based on certain conditions. If this returns false
        /// an action will not show up when a character is selected.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsUsable()
        {
            return true;
        }

        /// <summary>
        /// Execute a character action. Note this might not necessarily do something immediately,
        /// IE: For CharacterMove this will just allow the user to select a position, the character won't
        /// actually move until selection has taken place.
        /// </summary>
        public virtual void Execute()
        {
            Debug.Log(uiName_);
        }
    }
}
