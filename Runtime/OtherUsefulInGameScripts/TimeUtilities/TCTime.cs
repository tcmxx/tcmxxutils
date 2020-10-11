using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TCUtils
{
    public static class TCTime
    {
        public const long SecToTicksConst = 10000000;

        public static ITimeProvider Provider
        {
            get; set;
        } = new DeviceTimeProvider();

        public static System.DateTime UTCNow()
        {
            return Provider.UTCNow();
        }
        
        public static async UniTask RequestUpdateTime()
        {
            await Provider.RequestUpdateTime();
        }

        public static long TicksToSec(long ticks)
        {
            return ticks / SecToTicksConst;
        }

        public static long SecToTicks(long sec)
        {
            return sec * SecToTicksConst;
        }

        public static string FormatTimeSpanSec(int sec)
        {
            return System.TimeSpan.FromSeconds(sec).ToString("g");
        }
    }
    
    public interface ITimeProvider
    {
        System.DateTime UTCNow();
        UniTask RequestUpdateTime();
    }

    public class DeviceTimeProvider : ITimeProvider
    {
        public System.DateTime UTCNow()
        {
            return System.DateTime.UtcNow;
        }

        public async UniTask RequestUpdateTime()
        {
            await UniTask.CompletedTask;
        }
    }
}