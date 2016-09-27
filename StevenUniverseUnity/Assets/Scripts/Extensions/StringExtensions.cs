using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StevenUniverse.FanGame.Extensions
{
    public static class StringExtensions
    {
        public static string[] Split(this string target, string delimiter)
        {
            List<string> segments = new List<string>();
            segments.Add(target);

            bool splitComplete = false;
            while (!splitComplete)
            {
                string currentSegment = segments[segments.Count - 1];
                int nextDelimiterIndex = currentSegment.IndexOf(delimiter);

                if (nextDelimiterIndex != -1)
                {
                    string beforeDelim = currentSegment.Substring(0, nextDelimiterIndex);
                    string afterDelim = currentSegment.Substring(nextDelimiterIndex + delimiter.Length);

                    segments[segments.Count - 1] = beforeDelim;
                    segments.Add(afterDelim);
                }
                else
                {
                    splitComplete = true;
                }
            }

            return segments.ToArray();
        }
    }
}