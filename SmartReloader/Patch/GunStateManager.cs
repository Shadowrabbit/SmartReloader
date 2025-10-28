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
            return false;
        }

        // 重置连发计数器
        ResetBurstCounter();
        // 设置目标子弹类型
        SetTargetBulletType();
        // 自动选择子弹类型
        if (!AutoSelectBulletType())
        {
            return false;
        }

        // 检查是否已满
        if (IsAlreadyFull())
        {
            return false;
        }

        // 检查子弹数量
        if (!HasEnoughBullets())
        {
            return false;
        }

        // 开始装弹
        StartReloadProcess();
        return true;
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

        var bulletCount = gun.GunItemSetting.GetBulletCountofTypeInInventory(
            gun.GunItemSetting.TargetBulletID,
            gun.Holder.CharacterItem.Inventory
        );

        return bulletCount > 0;
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