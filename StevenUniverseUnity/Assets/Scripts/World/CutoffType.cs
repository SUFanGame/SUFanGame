using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SUGame.World
{
    public enum CutoffType
    {
        NONE = 0,
        ABOVE = 1,
        CROSS_SECTION = 2,
    }

    public static class CutoffTypeExtenstion
    {
        public static bool IsCutoff( this CutoffType cutoffType, int currentHeight, int isCutOff )
        {
            switch (cutoffType)
            {
                case CutoffType.ABOVE:
                    {
                        //Debug.LogFormat("Is {0} > {1}?", isCutOff, currentHeight);
                        return isCutOff > currentHeight;
                    }
                case CutoffType.CROSS_SECTION:
                    return isCutOff != currentHeight;
            }
            return false;
        }
    }
}
