using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCUtils
{
    public interface IDataConsumer
    {
        string ID { get; }  //id of the data to consume. empty string will always consume the latest data
        void Consume(object data);
    }
}