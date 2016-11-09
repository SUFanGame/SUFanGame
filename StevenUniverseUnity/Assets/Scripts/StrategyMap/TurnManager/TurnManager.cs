using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StevenUniverse.FanGame.StrategyMap.Players;
using System;
using StevenUniverse.FanGame.Overworld;

namespace StevenUniverse.FanGame.StrategyMap
{
    /// <summary>
    /// Responsible for sorting players and calling each player's tick function.
    /// </summary>
    public class TurnManager : MonoBehaviour
    {
        [SerializeField]
        List<StrategyPlayer> players_ = new List<StrategyPlayer>();

        /// <summary>
        /// Read only list of players being managed
        /// </summary>
        public IList<StrategyPlayer> Players_ { get; private set; }
        
        /// <summary>
        /// The player whose turn it is.
        /// </summary>
        public StrategyPlayer ActingPlayer_ { get; private set; }

        TurnManagerUIPanel ui_;

        void Awake()
        {
            var players = GameObject.FindObjectsOfType<StrategyPlayer>();

            if( players == null || players.Length == 0 )
            {
                enabled = false;
                Debug.Log("Turn manager couldn't find any StrategyPlayer components.", gameObject);
                return;
            }

            // Ensure players get to go first.
            Func<StrategyPlayer,int> orderByType = (p) => p is HumanPlayer ? 0 : 1;

            players_ = players.OrderBy(orderByType).ToList();
            Players_ = players_.AsReadOnly();

            ui_ = FindObjectOfType<TurnManagerUIPanel>();

        }
        
        void Start()
        {
            Grid.Instance.OnGridBuilt_ += OnGridBuilt;
        }

        void OnGridBuilt( Grid grid )
        {
            //Debug.LogFormat("Beginning turn ticks");
            StartCoroutine(Tick());
        }

        IEnumerator Tick()
        {
            while( true )
            {
                for( int i = 0; i < players_.Count; ++i )
                {
                    var player = players_[i];

                    ui_.SetText(string.Format("{0} turn", player.name));

                    player.OnTurnStart();

                    ui_.DoFadeAnim();

                    yield return new WaitForSeconds(.15f);

                    ActingPlayer_ = player;

                    yield return player.Tick();

                    player.OnTurnEnd();

                    // Don't complete turn transitions until the animation has finished.
                    if (ui_.Animating_)
                        yield return new WaitUntil(() => !ui_.Animating_);
                }
            }

        }
        
    }
}
