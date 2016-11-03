using System.Collections;
using UnityEngine;

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

        public virtual string UIName { get { return uiName_; } }
        public MapCharacter Actor { get { return actor_; } }

        protected virtual void Awake()
        {
            actor_ = GetComponent<MapCharacter>();
        }

        /// <summary>
        /// Allows actions to be restricted based on certain conditions. If this returns false
        /// an action will not show up when a character is selected.
        /// </summary>
        public virtual bool IsUsable()
        {
            return true;
        }

        /// <summary>
        /// Starts the action's coroutine.
        /// </summary>
        public void Execute()
        {
            StartCoroutine(Routine());
        }

        /// <summary>
        /// Coroutine which will perform the action
        /// </summary>
        public virtual IEnumerator Routine()
        {
            //Debug.Log(uiName_);
            actor_.Paused_ = true;

            yield return null;
        }
    }
}
