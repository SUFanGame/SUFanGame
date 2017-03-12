using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SUGame.Util
{
    public class EnumUtil
    {
        public static List<T> GetEnumValues<T>() where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new System.ArgumentException("GetValues<T> can only be called for types derived from System.Enum", "T");
            }
            return System.Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }
    }
}
