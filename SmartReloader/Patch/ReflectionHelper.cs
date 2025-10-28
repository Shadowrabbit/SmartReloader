// ******************************************************************
//       /\ /|       @file       ReflectionHelper.cs
//       \ V/        @brief      反射助手 - 简化反射操作
//       | "")       @author     Catarina·RabbitNya, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2025-01-27 16:45:00
//    *(__\_\        @Copyright  Copyright (c) 2025, Shadowrabbit
// ******************************************************************

using System.Reflection;

namespace SmartReloader.Patch;

/// <summary>
/// 反射助手 - 简化反射操作
/// </summary>
public static class ReflectionHelper
{
    /// <summary>
    /// 设置字段值
    /// </summary>
    /// <param name="instance">实例对象</param>
    /// <param name="fieldName">字段名称</param>
    /// <param name="value">要设置的值</param>
    public static void SetField(object instance, string fieldName, object value)
    {
        var field = instance.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        field?.SetValue(instance, value);
    }

    /// <summary>
    /// 调用方法
    /// </summary>
    /// <param name="instance">实例对象</param>
    /// <param name="methodName">方法名称</param>
    public static void InvokeMethod(object instance, string methodName)
    {
        var method = instance.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        method?.Invoke(instance, null);
    }
}