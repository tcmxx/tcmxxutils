using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class EnumMaskAttribute : PropertyAttribute
{
    public bool alwaysFoldOut;
    public EnumMaskLayout layout = EnumMaskLayout.Vertical;
}

public enum EnumMaskLayout
{
    Vertical,
    Horizontal
}