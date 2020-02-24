﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : Item
{
    protected override void Init()
    {
        _itemCode = (int)ItemMaster.ItemList.AXE;
    }
    public override void UseItem()
    {
        Debug.Log("AXE!!!");
    }
}
