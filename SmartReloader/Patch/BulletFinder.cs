// ******************************************************************
//       /\ /|       @file       BulletFinder.cs
//       \ V/        @brief      子弹查找器 - 负责寻找兼容子弹
//       | "")       @author     Catarina·RabbitNya, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2025-01-27 16:45:00
//    *(__\_\        @Copyright  Copyright (c) 2025, Shadowrabbit
// ******************************************************************

using ItemStatsSystem;

namespace SmartReloader.Patch;

/// <summary>
/// 子弹查找器 - 负责寻找兼容子弹
/// </summary>
public static class BulletFinder
{
    /// <summary>
    /// 查找兼容的子弹
    /// </summary>
    public static Item? FindCompatibleBullet(Inventory inventory, string caliber)
    {
        if (inventory == null || string.IsNullOrEmpty(caliber))
        {
            return null;
        }

        foreach (var item in inventory)
        {
            if (IsCompatibleBullet(item, caliber))
            {
                return item;
            }
        }

        return null;
    }

    /// <summary>
    /// 检查物品是否是兼容的子弹
    /// </summary>
    /// <param name="item">要检查的物品</param>
    /// <param name="caliber">目标口径</param>
    /// <returns>是否兼容</returns>
    private static bool IsCompatibleBullet(Item item, string caliber)
    {
        if (item == null) return false;
        if (!item.GetBool("IsBullet")) return false;
        var itemCaliber = item.Constants.GetString("Caliber");
        if (string.IsNullOrEmpty(itemCaliber) || itemCaliber != caliber) return false;
        return item.StackCount > 0;
    }
}