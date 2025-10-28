// ******************************************************************
//       /\ /|       @file       GunStateManager.cs
//       \ V/        @brief      枪械状态管理器 - 处理原始装弹逻辑
//       | "")       @author     Catarina·RabbitNya, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2025-01-27 16:45:00
//    *(__\_\        @Copyright  Copyright (c) 2025, Shadowrabbit
// ******************************************************************

namespace SmartReloader.Patch;

/// <summary>
/// 枪械状态管理器 - 处理原始装弹逻辑
/// </summary>
public class GunStateManager(ItemAgent_Gun gun)
{
    /// <summary>
    /// 尝试原始装弹逻辑（不显示提示）
    /// </summary>
    public bool TryOriginalReload()
    {
        // 检查枪械状态
        if (!IsValidGunState())
        {
            return true;
        }

        // 重置连发计数器
        ResetBurstCounter();
        // 设置目标子弹类型
        SetTargetBulletType();
        // 自动选择子弹类型
        if (!AutoSelectBulletType())
        {
            return true;
        }

        // 检查是否已满
        if (IsAlreadyFull())
        {
            return true;
        }

        if (gun.GunItemSetting.PreferdBulletsToLoad != null)
        {
            return true;
        }

        // 背包内目标类型子弹数量
        var inventoryCount = gun.GunItemSetting.GetBulletCountofTypeInInventory(
            gun.GunItemSetting.TargetBulletID,
            gun.Holder.CharacterItem.Inventory
        );

        switch (inventoryCount)
        {
            // 情况1：枪与背包都无子弹 -> 返回false，后续走智能装填或提示
            case <= 0 when gun.BulletCount <= 0:
                return false;
            // 情况2：背包里仍有该类型子弹 -> 返回true，继续原始装填，不触发智能装填
            case > 0:
                StartReloadProcess();
                return true;
            default:
                // 其余情况（如枪内有子弹但背包没有该类型）-> 返回false，允许后续智能装填介入
                return true;
        }
    }

    /// <summary>
    /// 检查枪械状态是否允许装弹
    /// </summary>
    private bool IsValidGunState()
    {
        return gun.GunState is ItemAgent_Gun.GunStates.ready or ItemAgent_Gun.GunStates.empty
            or ItemAgent_Gun.GunStates.shootCooling;
    }

    /// <summary>
    /// 重置连发计数器
    /// </summary>
    private void ResetBurstCounter()
    {
        ReflectionHelper.SetField(gun, "burstCounter", 0);
    }

    /// <summary>
    /// 设置目标子弹类型
    /// </summary>
    private void SetTargetBulletType()
    {
        if (gun.GunItemSetting.PreferdBulletsToLoad != null)
        {
            gun.GunItemSetting.SetTargetBulletType(gun.GunItemSetting.PreferdBulletsToLoad);
        }
    }

    /// <summary>
    /// 自动选择子弹类型
    /// </summary>
    private bool AutoSelectBulletType()
    {
        if (gun.GunItemSetting.TargetBulletID == -1)
        {
            gun.GunItemSetting.AutoSetTypeInInventory(gun.Holder.CharacterItem.Inventory);
        }

        return gun.GunItemSetting.TargetBulletID != -1;
    }

    /// <summary>
    /// 检查是否已经装满相同类型的子弹
    /// </summary>
    private bool IsAlreadyFull()
    {
        var currentBulletTypeID = -1;
        var currentLoadedBullet = gun.GunItemSetting.GetCurrentLoadedBullet();
        if (currentLoadedBullet != null)
        {
            currentBulletTypeID = currentLoadedBullet.TypeID;
        }

        return gun.BulletCount >= gun.Capacity &&
               currentBulletTypeID == gun.GunItemSetting.TargetBulletID;
    }

    /// <summary>
    /// 检查是否有足够的子弹
    /// </summary>
    private bool HasEnoughBullets()
    {
        if (gun.GunItemSetting.PreferdBulletsToLoad != null)
        {
            return true;
        }

        // 背包内目标类型子弹数量
        var inventoryCount = gun.GunItemSetting.GetBulletCountofTypeInInventory(
            gun.GunItemSetting.TargetBulletID,
            gun.Holder.CharacterItem.Inventory
        );

        // 情况1：枪与背包都无子弹 -> 返回false，后续走智能装填或提示
        if (inventoryCount <= 0 && gun.BulletCount <= 0)
        {
            return true;
        }

        // 情况2：背包里仍有该类型子弹 -> 返回true，继续原始装填，不触发智能装填
        if (inventoryCount > 0)
        {
            return true;
        }

        // 其余情况（如枪内有子弹但背包没有该类型）-> 返回false，允许后续智能装填介入
        return false;
    }

    /// <summary>
    /// 开始装弹流程
    /// </summary>
    private void StartReloadProcess()
    {
        ReflectionHelper.SetField(gun, "gunState", ItemAgent_Gun.GunStates.reloading);
        ReflectionHelper.SetField(gun, "stateTimer", 0f);
        ReflectionHelper.InvokeMethod(gun, "PostStartReloadSound");
    }
}