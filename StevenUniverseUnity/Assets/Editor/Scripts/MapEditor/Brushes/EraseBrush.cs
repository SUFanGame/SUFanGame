using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StevenUniverse.FanGameEditor.SceneEditing.Brushes
{
    public class EraseBrush : MapEditorBrush
    {
        protected override string IconTextureName_
        {
            get
            {
                return "eraser";
            }
        }
    }
}
