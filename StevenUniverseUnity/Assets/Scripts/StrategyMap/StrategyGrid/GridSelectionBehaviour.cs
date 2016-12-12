using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using SUGame.StrategyMap;
using SUGame.Util.Common;
using UnityEngine.UI;

namespace SUGame.StrategyMap
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class GridSelectionBehaviour : MonoBehaviour, IPointerClickHandler
    {
        BoxCollider2D collider_;

        public System.Action<PointerEventData> OnClicked_;

        void Awake()
        {
            collider_ = GetComponent<BoxCollider2D>();
        }

        void Start()
        {
            Grid.Instance.OnGridBuilt_ += OnGridBuilt;
        }

        void OnGridBuilt(Grid grid)
        {
            collider_.size = (Vector2)grid.Size;
            //collider_.offset = collider_.size * .5f;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("GridSelectionBehaviour ONPOINTERCLICK");
            if (OnClicked_ != null)
                OnClicked_.Invoke(eventData);
        }

        void Update()
        {
            if (Input.GetButtonDown("XboxA") && (Grid.Instance.GetHighestNode((IntVector2)StrategyCursor.Instance.pos).Objects == null))
            {
                Debug.Log("GridSelectionBehaviour A button press.");
                if (OnClicked_ != null)
                    OnClicked_.Invoke(new PointerEventData(EventSystem.current));
            }
            
        }
    }
}