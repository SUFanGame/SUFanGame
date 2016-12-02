using SUGame.Util.Logic.States;
using System.Collections;
using UnityEngine;

namespace SUGame.StrategyMap.Characters.Actions
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

        // Optional coroutine for actions which require no user inputs ( meaning they require no parameters and return no UI state ).
        // If an action would execute immediately upon being selected it should do so here.
        public virtual IEnumerator Execute()
        {
            yield return null;
        }

        /// <summary>
        /// Starts the action's coroutine.
        /// </summary>
        //public void Execute()
        //{
        //    StartCoroutine(Routine());
        //}

        ///// <summary>
        ///// Coroutine which will perform the action. Base behaviour is to pause the unit (end their turn).
        ///// </summary>
        //public virtual IEnumerator Routine()
        //{
        //    //Debug.Log(uiName_);
        //    actor_.Paused_ = true;

        //    yield return null;
        //}

        /// <summary>
        /// Optional state that will be passed to HumanPlayer's SelectionState for processing.
        /// The state should process player input sensibly and pop itself
        /// off the stack if the player cancels the action
        /// </summary>
        //public virtual State UIState { get; protected set; }

        public virtual State GetUIState()
        {
            return null;
        }
    }
}
