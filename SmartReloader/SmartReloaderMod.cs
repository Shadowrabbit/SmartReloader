// ******************************************************************
//       /\ /|       @file       ModBehaviour.cs
//       \ V/        @brief      SmartReloader Mod入口类
//       | "")       @author     Catarina·RabbitNya, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2025-01-27 03:12:31
//    *(__\_\        @Copyright  Copyright (c) 2025, Shadowrabbit
// ******************************************************************

using HarmonyLib;
using UnityEngine;

namespace SmartReloader;

/// <summary>
/// BulletEjector Mod主入口类
/// 负责初始化Harmony补丁和Mod功能
/// </summary>
public class ModBehaviour : Duckov.Modding.ModBehaviour
{
    /// <summary>
    /// Harmony实例，用于应用补丁
    /// </summary>
    private Harmony _harmony = null!;

    /// <summary>
    /// Mod初始化方法
    /// </summary>
    protected override void OnAfterSetup()
    {
        // 创建Harmony实例
        _harmony = new Harmony("SmartReloader");
        // 应用所有补丁
        _harmony.PatchAll();
        // 输出初始化日志
        Debug.Log("[SmartReloader] Mod initialization completed, Harmony patches applied");
    }

    /// <summary>
    /// Mod销毁方法
    /// </summary>
    protected override void OnBeforeDeactivate()
    {
        // 移除我们添加的补丁
        _harmony.UnpatchAll("SmartReloader");
        Debug.Log("[SmartReloader] Mod unloaded, Harmony patches removed");
    }
}