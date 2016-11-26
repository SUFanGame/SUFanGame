using UnityEngine;
using StevenUniverse.FanGame.Util;
using StevenUniverse.FanGame.Util.MapEditing;

/// <summary>
/// A dynamic mesh, where the verts are divided into one-unit cells in a 2D grid.
/// Each cell's data can be manipulated invividually and the mesh will only update
/// once per frame and only when something has changed.
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer)), ExecuteInEditMode]
public class TiledMesh : MonoBehaviour 
{
    /// <summary>
    /// How many cells per row/column in the texture spritesheet.
    /// </summary>
    public static IntVector2 textureCellCount_ = new IntVector2(8, 8);


    // Mesh data. We want to avoid resizing these as much as possible since it
    // can cause a lot of unneccesary allocations. Sometimes it's necessary though.
    // In a perfect world we could use lists, but unity's mesh only accepts arrays, so arrays it is.
    #region MeshData
    [SerializeField]
    [HideInInspector]
    Vector3[] verts_;
    [SerializeField]
    [HideInInspector]
    int[] tris_;
    [SerializeField]
    //[HideInInspector]
    Vector2[] uvs_;
    [SerializeField]
    //[HideInInspector]
    Color32[] colors_;
    //[SerializeField, HideInInspector]
    //Vector3[] normals_;
    #endregion

    public MeshFilter filter_ { get; private set; }
    public MeshRenderer renderer_ { get; private set; }

    // See SetVisibleAlpha
    [SerializeField,HideInInspector]
    int visibleAlpha_ = 255;

    [SerializeField,HideInInspector]
    IntVector2 lastSize_;
    [SerializeField]
    protected IntVector2 size_ = IntVector2.one;
    public IntVector2 Size_
    {
        get { return size_; }
        set
        {
            value = IntVector2.Clamp(value, 1, MaxChunkSize_);
            if (value != size_)
            {
                sizeChanged_ = true;
            }
            size_ = value;
        }
    }

    public const int MaxChunkSize_ = 50;


    /// <summary>
    /// Total count of the cells on the mesh.
    /// </summary>
    public int CellCount_ { get { return size_.x * size_.y; } }

    Vector2 uvSize_ = Vector2.zero;

    // Bools to determine when the mesh needs to update itself
    // These will be set when outside sources make changes to the mesh
    // and the mesh will refresh itself on it's next lateupdate.
    // We want to serialize these so they get handled properly by unity's Undo/Redo system.
    [SerializeField, HideInInspector]
    bool uvsChanged_ = false;
    [SerializeField, HideInInspector]
    protected bool colorsChanged_ = false;
    [SerializeField, HideInInspector]
    protected bool vertsChanged_ = false;
    [SerializeField, HideInInspector]
    protected bool sizeChanged_ = false;

    public bool showLayerOrder_ = false;

    public SortingLayer SortingLayer_
    {
        get { return SortingLayerUtil.GetLayerFromID(renderer_.sortingLayerID); }
        set { renderer_.sortingLayerID = value.id; }
    }

    protected virtual void Awake()
    {
        filter_ = GetComponent<MeshFilter>();
        renderer_ = GetComponent<MeshRenderer>();

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetSelectedWireframeHidden(renderer_, true);
#endif

        // If we duplicate an object with a tiled mesh, the mesh doesn't get duplicated with it
        // Since the mesh is an asset the new object instead has a reference to the original object's mesh
        // If we check against the name of the new object we can know if the newly created object's mesh is 
        // referencing the mesh of another object.
        if ( filter_.sharedMesh != null && filter_.sharedMesh.name != name )
        {
            // Re-create our mesh if it doesn't match our name. This seems to handle
            // duplication nicely.
            MakeMesh();
        }

        OnValidate();
    }

    // Force an update on start.
    void Start()
    {
        vertsChanged_ = true;
        uvsChanged_ = true;
        colorsChanged_ = true;
    }

    /// <summary>
    /// Set the uv data for the vertices at the given cell position.
    /// </summary>
    /// <param name="pos">2D Cell position in the mesh.</param>
    /// <param name="uv">The uv data to assign to the vertices at the given cell.</param>
    public void SetUVs(IntVector2 pos, Vector2[] uv)
    {
        uvsChanged_ = true;
        int uvIndex = (pos.y * Size_.x + pos.x) * 4;

        for (int i = 0; i < 4; ++i)
        {
            uvs_[uvIndex + i] = uv[i];
        }
    }

