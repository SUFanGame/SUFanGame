using System;
using System.Collections.Generic;
using UnityEngine;
using StevenUniverse.FanGame.Data.DataTypes;

namespace StevenUniverse.FanGame.Data
{
    [System.Serializable]
    public class DataGroup
    {
        [SerializeField]
        private string name;
        [SerializeField]
        private List<DataBool> dataBools = new List<DataBool>();
        [SerializeField]
        private List<DataInt> dataInts = new List<DataInt>();

        //Full constructor
        public DataGroup(string name, DataBool[] dataBools, DataInt[] dataInts)
        {
            Name = name;
            DataBools = dataBools;
            DataInts = dataInts;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public DataBool[] DataBools
        {
            get { return dataBools.ToArray(); }
            set { dataBools = new List<DataBool>(value); }
        }

        public DataInt[] DataInts
        {
            get { return dataInts.ToArray(); }
            set { dataInts = new List<DataInt>(value); }
        }

        private T GetData<T>(T[] dataArray, string dataName) where T : DataType
        {
            foreach (T data in dataArray)
            {
                if (data.Name.Equals(dataName, System.StringComparison.OrdinalIgnoreCase))
                {
                    return data;
                }
            }

            throw new UnityException(string.Format("Couldn't find data of type '{0}' with the name '{1}' in the dataGroup '{2}'", typeof(T).Name, dataName, Name));
        }

        public DataType GetDataOfAnyType(string dataName)
        {
            List<DataType> allDataTypes = new List<DataType>();
            allDataTypes.AddRange(DataBools);
            allDataTypes.AddRange(DataInts);

            return GetData<DataType>(allDataTypes.ToArray(), dataName);
        }

        //DataBool methods
        private DataBool GetDataBool(string dataBoolName) { return GetData<DataBool>(DataBools, dataBoolName); }
        public bool GetDataBoolValue(string dataBoolName) { return GetDataBool(dataBoolName).Data; }
        public void SetDataBoolValue(string dataBoolName, bool dataBoolValue) { GetDataBool(dataBoolName).Data = dataBoolValue; }

        //DataInt methods
        private DataInt GetDataInt(string dataIntName) { return GetData<DataInt>(DataInts, dataIntName); }
        public int GetDataIntValue(string dataIntName) { return GetDataInt(dataIntName).Data; }
        public void SetDataIntValue(string dataIntName, int dataIntValue) { GetDataInt(dataIntName).Data = dataIntValue; }
    }
}