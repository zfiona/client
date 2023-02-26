using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;
using System;

public static class XChartLuaHelper
{
    public static MainComponent GetChartComponent(BaseChart chart,string componentName)
    {
        var components = chart.components;

        foreach (var component in components)
        {
            if (component.GetType().Name==componentName)
            {
               
                return component;
            }
        }
        return null;
    }
}
