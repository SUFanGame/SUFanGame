using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Util;
using UnityEditor;

namespace StevenUniverse.FanGameEditor.SceneEditing
{
    /// <summary>
    /// Defines the shape and area of a map editor brush.
    /// </summary>
    public class BrushArea
    {
        List<IntVector2> cursorPoints_ = new List<IntVector2>();
        public IList<IntVector2> Points_ = null;
        int size_ = int.MinValue;
        public BrushShape brushShape_ = BrushShape.CIRCLE;
        string editorPrefsName_;

        public BrushArea( string editorPrefsName )
        {
            editorPrefsName_ = editorPrefsName;
            Points_ = cursorPoints_.AsReadOnly();
            brushShape_ = (BrushShape)EditorPrefs.GetInt(editorPrefsName + "Shape", 0);
            Size_ = EditorPrefs.GetInt(editorPrefsName + "Size", 0);
        }

        public int Size_
        {
            get { return size_; }
            set
            {
                value = Mathf.Clamp(value, 0, int.MaxValue);

                if (value == size_)
                    return;

                size_ = value;

                if (size_ == 0)
                {
                    cursorPoints_.Clear();
                    cursorPoints_.Add(IntVector2.zero);
                }
                else
                {
                    RefreshPoints();
                }

            }
            
        }

        public void RenderCursor()
        {
            var cursorPos = SceneEditorUtil.GetCursorPosition();

            var offset = Vector2.one * .5f;
            foreach (var pos in cursorPoints_)
            {
                Gizmos.DrawWireCube(cursorPos + pos + Vector3.back + (Vector3)offset, Vector3.one);
            }
        }

        public void OnDisable()
        {
            EditorPrefs.SetInt(editorPrefsName_ + "Size", size_);
            EditorPrefs.SetInt(editorPrefsName_ + "Shape", (int)brushShape_);
        }

        public void OnGUI()
        {
            var newShape = (BrushShape)EditorGUILayout.EnumPopup("Shape", brushShape_);
            if( newShape != brushShape_ )
            {
                brushShape_ = newShape;
                RefreshPoints();
            }
            Size_ = EditorGUILayout.IntField("Size", Size_);
            //int enumToInt = (int)brushShape_;
            //EditorGUILayout.LabelField("EnumToInt: " + enumToInt.ToString());
            //BrushShape intToEnum = (BrushShape)0;
            //EditorGUILayout.LabelField("IntToEnum: " + intToEnum);
        }


        /// <summary>
        /// Callback for when the mouse wheel is scrolled in the scene view.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="scrollValue"></param>
        public void OnScroll(float scrollValue)
        {
            var e = Event.current;

            if (e.shift)
                e.Use();

            if (scrollValue < 0)
            {
                Size_++;
            }
            else if (scrollValue > 0)
            {
                Size_--;
            }
        }

        void RefreshPoints()
        {
            cursorPoints_.Clear();
            brushShape_.GetCallback().Invoke(size_, cursorPoints_);
        }
    }

    public enum BrushShape
    {
        SQUARE = 0,
        CIRCLE = 1,
        DIAMOND = 2,
    }

    public delegate void BrushAreaCallback(int size, List<IntVector2> buffer);

    public static class BrushShapeExtensions
    {
        public static BrushAreaCallback GetCallback( this BrushShape shape )
        {
            switch( shape )
            {
                case BrushShape.CIRCLE: return BrushShapeUtil.BrushShapeCallbacks.Circle_;
                case BrushShape.SQUARE: return BrushShapeUtil.BrushShapeCallbacks.Square_;
                case BrushShape.DIAMOND: return BrushShapeUtil.BrushShapeCallbacks.Diamond_;
            }
            return null;
        }
    }

    namespace BrushShapeUtil
    {
        /// <summary>
        /// Callbacks which return a list of points (relative to origi) in a certain shape, given a certain size.
        /// </summary>
        static class BrushShapeCallbacks
        {
            static BrushAreaCallback squareCallback_ = Square;
            public static BrushAreaCallback Square_ { get { return squareCallback_; } }

            static BrushAreaCallback circleCallback_ = Circle;
            public static BrushAreaCallback Circle_ { get { return circleCallback_; } }

            static BrushAreaCallback diamondCallback_ = Diamond;
            public static BrushAreaCallback Diamond_ { get { return diamondCallback_; } }

            static void Square(int size, List<IntVector2> buffer)
            {
                int iterSize = size * 2 + 1;

                IntVector2 half = (IntVector2.one * iterSize) / 2;

                for (int x = -half.x; x < half.x + 1; ++x)
                {
                    for (int y = -half.y; y < half.y + 1; ++y)
                    {
                        buffer.Add(new IntVector2(x, y));
                    }
                }
            }

            static void Circle(int size, List<IntVector2> buffer)
            {
                int iterSize = size * 2 + 1;

                IntVector2 half = (IntVector2.one * iterSize) / 2;

                for (int x = -half.x; x < half.x + 1; ++x)
                {
                    for (int y = -half.y; y < half.y + 1; ++y)
                    {
                        if ( x * x + y * y <= size * 2)
                            buffer.Add(new IntVector2(x, y));
                    }
                }
            }

            static void Diamond(int size, List<IntVector2> buffer)
            {
                int iterSize = size * 2 + 1;

                IntVector2 half = (IntVector2.one * iterSize) / 2;

                for (int x = -half.x; x < half.x + 1; ++x)
                {
                    for (int y = -half.y; y < half.y + 1; ++y)
                    {
                        if (IntVector2.ManhattanDistance(IntVector2.zero, new IntVector2(x, y)) <= size)
                            buffer.Add(new IntVector2(x, y));
                    }
                }
            }
        }

    }
}