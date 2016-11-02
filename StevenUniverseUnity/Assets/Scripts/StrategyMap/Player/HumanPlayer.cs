using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace StevenUniverse.FanGame.StrategyMap.Players
{
    public class HumanPlayer : StrategyPlayer
    {

        MapCharacter selected_ = null;

        RaycastHit2D[] buffer_ = new RaycastHit2D[1];

        void OnEnable()
        {
            MapCharacter.OnSelected_ += OnSelected;
        }

        void OnDisable()
        {
            MapCharacter.OnSelected_ -= OnSelected;
        }


        void Update()
        {
            //if( Input.GetMouseButtonUp(0) )
            //{
            //    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //    // Note we're not casting against any layers. If we want to click on terrain we'll probably
            //    // have that on a separate layer and cast against it as well.
            //    int hitCount = Physics2D.RaycastNonAlloc(ray.origin, ray.direction, buffer_, Mathf.Infinity);

            //    if (hitCount == 0)
            //        return;

            //    var hit = buffer_[0].transform.GetComponentInParent<MapCharacter>();

            //    Debug.LogFormat("RAYHIT {0}", hit.name );
            //}
        }

        void OnSelected( MapCharacter character )
        {
            // What to do when a unit is first selected
            if( selected_ != character )
            {
                
            }
            // What to do if a unit is clicked again after it's already been selected
            else if( selected_ == character )
            {

            }

        }

    }
}