    public void SetUVs(IntVector2 pos, Vector2 uv)
    {
        uvsChanged_ = true;
        int uvIndex = (pos.y * Size_.x + pos.x) * 4;

        for (int i = 0; i < 4; ++i)
        {
            uvs_[uvIndex + i] = uv;
        }
    }

    /// <summary>
    /// Set the UVs for the given mesh cell to be the image at the given texture cell.
    /// </summary>
    public void SetUVs(IntVector2 pos, IntVector2 textureCellIndex)
    {
        if (textureCellIndex.x < 0 || textureCellIndex.x >= textureCellCount_.x ||
            textureCellIndex.y < 0 || textureCellIndex.y >= textureCellCount_.y)
        {
            string errString = string.Format(
                "Error setting uvs on {0}, {1} is out of the texture cell bounds {2}", name, textureCellIndex, textureCellCount_);
            Debug.Log(errString, gameObject);
        }

        uvsChanged_ = true;
        int uvIndex = (pos.y * Size_.x + pos.x) * 4;

        textureCellIndex.y = textureCellCount_.y - 1 - textureCellIndex.y;

        // Get the four corners of our texture cell in uv coordinates
        Vector2 bl = Vector2.Scale(uvSize_, textureCellIndex);
        Vector2 tr = bl + uvSize_;
        Vector2 tl = bl + Vector2.up * uvSize_.y;
        Vector2 br = bl + Vector2.right * uvSize_.x;

        // Assign our uv data.
        uvs_[uvIndex + 0] = tl;
        uvs_[uvIndex + 1] = tr;
        uvs_[uvIndex + 2] = bl;
        uvs_[uvIndex + 3] = br;
    }

    public void SetUVs(int x, int y, Vector2[] uv)
    {
        uvsChanged_ = true;
        int uvIndex = (y * Size_.x + x) * 4;

        for (int i = 0; i < 4; ++i)
        {
            uvs_[uvIndex + i] = uv[i];
        }
    }

    public void SetUVs(int x, int y, int texX, int texY)
    {
        SetUVs(new IntVector2(x, y), new IntVector2(texX, texY));
    }

    ///// <summary>
    ///// Set the normal data for the vertices at the given cell position.
    ///// </summary>
    //public void SetNormals(IntVector2 pos, Vector3 normal)
    //{
    //    normalsChanged_ = true;
    //    int normalIndex = (pos.y * Size_.x + pos.x) * 4;

    //    for (int i = 0; i < 4; ++i)
    //        normals_[normalIndex + i] = normal;
    //}

    /// <summary>
    /// Get the current UV data at the given position.
    /// </summary>
    public Vector2 GetUVs(IntVector2 pos)
    {
        return uvs_[(pos.y * Size_.x + pos.x) * 4];
    }

    /// <summary>
    /// Gets the color data at the given position.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Color32 GetColors(IntVector2 pos)
    {
        return colors_[(pos.y * Size_.x + pos.x) * 4];
    }

    /// <summary>
    /// Set the color data for the vertices at the given cell position. Note tha color's alpha value will be ignored.
    /// Use SetAlpha to change a cells alpha value.
    /// </summary>
    public void SetColors(IntVector2 pos, Color32 color)
    {
        //Debug.LogFormat("Setting mesh to {0} at {1}", color, pos);
        colorsChanged_ = true;
        color = new Color32(color.r, color.g, color.b, (byte)visibleAlpha_);
        int colorIndex = (pos.y * Size_.x + pos.x) * 4;
        for (int i = 0; i < 4; ++i)
            colors_[colorIndex + i] = color;
    }

    /// <summary>
    /// Set the color data for the vertices at the given cell position.
    /// </summary>
    public void SetColors(int x, int y, Color32 color)
    {
        SetColors(new IntVector2(x, y), color);
    }



    /// <summary>
    /// Update our mesh if any data has changed.
    /// </summary>
    void LateUpdate()
    {
        //Debug.LogFormat("Late update for mesh {0}", name);
        if (filter_.sharedMesh == null)
            return;

        if( sizeChanged_ )
        {
            sizeChanged_ = false;
            RebuildUnderlyingArrays();
            vertsChanged_ = true;
            lastSize_ = size_;
        }

        if( vertsChanged_ )
        {
            vertsChanged_ = false;
            uvsChanged_ = true;
            PushVertsAndTris();
        }

        if (uvsChanged_)
        {
            uvsChanged_ = false;
            PushUVs();
        }

        //if (normalsChanged_)
        //{
        //    normalsChanged_ = false;
        //    RefreshNormals();
        //}

        if (colorsChanged_)
        {
            colorsChanged_ = false;
            PushColors();
        }

    }

