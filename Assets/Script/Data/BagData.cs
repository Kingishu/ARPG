using Game.Config;
using System;
using System.Collections.Generic;
using UnityEngine;
public enum BagError
{
    BagMax,
    CountError
}

public class BagData
{
    private static BagData instance = new BagData();
    public static BagData Instance => instance;
    private BagData() { }
    public int maxCount = 54;

    public Dictionary<int, BagEntity> dct = new Dictionary<int, BagEntity>();

    // ==========================================
    // 1. 添加物品 (修正：使用传入的 count，而不是写死 1)
    // ==========================================
    public void Add(int id, int count, Action<BagEntity> callback,Action<BagError> error=null)
    {
        if (count<=0)
        {
            return;
        }
        bool HasItem = false;//是否有这个物体
        bool CanCap = false;//是否可以堆叠
        int GrildID = -1;

        foreach (var item in dct.Values)
        {
            if (item.id == id)
            {
                HasItem = true;
                if (item.entity.superposition > 0)
                {
                    CanCap = true;
                    GrildID = item.grild_id;
                    break; // 找到了可堆叠的，直接退出循环
                }
            }
        }

        BagEntity targetEntity = null;

        if (HasItem && CanCap)
        {
            dct[GrildID].count += count;
            targetEntity = dct[GrildID];
        }
        else
        {
            if (dct.Count>=maxCount)
            {
                error?.Invoke(BagError.BagMax);
                return;
            }
            BagEntity entity = new BagEntity();
            entity.id = id;
            entity.count = count;
            entity.entity = PropData.Get(entity.id);

            // 找空位插空
            bool foundEmpty = false;
            for (global::System.Int32 i = 0; i < maxCount; i++)
            { 
                if (dct.ContainsKey(i))
                {
                    continue;
                }
                else
                {
                    entity.grild_id = i;
                    dct[entity.grild_id] = entity; // 入库
                    targetEntity = entity;
                    foundEmpty = true;
                    break; // 找到了就退出
                }
            }

            // 如果中间没空位，追加到最后
            if (!foundEmpty)
            {
                entity.grild_id = dct.Count;
                dct[entity.grild_id] = entity; // 入库
                targetEntity = entity;
            }
        }

        // 调用回调通知UI更新
        callback?.Invoke(targetEntity);
    }
    

    // ==========================================
    // 2. 根据格子ID移除
    // ==========================================
    public void Remove_Grild_ID(int _grild_id, int count, Action<BagEntity> callback)
    {
        if (count <= 0) return;

        // 背包中没有这个格子
        if (dct.ContainsKey(_grild_id) == false) return;

        // 有这个格子
        else
        {
            BagEntity bagEntity = dct[_grild_id];

            // 如果要删的数量 >= 拥有的数量，直接移除
            if (count >= bagEntity.count)
            {
                // 在移除前先通知UI（或者UI层处理移除逻辑），通常根据业务需求
                // 这里先将count设为0，通知UI，然后再从字典移除
                bagEntity.count = 0;
                callback?.Invoke(bagEntity);

                dct.Remove(_grild_id);
            }
            else
            {
                bagEntity.count -= count;
                callback?.Invoke(bagEntity);
            }
        }
    }

    // ==========================================
    // 3. 根据物品ID移除 (核心逻辑填充)
    // ==========================================
    public void Remove_ID(int id, int count, Action<BagEntity> callback)
    {
        if (count <= 0) return;

        // 记录需要彻底删除的格子ID (防止在foreach中修改字典报错)
        List<int> gridsToRemove = new List<int>();

        foreach (var item in dct.Values)
        {
            if (item.id == id)
            {
                // 情况A: 当前格子够扣，或者扣完还有剩
                if (item.count > count)
                {
                    item.count -= count;
                    count = 0;
                    callback?.Invoke(item); // 通知UI更新数量
                    break; // 任务完成
                }
                // 情况B: 当前格子不够扣，或者刚好扣完
                else
                {
                    count -= item.count;
                    item.count = 0; // 显式设为0，方便UI更新逻辑判断

                    // 通知UI这个格子变0了 (UI层应该处理隐藏逻辑)
                    callback?.Invoke(item);

                    // 记下来待会儿删掉
                    gridsToRemove.Add(item.grild_id);
                }

                // 如果已经不需要再删了，退出循环
                if (count == 0) break;
            }
        }

        // 统一执行删除
        foreach (var gridId in gridsToRemove)
        {
            dct.Remove(gridId);
        }
    }

