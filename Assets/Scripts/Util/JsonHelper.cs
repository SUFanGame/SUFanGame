using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using FullSerializer;

namespace SUGame.Util
{
    /// <summary>
    /// Helper class to deserialize Array Json. Interally uses FullSerializer
    /// Expected Format: {"Items":[ {...}, {...}, ... ] }
    /// </summary>
    public static class JsonHelper
    {

        public static string Serialize<T>(Type type, T[] array)
        {
            // Expected Format: {"Items":[ {...}, {...} ] }
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return StringSerializationAPI.Serialize(typeof(Wrapper<T>), wrapper);
        }

        public static T[] Deserialize<T>(string json)
        {
            // Expected Format: {"Items":[ {...}, {...} ] }
            Wrapper<T> wrapper = (Wrapper<T>)StringSerializationAPI.Deserialize(typeof(Wrapper<T>), json);
            return wrapper.Items;
        }

        private class Wrapper<T>
        {
            public T[] Items;
        }
    }

    /// <summary>
    /// The most basic use of FullSerializer without any bells or whistles.
    /// </summary>
    public static class StringSerializationAPI
    {
        private static readonly fsSerializer _serializer = new fsSerializer();

        public static string Serialize(Type type, object value)
        {
            // serialize the data
            fsData data;
            _serializer.TrySerialize(type, value, out data).AssertSuccessWithoutWarnings();

            // emit the data via JSON
            return fsJsonPrinter.CompressedJson(data);
        }

        public static object Deserialize(Type type, string serializedState)
        {
            // step 1: parse the JSON data
            fsData data = fsJsonParser.Parse(serializedState);

            // step 2: deserialize the data
            object deserialized = null;
            _serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();

            return deserialized;
        }
    }

}