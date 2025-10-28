// ******************************************************************
//       /\ /|       @file       SmartReloadManager.cs
//       \ V/        @brief      智能装弹管理器 - 核心协调器
//       | "")       @author     Catarina·RabbitNya, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2025-01-27 16:45:00
//    *(__\_\        @Copyright  Copyright (c) 2025, Shadowrabbit
// ******************************************************************

using SodaCraft.Localizations;
using UnityEngine;

namespace SmartReloader.Patch;

/// <summary>
/// 智能装弹管理器 - 封装所有装弹逻辑
/// </summary>
public class SmartReloadManager(ItemAgent_Gun gun)
{
    private readonly GunStateManager _stateManager = new(gun);

    /// <summary>
    /// 尝试装弹 - 主要逻辑入口
    /// </summary>
    public bool TryReload()
    {
        // 1. 尝试原始装弹逻辑
        if (_stateManager.TryOriginalReload())
        {
            return true;
        }

        // 2. 尝试智能装弹
        if (TrySmartReload())
        {
            return true;
        }

        // 3. 显示子弹不足提示
        ShowOutOfAmmoMessage();
        return false;
    }

    /// <summary>
    /// 尝试智能装弹 - 寻找同尺寸子弹
    /// </summary>
    private bool TrySmartReload()
    {
        if (!CanPerformSmartReload())
        {
            return false;
        }

        var compatibleBullet = BulletFinder.FindCompatibleBullet(
            gun.Holder.CharacterItem.Inventory,
            GetGunCaliber()
        );
        if (compatibleBullet == null)
        {
            return false;
        }

        // 设置偏好子弹并重新尝试装弹
        gun.GunItemSetting.PreferdBulletsToLoad = compatibleBullet;
        var success = _stateManager.TryOriginalReload();
        if (success)
        {
            Debug.Log($"[SmartReloader]Auto reload successful: {compatibleBullet.DisplayName}");
        }

        return success;
    }

    /// <summary>
    /// 检查是否可以执行智能装弹
    /// </summary>
    private bool CanPerformSmartReload()
    {
        return gun.Holder?.CharacterItem?.Inventory != null &&
               gun.GunItemSetting != null &&
               !string.IsNullOrEmpty(GetGunCaliber());
    }

    /// <summary>
    /// 获取枪械口径
    /// </summary>
    private string GetGunCaliber()
    {
        return gun.Item.Constants.GetString("Caliber");
    }

    /// <summary>
    /// 显示子弹不足提示
    /// </summary>
    private void ShowOutOfAmmoMessage()
    {
        if (gun.Holder == null || gun.GunItemSetting.BulletCount > 0)
        {
            return;
        }

        var message = "Poptext_OutOfAmmo".ToPlainText();
        gun.Holder.PopText(message);
    }
}