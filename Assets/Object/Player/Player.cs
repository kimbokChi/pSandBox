﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Inventory Inventory;
    public PlayerCarryItem CarryItem;
    [Tooltip("플레이어가 근처 아이템을 감지할 때 사용되는 레이더입니다.")]
    public ItemRadar Radar;
    public bool FlipX
    {
        get { return sprite.flipX; }
    }

    #region 변수 설명 : 
    /// <summary>
    /// 플레이어의 상호작용 범위를 지정합니다.
    /// </summary>
    #endregion
    [Tooltip("플레이어의 상호작용 범위를 지정합니다.")]
    public float InteractionRange = 1;

    public Brake  LeftBrake;
    public Brake RightBrake;

    private Vector3 vDir;
    private SpriteRenderer sprite;

    [Tooltip("플레이어가 현재 프레임에 장비한 아이템 슬롯들을 담는 배열")]
    public  List<ItemSlot> EquipItemSlots    = new List<ItemSlot>();
    [Tooltip("플레이어가 이전 프레임에 장비한 아이템 슬롯들을 담는 배열")]
    private List<ItemList> EquippedItemSlots = new List<ItemList>();

    #region 함수 설명 : 
    /// <summary>
    /// 플레이어가 가진 브레이크를 통해, 이동하려는 방향으로 나아갈 수 있는지를 판단하는 함수.
    /// </summary>
    /// <param name="moveDir">
    /// 플레이어가 움직이려는 방향. 
    /// <para>
    /// ※ Vector2.left와 같이 방향을 나타내는 벡터를 하십시오 ※
    /// </para>
    /// </param>
    #endregion 
    public bool CheckBrakeOper(Vector2 moveDir)
    {
        if (LeftBrake.Exit && moveDir.Equals(Vector2.left))
        {
            return true;
        }
        else if (RightBrake.Exit && moveDir.Equals(Vector2.right))
        {
            return true;
        }
        return false;
    }

    #region 함수 설명 : 
    /// <summary>
    /// 플레이어가 들고있는 아이템들을 모두 사용하는 함수입니다.
    /// </summary>
    /// <param name="interactionID">
    /// 플레이어가 상호작용한 대상의 GetInstanceID를 지정합니다   
    /// </param>
    #endregion
    private void UseItem(int interactionID)
    {
        for (int i = 0; i < EquipItemSlots.Count; ++i)
        {
            if (EquipItemSlots[i])
            {
                if (EquipItemSlots[i].ContainItem)
                {
                    if (EquipItemSlots[i].ContainItem.TryGetComponent(out IItemFunction function))
                    {
                        if(function.HasFunction(ItemFunc.USE))
                        {
                            Player_Interaction.Instance.InObjGetValue(interactionID).OperateAction(function);
                        }
                        else
                        {
                            Player_Interaction.Instance.InObjGetValue(interactionID).OperateAction<IItemFunction>(null);
                        }
                    }
                    else
                    {
                        Player_Interaction.Instance.InObjGetValue(interactionID).OperateAction<IItemFunction>(null);
                    }
                }
            }
        }
       
    }

    private void OperateCarryItem()
    {
        for (int i = 0; i < EquipItemSlots.Count; i++)
        {
            if (EquipItemSlots[i].ContainItem)
            {
                if (EquipItemSlots[i].ContainItem.TryGetComponent(out IItemFunction function))
                {
                    if (function.HasFunction(ItemFunc.CARRY))
                    {
                        StartCoroutine(function.CarryItem(EquipItemSlots[i]));
                    }
                }
            }
        }
    }

    private void OperateMountAndUnmountItem()
    {
        IItemFunction function;

        for (int i = 0; i < EquipItemSlots.Count; i++)
        {
            // 새로운 아이템이 들어왓을 때
            if(EquippedItemSlots[i] == ItemList.NONE && EquipItemSlots[i].ContainItem != null)
            {
                if (EquipItemSlots[i].ContainItem.TryGetComponent(out function))
                {
                    if(function.HasFunction(ItemFunc.MOUNT))
                    {
                        StartCoroutine(function.MountItem());
                    }
                    EquippedItemSlots[i] = EquipItemSlots[i].ContainItem.ItemData;
                }
            }
            // 아이템이 나갔을 때
            if (EquippedItemSlots[i] != ItemList.NONE && EquipItemSlots[i].ContainItem == null)
            {
                if (ItemMaster.Instance.GetItem(EquippedItemSlots[i]).TryGetComponent(out function))
                {
                    if(function.HasFunction(ItemFunc.UNMOUNT))
                    {
                        StartCoroutine(function.UnmountItem());
                    }
                    EquippedItemSlots[i] = ItemList.NONE;
                }               
            }
        }

    }

    private void OnEnable()
    {
        vDir = transform.position;

        for(int i = 0; i < EquipItemSlots.Count; i++)
        {
            EquippedItemSlots.Add(ItemList.NONE);
        }

        sprite = gameObject.GetComponent<SpriteRenderer>();

        StateStorage.Instance.IncreaseState(States.TREE_LOGGING, 1);

        StartCoroutine(CR_update());
    }

    private IEnumerator CR_update()
    {
        while (gameObject.activeSelf)
        {
            // 이동 방향은 현재 플레이어의 위치로 계속해서 초기화한다.
            vDir = transform.position;

            OperateMountAndUnmountItem();

            OperateCarryItem();

            if(Input.GetKeyDown(KeyCode.Z))
            {
                if(Radar.GetCloseItem() != null)
                {
                    Player_Instructions.Instance.FollowInstr(Instructions.DO_INTERACT, Radar.GetCloseItemID());
                }
            }

            if (Input.GetAxis("Horizontal") != 0)
            {
                Player_Instructions.Instance.DiscontinueInstr();

                if (Input.GetAxis("Horizontal") > 0)
                {
                    if(CheckBrakeOper(Vector2.right))
                    {
                        yield return StartCoroutine(CR_Vibration(0.06f, 0.25f));
                        continue;
                    }

                    sprite.flipX = false;
                }
                else if (Input.GetAxis("Horizontal") < 0)
                {
                    if (CheckBrakeOper(Vector2.left))
                    {
                        yield return StartCoroutine(CR_Vibration(0.06f, 0.25f));
                        continue;
                    }

                    sprite.flipX = true;
                }
                vDir.x += Time.deltaTime * 3.5f * Input.GetAxis("Horizontal");

                transform.position = vDir;
            }

            CarryItem.Patch(sprite.flipX);

            yield return null;
        }

        yield break;
    }

    #region 코루틴 설명
    /// <summary>
    /// 플레이어가 상호작용 대상으로 이동하는 코루틴.
    /// </summary>
    /// <param name="interactObj">
    /// 상호작용할 오브젝트의 GetInstanceID()를 담느다.
    /// </param>
    #endregion
    public IEnumerator CR_Interaction(int interactObj)
    {
        // 플레이어와 상호작용 대상과의 거리가 InteractionRange보다 작다면, 상호작용 대상을 향해 이동한다.
        if (AbsDistance(Player_Interaction.Instance.InObjGetValue(interactObj).InteractObject().transform.position.x, transform.position.x) > InteractionRange)
        {
            Player_Instructions.Instance.FollowInstr(Instructions.GOTO_INSTR, interactObj);

            Player_Instructions.Instance.ScheduleInstr(InstrTrigger.NEXT_INSTR_UNINTERRUPTED_DONE, Instructions.DO_INTERACT, interactObj);

            yield break;
        }

        StartCoroutine(CR_Vibration(0.06f, 0.25f));

        bool isSlotEmpty = true;

        for(int i = 0; i < EquipItemSlots.Count; i++)
        {
            if(EquipItemSlots[i].ContainItem)
            {
                UseItem(interactObj);

                isSlotEmpty = false;

                break;
            }
        }
        if(isSlotEmpty)
        {
            Player_Interaction.Instance.InObjGetValue(interactObj).OperateAction<IItemFunction>(null);
        }
        

        Player_Instructions.Instance.CompletionInstr();
        yield break;
    }

    #region 코루틴 설명
    /// <summary>
    /// 플레이어가 지정한 위치로 이동하는 코루틴입니다.
    /// </summary>
    /// <param name="targetPoint">
    /// 이동할 지점을 지정합니다
    /// </param>
    #endregion
    public IEnumerator CR_moveMovementPoint(Vector2 targetPoint)
    {
        float fMoveAmount = 0;

        if(targetPoint.x > transform.position.x)
        {
            sprite.flipX = false;

            while (targetPoint.x > transform.position.x)
            {
                if (fMoveAmount < 1)
                {
                    fMoveAmount += 0.06f;
                }
                if (CheckBrakeOper(Vector2.right))
                {
                    yield return StartCoroutine(CR_Vibration(0.06f, 0.25f));

                    Player_Instructions.Instance.DiscontinueInstr();
                }
                vDir.x += fMoveAmount * Time.deltaTime * 3.5f;

                transform.position = vDir;

                yield return null;
            }
            while (fMoveAmount > 0)
            {
                fMoveAmount -= 0.16f;

                vDir.x += fMoveAmount * Time.deltaTime * 3.5f;
                transform.position = vDir;

                yield return null;
            }
        }

        else if (targetPoint.x < transform.position.x)
        {
            sprite.flipX = true;

            while (targetPoint.x < transform.position.x)
            {
                if (fMoveAmount < 1)
                {
                    fMoveAmount += 0.06f;
                }
                if (CheckBrakeOper(Vector2.left))
                {
                    yield return StartCoroutine(CR_Vibration(0.06f, 0.25f));

                    Player_Instructions.Instance.DiscontinueInstr();
                }
                vDir.x -= fMoveAmount * Time.deltaTime * 3.5f;

                transform.position = vDir;

                yield return null;
            }
            while (fMoveAmount > 0)
            {
                fMoveAmount -= 0.16f;

                vDir.x -= fMoveAmount * Time.deltaTime * 3.5f;
                transform.position = vDir;

                yield return null;
            }
        }
        Player_Instructions.Instance.CompletionInstr();
        yield break;
    }

    #region 코루틴 설명
    /// <summary>
    /// 플레이어가 지정한 오브젝트를 추적하는 코루틴입니다.
    /// </summary>
    /// <param name="target">
    /// 추적할 오브젝트를 지정합니다.
    /// </param>
    #endregion
    public IEnumerator CR_moveMovementPoint(GameObject target)
    {
        float fMoveAmount = 0;
        Transform Target  = target.transform;

        if (Target.position.x > transform.position.x)
        {
            sprite.flipX = false;

            while (Target.position.x > transform.position.x)
            {
                if (fMoveAmount < 1)
                {
                    fMoveAmount += 0.06f;
                }
                if (CheckBrakeOper(Vector2.right))
                {
                    yield return StartCoroutine(CR_Vibration(0.06f, 0.25f));

                    Player_Instructions.Instance.DiscontinueInstr();
                }
                vDir.x += fMoveAmount * Time.deltaTime * 3.5f;

                transform.position = vDir;

                yield return null;
            }
            while (fMoveAmount > 0)
            {
                fMoveAmount -= 0.16f;

                vDir.x += fMoveAmount * Time.deltaTime * 3.5f;
                transform.position = vDir;

                yield return null;
            }
        }

        else if (Target.position.x < transform.position.x)
        {
            sprite.flipX = true;

            while (Target.position.x < transform.position.x)
            {
                if (fMoveAmount < 1)
                {
                    fMoveAmount += 0.06f;
                }
                if (CheckBrakeOper(Vector2.left))
                {
                    yield return StartCoroutine(CR_Vibration(0.06f, 0.25f));

                    Player_Instructions.Instance.DiscontinueInstr();
                }
                vDir.x -= fMoveAmount * Time.deltaTime * 3.5f;

                transform.position = vDir;

                yield return null;
            }
            while (fMoveAmount > 0)
            {
                fMoveAmount -= 0.16f;

                vDir.x -= fMoveAmount * Time.deltaTime * 3.5f;
                transform.position = vDir;

                yield return null;
            }
        }

        Player_Instructions.Instance.CompletionInstr();
        yield break;
    }

    #region 코루틴 설명
    /// <summary>
    /// 플레이어가 지정한 상호작용 대상을 향해 이동하는 코루틴입니다.
    /// </summary>
    /// <param name="interactionID">
    /// 상호작용할 대상의 GetInstanceID()를 지정합니다
    /// </param>
    #endregion
    public IEnumerator CR_moveMovementPoint(int interactionID)
    {
        float fMoveAmount = 0;
        Transform IntractObj = Player_Interaction.Instance.InObjGetValue(interactionID).InteractObject().transform;

        if (IntractObj.position.x > transform.position.x + InteractionRange)
        {
            sprite.flipX = false;

            while (IntractObj.position.x > transform.position.x + InteractionRange)
            {
                if (fMoveAmount < 1)
                {
                    fMoveAmount += 0.06f;
                }
                if (CheckBrakeOper(Vector2.right))
                {
                    yield return StartCoroutine(CR_Vibration(0.06f, 0.25f));

                    Player_Instructions.Instance.DiscontinueInstr();
                }
                vDir.x += fMoveAmount * Time.deltaTime * 3.5f;

                transform.position = vDir;

                yield return null;
            }
            while (fMoveAmount > 0)
            {
                fMoveAmount -= 0.16f;

                vDir.x += fMoveAmount * Time.deltaTime * 3.5f;
                transform.position = vDir;

                yield return null;
            }
        }

        else if (IntractObj.position.x < transform.position.x - InteractionRange)
        {
            sprite.flipX = true;

            while (IntractObj.position.x < transform.position.x - InteractionRange)
            {
                if (fMoveAmount < 1)
                {
                    fMoveAmount += 0.06f;
                }
                if (CheckBrakeOper(Vector2.left))
                {
                    yield return StartCoroutine(CR_Vibration(0.06f, 0.25f));

                    Player_Instructions.Instance.DiscontinueInstr();
                }
                vDir.x -= fMoveAmount * Time.deltaTime * 3.5f;

                transform.position = vDir;

                yield return null;
            }
            while (fMoveAmount > 0)
            {
                fMoveAmount -= 0.16f;

                vDir.x -= fMoveAmount * Time.deltaTime * 3.5f;
                transform.position = vDir;

                yield return null;
            }
        }
        Player_Instructions.Instance.CompletionInstr();
        yield break;
    }

    private IEnumerator CR_Vibration(float amount, float time)
    {
        Vector2 vInitPos = transform.position;

        while(time > 0)
        {
            time -= Time.deltaTime;

            transform.position = ((Vector2)UnityEngine.Random.insideUnitSphere * amount) + vInitPos;

            yield return null;
        }
        transform.position = vInitPos;

        yield break;
    }

    #region 함수 설명 :
    /// <summary>
    /// 수직선상 두 점 사이 거리의 절댓값을 반환합니다.
    /// </summary>
    /// <param name="pointA">
    /// 수직선상에 위치한 한 점입니다.
    /// </param>
    /// <param name="pointB">
    /// 수직선상에 위치한 한 점입니다.
    /// </param>
    /// <returns></returns>
    #endregion
    private float AbsDistance(float pointA, float pointB)
    {
        if(pointA > pointB)
        {
            return pointA - pointB;
        }
        else if (pointA < pointB)
        {
            return pointB - pointA;
        }

        return 0;
    }
}