    /// <summary>
    /// Assign the current color data to the mesh. Note this is called automatically when
    /// changes are made via SetColors or SetVisble/Hidden
    /// </summary>
    void PushColors()
    {
        //Debug.LogFormat("Refreshing Colors:{0}", string.Join(",", colors_.Select(c=>c.ToString()).ToArray()));
        filter_.sharedMesh.colors32 = colors_;
    }

    public virtual void ClearColors()
    {
        System.Array.Clear(colors_, 0, colors_.Length);
        colorsChanged_ = true;
    }

    //public virtual void ClearNormals()
    //{
    //    System.Array.Clear(normals_, 0, normals_.Length);
    //    normalsChanged_ = true;
    //}

    public virtual void ClearUVs()
    {
        System.Array.Clear(uvs_, 0, uvs_.Length);
        uvsChanged_ = true;
    }

    public void RefreshUVs()
    {
        uvsChanged_ = true;
    }

    public void RefreshColors()
    {
        colorsChanged_ = true;
    }

    /// <summary>
    /// Assign the current uv data to the mesh. Note this is called automatically on the next
    /// update when changes are made via SetUVs. This DOES NOT cause allocations.
    /// </summary>
    void PushUVs()
    {
        //Debug.LogFormat("Assigning UVs to mesh");
        //Debug.LogFormat("Refreshing UVs: {0}", string.Join(",", uvs_.Select(uv=> uv.ToString() ).ToArray() ));
        filter_.sharedMesh.uv = uvs_;
    }

    /// <summary>
    /// Assign the current vert and triangle data to the mesh. This DOES NOT cause allocations.
    /// </summary>
    void PushVertsAndTris()
    {
        filter_.sharedMesh.Clear();

        filter_.sharedMesh.vertices = verts_;
        filter_.sharedMesh.triangles = tris_;

        filter_.sharedMesh.RecalculateBounds();
    }

    /// <summary>
    /// Force the mesh to update immediately. 
    /// You will need to do this if you try to resize the mesh and change the mesh data on the same frame.
    /// </summary>
    public void ImmediateUpdate()
    {
        LateUpdate();
    }

    ///// <summary>
    ///// Assign our current normal data to the mesh. This DOES NOT cause allocations.
    ///// </summary>
    //void RefreshNormals()
    //{
    //    filter_.sharedMesh.normals = normals_;
    //}


    /// <summary>
    /// Defines what this mesh considers a "visible" tile's alpha.
    /// All cells with a non-zero alpha will be set to this value.
    /// Future tiles set to visible will have their alpha set to this.
    /// </summary>
    /// <param name="alpha"></param>
    public void SetVisibleAlpha( byte alpha )
    {
        //Debug.Log("Setting alpha for " + name + ":" + alpha.ToString() );  
        visibleAlpha_ = alpha;
        colorsChanged_ = true;
        for( int i = 0; i < colors_.Length; ++i )
        {
            if (colors_[i].a != 0)
                colors_[i].a = alpha;
        }
    }

    /// <summary>
    /// Sets the given cell to be visible (set's it's alpha value to match <seealso cref="visibleAlpha_"/>
    /// </summary>
    public void SetVisible(IntVector2 pos)
    {
        colorsChanged_ = true;
        int colorIndex = (pos.y * Size_.x + pos.x) * 4;
        for (int i = 0; i < 4; ++i)
            colors_[colorIndex + i].a = (byte)visibleAlpha_;
    }

    /// <summary>
    /// Sets the given cell to be visible (set's it's alpha value to 0
    /// </summary>
    public void SetHidden(IntVector2 pos)
    {
        colorsChanged_ = true;
        int colorIndex = (pos.y * Size_.x + pos.x) * 4;
        for (int i = 0; i < 4; ++i)
            colors_[colorIndex + i].a = 0;
    }



    // Note this using this actually takes some noticable time when the mesh
    // is being polled many many times per frame. Better to inline it.
    ///// <summary>
    ///// Get the 1D index for the cell at the given position.
    ///// </summary>
    //int GetIndex( IntVector2 pos )
    //{
    //    return pos.y * Size_.x + pos.x;
    //}

