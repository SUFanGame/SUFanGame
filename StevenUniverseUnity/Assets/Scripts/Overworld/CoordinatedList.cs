using System.Collections.Generic;
using UnityEngine;

namespace StevenUniverse.FanGame.Overworld
{
    public class CoordinatedList<T> where T : ICoordinated
    {
        private SortedDictionary<int, SortedDictionary<int, List<T>>> mapList =
            new SortedDictionary<int, SortedDictionary<int, List<T>>>();

        int minX, maxX, minY, maxY;

        public CoordinatedList()
        {
        }

        public CoordinatedList(T[] items)
        {
            AddRange(items);
        }

        public void Add(T t)
        {
            Vector3 tPos = t.Position;
            int x = (int) tPos.x;
            int y = (int) tPos.y;

            //Set the mins and maxes if there are currently no elements
            if (mapList.Count == 0)
            {
                MinX = MaxX = x;
                MinY = MaxY = y;
            }
            //Otherwise, set any new mins and maxes that surpass previous values
            else
            {
                if (x < MinX)
                {
                    MinX = x;
                }
                else if (x > MaxX)
                {
                    MaxX = x;
                }
                if (y < MinY)
                {
                    MinY = y;
                }
                else if (y > MaxY)
                {
                    MaxY = y;
                }
            }

            bool added = false;
            while (!added)
            {
                try
                {
                    //Add the value to the list at the map's X and Y
                    mapList[x][y].Add(t);
                    added = true;
                }
                catch (KeyNotFoundException)
                {
                    //If the Map doesn't contain a key for the X value, add it
                    if (!mapList.ContainsKey(x))
                    {
                        mapList.Add(x, new SortedDictionary<int, List<T>>());
                    }

                    //If the Map doesn't contain a key for the Y value at the X value, add it
                    if (!mapList[x].ContainsKey(y))
                    {
                        mapList[x].Add(y, new List<T>());
                    }
                }
            }
        }

        public void AddRange(T[] ts)
        {
            foreach (T t in ts)
            {
                Add(t);
            }
        }

        public T[] Get(int x, int y)
        {
            try
            {
                return mapList[x][y].ToArray();
            }
            catch (KeyNotFoundException)
            {
                return new T[] {};
            }
        }

        public T[] GetRange(int rangeMinX, int rangeMinY, int rangeMaxX, int rangeMaxY)
        {
            List<T> rangeValues = new List<T>();

            //Loop through each X
            foreach (int x in mapList.Keys)
            {
                //Check if the X value is within the given range
                if (x >= rangeMinX && x <= rangeMaxX)
                {
                    //Loop through each Y
                    foreach (int y in mapList[x].Keys)
                    {
                        //Check if the Y value is within the given range
                        if (y >= rangeMinY && y <= rangeMaxY)
                        {
                            //Add the found non-null objects
                            rangeValues.AddRange(mapList[x][y]);
                        }
                    }
                }
            }

            return rangeValues.ToArray();
        }

        public T[] GetAll()
        {
            return GetRange(MinX, MinY, MaxX, MaxY);
        }

        public int MinX
        {
            get { return minX; }
            private set { minX = value; }
        }

        public int MinY
        {
            get { return minY; }
            private set { minY = value; }
        }

        public int MaxX
        {
            get { return maxX; }
            private set { maxX = value; }
        }

        public int MaxY
        {
            get { return maxY; }
            private set { maxY = value; }
        }
    }
}