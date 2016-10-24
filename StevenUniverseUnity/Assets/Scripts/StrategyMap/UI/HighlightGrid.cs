using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.StrategyMap;
using StevenUniverse.FanGame.Overworld;

// TODO Implement a more formal object pool class

/// <summary>
/// Global class for highlighting postions on the battle grid
/// </summary>
public class HighlightGrid : MonoBehaviour 
{
    [SerializeField]
    HighlighObject pfb_HighlightObject_;

    public static HighlightGrid Instance { get;  private set; }

    static Queue<HighlighObject> inactiveHighlights_ = new Queue<HighlighObject>();

    static Queue<HighlighObject> activeHighlights_ = new Queue<HighlighObject>();

    static Dictionary<IntVector2, HighlighObject> positionMap_ = new Dictionary<IntVector2, HighlighObject>();

    Grid grid_;

    void Awake()
    {
        Instance = this;
        grid_ = GameObject.FindObjectOfType<Grid>();
    }

    /// <summary>
    /// Hide all active highlights
    /// </summary>
    public static void Clear()
    {
        positionMap_.Clear();
        while (activeHighlights_.Count > 0)
        {
            var go = activeHighlights_.Dequeue();
            go.gameObject.SetActive(false);
            inactiveHighlights_.Enqueue(go);
        }
    }
    
    /// <summary>
    /// Create a highlight on the map at the given position with the given color.
    /// </summary>
    public static void HighlightPos( int x, int y, int elevation, Color color )
    {
        HighlighObject obj;

        IntVector2 pos = new IntVector2(x, y);
        if (!positionMap_.TryGetValue(pos, out obj))
        {
            obj = Get();
            positionMap_[pos] = obj;
        }
        
        obj.gameObject.SetActive(true);
        obj.Color = color;
        obj.transform.position = new Vector3(x, y, 0);
        obj.SortingOrder = elevation * 100 + 99;

        activeHighlights_.Enqueue(obj);
    }

    static HighlighObject Get()
    {
        if (inactiveHighlights_.Count == 0)
        {
            var newObj = Instantiate(Instance.pfb_HighlightObject_);
            newObj.transform.SetParent(Instance.transform);
            inactiveHighlights_.Enqueue(newObj);
        }
        return inactiveHighlights_.Dequeue();
    }

    public static void HighlightPos( IntVector3 pos, Color color )
    {
        HighlightPos(pos.x, pos.y, pos.z, color);
    }


}
