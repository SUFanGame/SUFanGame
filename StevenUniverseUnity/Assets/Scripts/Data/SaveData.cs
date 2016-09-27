using System;
using System.Collections.Generic;
using UnityEngine;
using StevenUniverse.FanGame.Data.DataTypes;

namespace StevenUniverse.FanGame.Data
{
    [System.Serializable]
    public class SaveData
    {
        [SerializeField]
        private List<DataGroup> dataGroups = new List<DataGroup>();

        //Full constructor
        public SaveData(DataGroup[] dataGroups)
        {
            DataGroups = dataGroups;
        }

        public DataGroup[] DataGroups
        {
            get { return dataGroups.ToArray(); }
            set { dataGroups = new List<DataGroup>(value); }
        }

        public DataType GetDataOfAnyType(string dataGroupName, string dataName) { return GetDataGroup(dataGroupName).GetDataOfAnyType(dataName); }

        //DataBool access methods
        public bool GetDataBoolValue(string dataGroupName, string dataBoolName) { return GetDataGroup(dataGroupName).GetDataBoolValue(dataBoolName); }
        public void SetDatBoolValue(string dataGroupName, string dataBoolName, bool dataBoolValue) { GetDataGroup(dataGroupName).SetDataBoolValue(dataBoolName, dataBoolValue); }

        //DataInt access methods
        public int GetDataIntValue(string dataGroupName, string dataIntName) { return GetDataGroup(dataGroupName).GetDataIntValue(dataIntName); }
        public void SetDatIntValue(string dataGroupName, string dataIntName, int dataIntValue) { GetDataGroup(dataGroupName).SetDataIntValue(dataIntName, dataIntValue); }

        private DataGroup GetDataGroup(string dataGroupName)
        {
            foreach (DataGroup dataGroup in dataGroups)
            {
                if (dataGroup.Name.Equals(dataGroupName, System.StringComparison.OrdinalIgnoreCase))
                {
                    return dataGroup;
                }
            }

            throw new UnityException(string.Format("Couldn't find a dataGroup with the name '{0}'", dataGroupName));
        }

        
    }
}