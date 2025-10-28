// ******************************************************************
//       /\ /|       @file       PatchItemAgentGun.cs
//       \ V/        @brief      智能装弹系统 - 自动寻找同尺寸子弹
//       | "")       @author     Catarina·RabbitNya, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2025-01-27 16:30:00
//    *(__\_\        @Copyright  Copyright (c) 2025, Shadowrabbit
// ******************************************************************

using HarmonyLib;
using JetBrains.Annotations;

namespace SmartReloader.Patch;

/// <summary>
/// 智能装弹系统 - 当装弹失败时自动寻找同尺寸子弹
/// </summary>
[HarmonyPatch(typeof(ItemAgent_Gun), nameof(ItemAgent_Gun.BeginReload))]
public static class PatchItemAgentGun
{
    /// <summary>
    /// Prefix补丁 - 拦截原始装弹逻辑并增强
    /// </summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    public static bool BeginReload_Prefix(ItemAgent_Gun __instance, ref bool __result)
    {
        var reloadManager = new SmartReloadManager(__instance);
        __result = reloadManager.TryReload();
        return false; // 跳过原始方法
    }
}