using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SizeStringUtils
{
    private static List<uint> sizes;//use temp list to prevent allocation
    private static string[] unitStrings = new string[]
    {
        "B",
        "KB",
        "MB",
        "GB",
        "TB",
        "PB"
    };

    public static void ParseSize(ulong totalBytes, out uint b, out uint k, out uint m, out uint g, out uint t)
    {
        b = (uint)totalBytes % 1024;
        totalBytes = totalBytes / 1024;
        k = (uint)totalBytes % 1024;
        totalBytes = totalBytes / 1024;
        m = (uint)totalBytes % 1024;
        totalBytes = totalBytes / 1024;
        g = (uint)totalBytes % 1024;
        totalBytes = totalBytes / 1024;
        t = (uint)totalBytes;
    }

    public static List<uint> ParseSize(ulong totalBytes)
    {
        if (sizes == null)
            sizes = new List<uint>();
        sizes.Clear();

        while (totalBytes > 0)
        {
            var s = (uint)totalBytes % 1024;
            totalBytes = totalBytes / 1024;
            sizes.Add(s);
        }
        return sizes;
    }

    public static string FormatSizeOneUnit(ulong totalBytes)
    {
        var result = ParseSize(totalBytes);

        string unitString = unitStrings[0];

        uint mainSize = 0;
        uint digitSize = 0;

        for(int i = result.Count-1;i >= 0; --i)
        {
            if(result[i] > 0)
            {
                unitString = i >= unitStrings.Length ? "??" : unitStrings[i];
                mainSize = result[i];
                if(i > 0)
                {
                    digitSize = result[i - 1];
                }
                break;
            }
        }

        float size = mainSize + digitSize / 1024.0f;

        return size.ToString("0.00") + unitString;
    }
}
