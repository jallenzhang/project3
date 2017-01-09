using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TestMain : MonoBehaviour {

	private List<Item> listItem;
	private UIWarpContent warpContent;


	void Start () {
		//测试数据
		listItem = new List<Item> ();
		for (int i = 0; i < 10; i++) {
			listItem.Add (new Item("测试:"+Random.Range(1,1000)));
		}

		//scrollView 相关所需注意接口
		warpContent = gameObject.transform.GetComponentInChildren<UIWarpContent> ();
		warpContent.onInitializeItem = onInitializeItem;
		//注意：目标init方法必须在warpContent.onInitializeItem之后
		warpContent.Init (listItem.Count);
	}

	private void onInitializeItem(GameObject go,int dataIndex){
//		Debug.Log ("go = "+go.name+"_dataIndex = "+dataIndex);
        return;
		Text text = go.transform.FindChild ("Text").GetComponent<Text>();
		text.text = "i:" + dataIndex+"_N:"+listItem[dataIndex].Name();

		//add按钮监听【添加功能】
		Button addbutton = go.transform.FindChild ("Add").GetComponent<Button> ();
		addbutton.onClick.RemoveAllListeners ();
		addbutton.onClick.AddListener (delegate() {
			listItem.Insert(dataIndex+1,new Item("Insert"+Random.Range(1,1000)));
			warpContent.AddItem(dataIndex+1);
		});

		//sub按钮监听【删除功能】
		Button subButton = go.transform.FindChild ("Sub").GetComponent<Button> ();
		subButton.onClick.RemoveAllListeners ();
		subButton.onClick.AddListener (delegate() {
			listItem.RemoveAt(dataIndex);
			warpContent.DelItem(dataIndex);
		});

	}



	//测试数据结构
	public class Item{
		private string name;
		public Item(string name){
			this.name = name;
		}
		public string Name(){
			return name;
		}
		public void destroy(){
			name = null;
		}

	}
}