    // ==========================================
    // 4. 查询总数量
    // ==========================================
    public int Query(int id)
    {
        int count = 0;
        foreach (var item in dct.Values)
        {
            if (item.id == id)
            {
                count += item.count;
            }
        }
        return count;
    }

    // ==========================================
    // 5. 交换格子 (Modify) - 安全交换逻辑
    // ==========================================
    public void Modify_Grild(BagEntity entity1, BagEntity entity2, Action<BagEntity, BagEntity> callback)
    {
        // 1. 先更新字典 (这时候 entity 里的 ID 还是旧的，所以是安全的)
        dct[entity1.grild_id] = entity2;
        dct[entity2.grild_id] = entity1;

        // 2. 更新对象内部的 ID (必须用 temp！！！)
        int tempID = entity1.grild_id; // 先把 A 的ID 存进保险箱

        entity1.grild_id = entity2.grild_id; // A 变成 B 的ID
        entity2.grild_id = tempID;           // B 从保险箱里拿出 A 的旧ID

        // 3. 回调通知UI刷新这两个格子
        callback?.Invoke(entity1, entity2);
    }
    public BagEntity Get(int grild_id)
    {
        if (dct.TryGetValue(grild_id,out var e))
        {
            return e;
        }
        return null;
    }
    public void Modify_Grild(int grild_id1,int grild_id2, Action<BagEntity, BagEntity> callback)
    {
        if (grild_id1==grild_id2)
        {
            return;
        }
        //两种情况,grild_id2是空,或者grild_id2不是空
        //如果第二个位置是空,将第一个放过去,然后将原本的第一个清空
        //如果不是空,就交换位置
        BagEntity entity1= Get(grild_id1);
        BagEntity entity2= Get(grild_id2);
        if (entity2==null)
        {
            //将1放在2的位置上,然后清空1
            dct[grild_id2]=entity1;
            dct.Remove(grild_id1);
            entity1.grild_id = grild_id2;
        }
        else
        {
            dct[grild_id1]=entity2;
            dct[grild_id2]= entity1;
            int temp=entity1.grild_id;
            entity1.grild_id=entity2.grild_id;
            entity2.grild_id=temp;
        }
        callback?.Invoke(entity1, entity2);
    }
    public void SortByType()
    {
        // 1. 准备容器
        List<BagEntity> prop = new List<BagEntity>();
        List<BagEntity> equip = new List<BagEntity>();
        List<BagEntity> mat = new List<BagEntity>();

        // 2. 分类 (分桶)
        foreach (var item in dct.Values) // 直接遍历 Values 稍微省点事
        {
            if (item.entity.type == 0)
            {
                prop.Add(item);
            }
            else if (item.entity.type == 1)
            {
                equip.Add(item);
            }
            else
            {
                mat.Add(item);
            }
        }

        // 3. 桶内微调 (可选：既然分好了，顺手按ID排个序，让同类物品整齐排列)
        // 如果你不加这几行，虽然类型分开了，但同类型内部是乱序的
        prop.Sort((a, b) => a.id.CompareTo(b.id));
        equip.Sort((a, b) => a.id.CompareTo(b.id));
        mat.Sort((a, b) => a.id.CompareTo(b.id));

        // 4. 清空原背包
        dct.Clear();

        // 5. 重新入库 (关键：必须维护 currentIdx 指针)
        int currentIdx = 0;

        // --- 填入道具 ---
        for (int i = 0; i < prop.Count; i++)
        {
            BagEntity item = prop[i];
            item.grild_id = currentIdx; // 【严谨核心】必须更新物体内部的坐标
            dct.Add(currentIdx, item);
            currentIdx++;
        }

        // --- 填入装备 ---
        for (int i = 0; i < equip.Count; i++)
        {
            BagEntity item = equip[i];
            item.grild_id = currentIdx; // 【严谨核心】必须更新物体内部的坐标
            dct.Add(currentIdx, item);
            currentIdx++;
        }

        // --- 填入材料 ---
        for (int i = 0; i < mat.Count; i++)
        {
            BagEntity item = mat[i]; // 【修正】这里是你原本写错的地方，必须是 mat[i]
            item.grild_id = currentIdx; // 【严谨核心】必须更新物体内部的坐标
            dct.Add(currentIdx, item);
            currentIdx++;
        }
    }
}

public class BagEntity
{
    public int id;
    public int grild_id;
    public int count;
    public PropEntity entity;
}