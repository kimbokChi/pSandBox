﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRod : Item, ItemFunction
{
    public IEnumerator CarryItem(ItemSlot itemSlot)
    {
        if (!StartWorking(ref _isCarryItem)) yield break;

        if (MouseCursor.Instance.RightClickVoid)
        {
            itemSlot.SetItem(ItemMaster.ItemList.FISHING_ROD_USED);
        }

        StopWorking(ref _isCarryItem);
        yield break;
    }

    public IEnumerator MountItem()
    {
        yield break;
    }

    public IEnumerator InSlotItem()
    {
        yield break;
    }

    public IEnumerator UseItem<T>(T xValue) where T : Interaction
    {
        yield break;
    }

    protected override void Init()
    {
        _itemCode = (int)ItemMaster.ItemList.FISHING_ROD;

        _itemType = ItemMaster.ItemType.TOOL;
    }

    public IEnumerator UnmountItem()
    {
        yield break;
    }
}
