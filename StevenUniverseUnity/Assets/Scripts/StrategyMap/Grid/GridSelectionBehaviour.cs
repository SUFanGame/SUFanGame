using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using StevenUniverse.FanGame.StrategyMap;

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

    void OnGridBuilt( Grid grid )
    {
        collider_.size = (Vector2)grid.Size;
        collider_.offset = collider_.size * .5f;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClicked_ != null)
            OnClicked_.Invoke(eventData);
    }
}
