using UnityEngine;
using System.Collections;

public class ItemData : GameData<ItemData> {

    public static readonly string fileName = "xml/item";

    public static ItemData GetItemDataByID(int id)
    {
        if (!ItemData.dataMap.ContainsKey(id))
            return null;

        ItemData itemData = ItemData.dataMap[id];

        return itemData;
    }
    /// <summary>
    /// 道具ID
    /// </summary>
    public int itemid { get; protected set; }

    /// <summary>
    /// 道具名
    /// </summary>
    public int itemName { get; protected set; }

    public string nRemarks { get; protected set; }

    public int description { get; protected set; }

    public string dRemarks { get; protected set; }

    public int icon { get; protected set; }

    public int itemType { get; protected set; }
}