    /// <summary>
    /// Initialize mesh data to accomodate the given size.
    /// </summary>
    public virtual void Initialize(IntVector2 size)
    {
        size_ = size;

        OnValidate();
    }

    protected virtual void OnValidate()
    {
        if (!isActiveAndEnabled)
            return;


        size_ = IntVector2.Clamp(size_, 1, MaxChunkSize_);

        uvSize_ = new Vector2(1f / (float)textureCellCount_.x, 1f / (float)textureCellCount_.y);

        if (filter_ == null || !gameObject.activeInHierarchy)
            return;

        // Create our mesh if it doesn't already exist.
        if (filter_.sharedMesh == null )
        {
            MakeMesh();
        }

        // If our vert or tri arrays don't match our current size.
        if (size_ != lastSize_)
        {
            //Debug.LogFormat("SIZECHANGED");
            lastSize_ = size_;
            sizeChanged_ = true;
        }

        // If our verts don't match the mesh's verts, they need to be pushed to the mesh.
        if ( sizeChanged_ || filter_.sharedMesh.vertexCount != verts_.Length)
        {
            vertsChanged_ = true;
        }

        //if (renderer_.sortingLayerID != sortingLayerID_)
        //{
        //    renderer_.sortingLayerID = sortingLayerID_;
        //}
        //if (renderer_.sortingOrder != orderInLayer_)
        //{
        //    renderer_.sortingOrder = orderInLayer_;
        //}
    }

    void MakeMesh()
    {
        var mesh = new Mesh();
        mesh.hideFlags = HideFlags.HideAndDontSave;
        mesh.name = name;
        filter_.sharedMesh = mesh;
    }

    /// <summary>
    /// Rebuild the underlying arrays of the mesh and re-populate the vert and triangle data.
    /// Note this is an expensive operation and shouldn't be called often.
    /// </summary>
    void RebuildUnderlyingArrays()
    {
        int cellCount = CellCount_;
        verts_ = new Vector3[cellCount * 4];
        tris_ = new int[cellCount * 6];
        uvs_ = new Vector2[cellCount * 4];
        colors_ = new Color32[cellCount * 4];

        // Set our colors to default. This will make all cells invisible
        // but allow you to show cells via SetVisible without messing
        // with the the colors.
        for (int i = 0; i < colors_.Length; ++i)
            colors_[i] = new Color32(255, 255, 255, 0);

        //normals_ = new Vector3[cellCount * 4];

        BuildVerts();

    }

    /// <summary>
    /// Populate the vert and triangle data for our current size.
    /// </summary>
    protected void BuildVerts()
    {
        //Debug.Log("Refreshing verts (overriding uvs)");
        for (int x = 0; x < Size_.x; ++x)
        {
            for (int y = 0; y < Size_.y; ++y)
            {
                int index = y * Size_.x + x;

                var pos = new Vector3(x, y, 0);

                int vertIndex = index * 4;
                verts_[vertIndex + 0] = pos + new Vector3(0, 1, 0);
                verts_[vertIndex + 1] = pos + new Vector3(1, 1, 0);
                verts_[vertIndex + 2] = pos + new Vector3(0, 0, 0);
                verts_[vertIndex + 3] = pos + new Vector3(1, 0, 0);

                // Verts ordered as such since that's how unity's sprite uvs are ordered...
                // This way we can just directly use unity's sprite uvs
                // 0---1
                // |   |
                // 2---3


                int triIndex = index * 6;
                tris_[triIndex + 0] = vertIndex + 0;
                tris_[triIndex + 1] = vertIndex + 1;
                tris_[triIndex + 2] = vertIndex + 2;
                tris_[triIndex + 3] = vertIndex + 3;
                tris_[triIndex + 4] = vertIndex + 2;
                tris_[triIndex + 5] = vertIndex + 1;
                // 0->1->2, 3->2->1
                //  0--1      1
                //  | /      /|
                //  |/      / |
                //  2      2--3

                // Set uvs to sensible defaults. Each cell will be the entire texture.
                uvs_[vertIndex + 0] = new Vector2(0, 1);
                uvs_[vertIndex + 1] = new Vector2(1, 1);
                uvs_[vertIndex + 2] = new Vector2(0, 0);
                uvs_[vertIndex + 3] = new Vector2(1, 0);
            }
        }
    }


    public bool InBounds(IntVector2 pos)
    {
        return pos.x >= 0 && pos.x < size_.x && pos.y >= 0 && pos.y < size_.y;
    }
}
