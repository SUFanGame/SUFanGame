using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace SUGame.StrategyMap.UI
{
    [SelectionBase]
    public class HighlighObject : MonoBehaviour
    {
        SpriteRenderer renderer_;

        void Awake()
        {
            renderer_ = GetComponentInChildren<SpriteRenderer>();
            renderer_.sortingLayerName = "Overworld";
        }

        public Color Color { get { return renderer_.color; } set { renderer_.color = value; } }

        public int SortingOrder { get { return renderer_.sortingOrder; } set { renderer_.sortingOrder = value; } }
    }
}