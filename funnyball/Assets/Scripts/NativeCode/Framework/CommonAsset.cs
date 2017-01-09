using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommonAsset : MonoBehaviour {

//	private const string requestUrl = "https://ci1.3pjgames.com:4433/assets/";

	public static bool isLoadConfig = false;

	private static Dictionary<string,string> config = new Dictionary<string,string> ();

	private static Dictionary<string,WWW> wwwDic = new Dictionary<string, WWW> ();

	//读取配置文件－如果在配置文件里能找到，就读取，没有就用resources.load读取
	public static Object Load(string resPath){
		if (FindStreamingPath (resPath) == "") {
			return Resources.Load(resPath);
		}else{
			return LoadStreamingAsset(FindStreamingPath (resPath));
		}
	}

	public static string LoadConfig(string fileName){
		if (FileHelper.ExistFile(Application.persistentDataPath,System.IO.Path.GetFileName (fileName)+".txt")/*&&SwitchEnvironmentButton.isStartFromSwitchEnvironment*/) {
			fileName+=".txt";
			return FileHelper.LoadFile (Application.persistentDataPath, System.IO.Path.GetFileName (fileName));
		}
		else{
			Object obj=Load (fileName);
			if(obj==null)
				return null;
			else
				return (obj as TextAsset).text;
		}
//			return (Load (fileName) as TextAsset).text;
//		return null;
	}

	private static Object LoadStreamingAsset(string streamingPath){

		Debug.Log (streamingPath);
		Debug.Log (HotUpdateProControl.serverPath+" "+HotUpdateProControl.resVersion);
//		if (!wwwDic.ContainsKey(streamingPath)) {
		WWW www=WWW.LoadFromCacheOrDownload(HotUpdateProControl.serverPath+streamingPath,HotUpdateProControl.resVersion);
		Object  o=www.assetBundle.mainAsset;
		www.assetBundle.Unload (false);
//			wwwDic.Add(streamingPath,www);
		Debug.Log ("LoadOK---"+streamingPath);
//		}
//		return wwwDic[streamingPath].assetBundle.mainAsset;
//		return www.assetBundle.mainAsset;
		return o;
	}

	private static string FindStreamingPath(string resPath){
		if (!isLoadConfig) {
			CreateConfigDic();
			isLoadConfig=true;
		}

		if (!config.ContainsKey(resPath))
			return "";
		return config [resPath];
	}

	private static void CreateConfigDic(){
		config.Clear ();
		string resConfig=FileHelper.LoadFile (Application.persistentDataPath, HotUpdateProControl.resConfigPath);
		if (!string.IsNullOrEmpty (resConfig)) {
			List<AssetConfig> list = JsonHelper.DeSerialize<List<AssetConfig>> (resConfig);
			for (int i=0; i<list.Count; i++) {
				Debug.Log(list[i].StreamingPath);
				config.Add(list[i].ResPath,list[i].StreamingPath);
			}
		}

	}

}

public class AssetConfig{
	public string ResPath{ get;set;}
	public string StreamingPath{get;set;}
	
}




