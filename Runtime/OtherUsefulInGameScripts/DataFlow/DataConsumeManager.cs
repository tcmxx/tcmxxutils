using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCUtils
{
    //can be used to pass data between scenes instead of using static variables
    public static class DataConsumeManager
    {
        public class DataEntry
        {
            public string id;
            public int consumableCount; //-1 means infinit
            public object data;
        }
        private static List<DataEntry> datas = new List<DataEntry>();

        public const string AllConsumableKey = "";
        public static void Consume(IDataConsumer node)
        {
            string id = node.ID;
            if (string.IsNullOrEmpty(id))
            {
                if (datas.Count > 0)
                {
                    ConsumeDataAt(datas.Count - 1, node);
                    return;
                }
            }

            for(int i = datas.Count-1; i >= 0; i--)
            {
                if (datas[i].id == id)
                {
                    ConsumeDataAt(i, node);
                    return;
                }
            }

            node.Consume(null);
        }

        private static void ConsumeDataAt(int index, IDataConsumer node)
        {
            var data = datas[index];
            data.consumableCount--;
            if (data.consumableCount == 0)
            {
                datas.RemoveAt(index);
            }

            node.Consume(data.data);
        }

        //push a data that can be consumed infinitely
        public static void PushPersistData(string id, object data)
        {
            PushData(id, data, -1);
        }

        //push one data
        public static void PushData(string id, object data)
        {
            PushData(id, data, 1);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">if empty string, it will be consumable by everyone</param>
        /// <param name="data">data </param>
        /// <param name="consumableCount">-1 or infinite</param>
        public static void PushData(string id, object data, int consumableCount)
        {
            datas.Add(new DataEntry() { id = id, consumableCount = consumableCount, data = data });
        }

        public static void Clear()
        {
            datas.Clear();
        }
    }
}