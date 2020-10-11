using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCUtils
{
    public class DataHistoryBuffer<T>
    {
        private List<(int timeStamp, T data)> history = new List<(int timeStamp, T data)>();
        public int MaxCount { get; private set; }
        public int CurrentCount { get; private set; }
        public int LastTimeStep => Last.timeStamp;
        public (int timeStamp, T data) Last { get; private set; } = (-1, default);

        private int currentHeadIndex;

        public DataHistoryBuffer(int maxCount)
        {
            MaxCount = maxCount;
            CurrentCount = 0;
            currentHeadIndex = -1;
        }

        public (int index, T data) PushData(T data)
        {
            currentHeadIndex = (currentHeadIndex + 1) % MaxCount;
            CurrentCount = Mathf.Min(CurrentCount + 1, MaxCount);

            Last = (Last.timeStamp + 1, data);
            (int index, T data) first = (-1, default);

            if (history.Count <= currentHeadIndex)
            {
                history.Add(Last);
            }
            else
            {
                first = history[currentHeadIndex];
                history[currentHeadIndex] = Last;
            }
            return first;
        }

        public void GetLatestHistory(int count, IList<(int timeStamp, T data)> listToAdd)
        {
            if (count > CurrentCount)
                throw new IndexOutOfRangeException();

            GetHistory(CurrentCount - count, count, listToAdd);
        }

        public void GetHistory(int startIndex, int length, IList<(int timeStamp, T data)> listToAdd)
        {
            if (startIndex >= CurrentCount || length + startIndex >= CurrentCount)
                throw new IndexOutOfRangeException();

            var startRealIndex = (currentHeadIndex + 1 + startIndex - CurrentCount + MaxCount) % MaxCount;

            for (int i = 0; i < length; ++i)
            {
                var index = (startRealIndex + i) % MaxCount;
                listToAdd.Add(history[index]);
            }
        }
    }
}