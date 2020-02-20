﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region 클래스 설명 : 
/// <summary>
/// 아이템 슬롯에 띄워지는 아이템의 스프라이트를 가지는 오브젝트의 클래스.
/// <para>
/// ※ 아이템 슬롯은 이 클래스에 대한 정보를 가지고 있어야 하며, 이 클래스를 가진 오브젝트는 아이템 슬롯의 자식이여야 합니다. ※
/// </para>
/// </summary>
#endregion
public class ItemSlotSprt : MonoBehaviour
{
    public SpriteRenderer Renderer;

    #region 함수 설명 : 
    /// <summary>
    /// 특정 아이템의 스프라이트를 띄우게 하는 함수.
    /// </summary>
    /// <param name="itemCode">
    /// 띄울 아이템 스프라이트의 아이템 코드.
    /// </param>
    #endregion
    public void ShowItemSprt(int itemCode)
    {
        Renderer.sprite = ItemMaster.Instance.GetItemSpr(itemCode);
    }

    #region 함수 설명 : 
    /// <summary>
    /// 특정 아이템의 스프라이트를 띄우게 하는 함수.
    /// </summary>
    /// <param name="item">
    /// 띄울 아이템의 ItemMaster.ItemList의 열거자.
    /// </param>
    #endregion
    public void ShowItemSprt(ItemMaster.ItemList item)
    {
        Renderer.sprite = ItemMaster.Instance.GetItemSpr(item);
    }

    #region 함수 설명 : 
    /// <summary>
    /// 더 이상 스프라이트 띄우지 마!
    /// </summary>
    #endregion
    public void HideItemSprt()
    {
        Renderer.sprite = null;
    }
}