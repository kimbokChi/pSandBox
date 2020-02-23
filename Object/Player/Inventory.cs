﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public ItemSlot[] itemSlots;
    public ItemSlot CarryItemSlot;

    private sbyte empty = -1;

    public void AddItemInventory(ItemSprt item)
    {
        int emptySlotIndex = empty;

        for(int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].ContainItem == ItemMaster.ItemList.NONE)
            {
                if (emptySlotIndex.Equals(empty))
                {
                    emptySlotIndex = i;
                }
                continue;
            }

            if(itemSlots[i].ContainItem == item.ItemCode)
            {
                item.gameObject.SetActive(false);

                itemSlots[i].AddItem(item.ItemCode);
                ItemMaster.Instance.LoadItem(item);
                return;
            }
        }
        if(!emptySlotIndex.Equals(empty))
        {
            item.gameObject.SetActive(false);

            itemSlots[emptySlotIndex].AddItem(item.ItemCode);
            return;
        }
        Debug.LogWarning("인벤토리가 가득 차 있습니다");
    }



}
