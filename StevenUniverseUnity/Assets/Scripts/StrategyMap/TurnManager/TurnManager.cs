using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StevenUniverse.FanGame.StrategyMap.Players;
using System;

namespace StevenUniverse.FanGame.StrategyMap
{
    public class TurnManager : MonoBehaviour
    {
        [SerializeField]
        List<StrategyPlayer> players_ = new List<StrategyPlayer>();
        

        void Awake()
        {
            players_ = GameObject.FindObjectsOfType<StrategyPlayer>().ToList();

            // Ensure players get to go first.
            Func<StrategyPlayer,int> orderByType = (p) => p is HumanPlayer ? 0 : 1;

            players_ = players_.OrderBy(orderByType).ToList();


            Grid.Instance.OnGridBuilt_ += OnGridBuilt;
        }

        void OnGridBuilt( Grid grid )
        {
            StartCoroutine(Tick());
        }

        IEnumerator Tick()
        {
            while( true )
            {

                yield return null;
            }

        }
        
    }
}
