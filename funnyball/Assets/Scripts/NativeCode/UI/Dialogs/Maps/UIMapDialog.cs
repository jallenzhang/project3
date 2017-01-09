using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIMapDialog : BaseDialog<NullParam> {
    public static void PopUp()
    {
        DialogManager.Instance.PopupDialog("Prefabs/Dialogs/MapDialog");
    }

    private List<MapItem> m_mapItems = new List<MapItem>();
    private UIWarpContent m_wrapContent;
    private int initMapItemCount = 10;

    void Start()
    {
        //1. init initMapItemCount, read from config file, chapter count for default;
        for(int i = 0; i < initMapItemCount; i++)
        {
            MapItem mapItem = new MapItem("chapter" + i);
            mapItem.IsChapter = true;
            mapItem.StarsCount = i;
            mapItem.LevelCount = 10;
            mapItem.TotalStarsCount = 10;
            mapItem.Status = MapItemStatus.Collipsed;
            m_mapItems.Add(mapItem);
        }

        //scrollView 相关所需注意接口
        m_wrapContent = gameObject.transform.GetComponentInChildren<UIWarpContent>();
        m_wrapContent.onInitializeItem = onInitializeItem;
        //注意：目标init方法必须在warpContent.onInitializeItem之后
        m_wrapContent.Init(m_mapItems.Count);
    }

    private void onInitializeItem(GameObject go,int dataIndex)
    {
        Text txtTitle = go.transform.FindChild("Title").GetComponent<Text>();
        txtTitle.text = m_mapItems[dataIndex].Name;

        Text StarValue = go.transform.FindChild("GotValue").GetComponent<Text>();
        StarValue.text = m_mapItems[dataIndex].StarsCount + "/" + m_mapItems[dataIndex].TotalStarsCount;

        bool isChapter = m_mapItems[dataIndex].IsChapter;

        go.GetComponent<UIMapItem>().SetLevelAreaVisible(!isChapter);
        if (isChapter)
        {
            Button btn = go.transform.FindChild("BG").GetComponent<Button>();
            if (m_mapItems[dataIndex].Status == MapItemStatus.Collipsed)
                AddExpandListener(btn, dataIndex);
            else
                AddCompressListener(btn, dataIndex);
        }
    }

    private void AddExpandListener(Button btn, int chapterIndex)
    {
        btn.onClick.RemoveAllListeners();
        UnityAction ua = delegate()
        {
            btn.enabled = false;
            m_mapItems[chapterIndex].Status = MapItemStatus.Expanded;
            CoroutineManager.DoCoroutine(AddLevels(m_mapItems[chapterIndex].LevelCount, btn, chapterIndex));
        };
        btn.onClick.AddListener(ua);
        m_mapItems[chapterIndex].OnClickDelegate = ua;
    }

    private void AddCompressListener(Button btn, int chapterIndex)
    {
        btn.onClick.RemoveAllListeners();
        UnityAction ua = delegate()
        {
            btn.enabled = false;
            m_mapItems[chapterIndex].Status = MapItemStatus.Collipsed;
            CoroutineManager.DoCoroutine(RemoveLevels(m_mapItems[chapterIndex].LevelCount, btn, chapterIndex));
        };
        btn.onClick.AddListener(ua);
        m_mapItems[chapterIndex].OnClickDelegate = ua;
    }

    IEnumerator RemoveLevels(int levelCount, Button btn, int currentIndex)
    {
        int orignalIndex = currentIndex;
        for (int i = 0; i < levelCount; i++)
        {
            m_mapItems.RemoveAt(currentIndex + 1);
            m_wrapContent.DelItem(currentIndex + 1);
            yield return new WaitForSeconds(0.1f);
        }

        AddExpandListener(btn, orignalIndex);
        btn.enabled = true;
    }

    IEnumerator AddLevels(int levelCount, Button btn, int currentIndex)
    {
        yield return new WaitForEndOfFrame();
        int originalIndex = currentIndex;
        for(int i = 0; i < levelCount; i++)
        {
            MapItem levelItem = new MapItem("level_Title");
            levelItem.IsChapter = false;
            levelItem.StarsCount = 0;
            levelItem.TotalStarsCount = 3;
            currentIndex = currentIndex + 1;
            m_mapItems.Insert(currentIndex, levelItem);
            m_wrapContent.AddItem(currentIndex);
            btn.transform.parent.parent.localPosition = new Vector3(-btn.transform.parent.localPosition.x, btn.transform.parent.parent.localPosition.y, 0);
            yield return new WaitForSeconds(0.1f);
        }

        AddCompressListener(btn, originalIndex);
        btn.enabled = true;
    }
}


public enum MapItemStatus
{
    Collipsed,
    Expanded
}

public class MapItem
{
    public string Name;
    public string Description;
    public bool IsChapter;
    public int StarsCount;
    public int TotalStarsCount;
    public int LevelCount;
    public MapItemStatus Status;

    public UnityAction OnClickDelegate;

    public MapItem(string name)
    {
        Name = name;
    }
}