using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace StevenUniverse.FanGameEditor.SceneEditing
{
    public class SceneEditorUtil
    {
        /// <summary>
        /// Draws a cursor of the given size and returns the cursor's position, snapped to the 1x1 grid
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetCursorPosition()
        {
            var mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            var mousePoint = mouseRay.GetPoint(0);
            for( int i = 0; i < 2; ++i )
            {
                mousePoint[i] = Mathf.Floor(mousePoint[i]);
            }

            mousePoint[2] = 0;

            return mousePoint;
        }
        


        /// <summary>
        /// Property to control or poll whether we're editing in the scene view
        /// </summary>
        public static bool EditMode_
        {
            // This is how Unity's terrain editor does it - when you
            // enter terrain editing mode it deselects tools
            // and prevents other selections in the scene view
            // until another tool is selected or until
            // the terrain is deselected via the hierarchy
            get { return UnityEditor.Tools.current == Tool.None; }
            set
            {
                if (value)
                    UnityEditor.Tools.current = Tool.None;
                else
                    UnityEditor.Tools.current = Tool.Move;
            }
        }
        
    }
}
 