﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, MouseAction
{
    private LinkedList<Item> _itemContainer = new LinkedList<Item>();
    
    public int ItemCount
    {
        get { return _itemContainer.Count; }
    }
    public Item ContainItem
    {
        get 
        {
            if (_itemContainer.Count == 0) return null;

            return _itemContainer.First.Value; 
        }
    }

    public Text text;

    private void OnEnable()
    {
        RegisterAction();

        UpdateItemCount();
    }

    /***************************************
           ItemSlot를 관리하는 함수들
    ****************************************/
    public void AddItem(params Item[] items)
    {
        int i = 0;

        if (ItemCount == 0)
        {
            _itemContainer.AddLast(items[i++]);
        }

        for (; i < items.Length; i++)
        {
            if (_itemContainer.First.Value.itemCode == items[i].itemCode)
            {
                _itemContainer.AddLast(items[i]);
            }
            else Debug.LogWarning("적합하지 않은 아이템은 추가할 수 없습니다.");
        }
        UpdateItemCount();
    }
    public void AddItem(Item item)
    {
        if (_itemContainer.Count == 0)
        {
            _itemContainer.AddLast(item);
        }

        else if (item.itemCode == _itemContainer.First.Value.itemCode)
        {
            _itemContainer.AddLast(item);          
        }
        UpdateItemCount();
    }

    public void UpdateItemCount()
    {
        text.text = ItemCount.ToString();
    }


    /******************************************
            MouseAction 인터페이스 함수들
     ******************************************/
    public void OperateAction(byte input)
    {
        switch (input)
        {
            case 0:
                if (MouseCursor.Instance.CarryItem != null)
                {
                    if (ContainItem == null)
                    {
                        _itemContainer.AddLast(MouseCursor.Instance.CarryItem);
                        MouseCursor.Instance.DelCarryItem();
                        UpdateItemCount();
                    }
                    else if (MouseCursor.Instance.CarryItem.itemCode == ContainItem.itemCode)
                    {
                        _itemContainer.AddLast(MouseCursor.Instance.CarryItem);
                        MouseCursor.Instance.DelCarryItem();
                        UpdateItemCount();
                    }
                }
                break;

            case 1:
                if (ItemCount > 0)
                {
                    if (MouseCursor.Instance.CarryItem == null)
                    {
                        MouseCursor.Instance.AddCarryItem(_itemContainer.Last.Value);
                        _itemContainer.RemoveLast();
                        UpdateItemCount();
                    }
                    else if (MouseCursor.Instance.CarryItem.itemCode == ContainItem.itemCode)
                    {
                        MouseCursor.Instance.AddCarryItem(_itemContainer.Last.Value);
                        _itemContainer.RemoveLast();
                        UpdateItemCount();
                    }
                }
                break;
        }             
    }

    public void RegisterAction()
    {
        MouseRepeater.Instance.AddActionObj(gameObject.GetInstanceID(), this);
    }

    public GameObject ActionObject()
    {
        return gameObject;
    }
}
