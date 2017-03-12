using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SUGame.Util
{
    public class RandUtil
    {
        /// <summary>
        /// Returns true if a random roll between 0 and 100 is below the given value
        /// </summary>
        public static bool Chance100(int chance)
        {
            return chance <= 0 ? false : Random.value < (chance / 100f);
        }
    }
}
