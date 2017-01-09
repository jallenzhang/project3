using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HotUpdateProControl : MonoBehaviour {

	#if UNITY_EDITOR
	public static string serverPath = "https://svn.3pjgames.com/nas/open-95c7c644daca680ca2d55674/gu/ci/";
	#else
	public static string serverPath="http://assets.3pjgames.com/gu";
	#endif

	public static readonly string resConfigPath="resConfig";

	public static readonly int resVersion=0;

	public System.Action nextTodo=null;

	//public UISlider progress;

	public void CheckoutResHotUpdate(System.Action nextTodo){

		this.nextTodo = nextTodo;
		StartCoroutine(CheckResUpdate());
	}


	IEnumerator CheckResUpdate(){
		print("start update");
		ResConfigInfoList resConfigInfoList = null;
		using (WWW updateInfo = new WWW(HotUpdateProControl.serverPath+"/resConfig.unity3d")) {
			yield return updateInfo;
//			updateInfo.
			if(string.IsNullOrEmpty (updateInfo.error)){
				print((updateInfo.assetBundle.mainAsset as TextAsset).ToString());
				resConfigInfoList=JsonHelper.DeSerialize<ResConfigInfoList>((updateInfo.assetBundle.mainAsset as TextAsset).ToString());
				updateInfo.assetBundle.Unload(true);
			}else{
//				DialogManager.Instance.PopupErrorMessageBox("", 
//				                                            ConfigHelper.LoadLocalStringByKey("IDS_COMMON_SERVER_FAILED"),
//				                                            ()=>{System.Diagnostics.Process.GetCurrentProcess().Kill();},null,string.Empty,ConfigHelper.LoadLocalStringByKey("IDS_COMMON_CONFIRM")
//				,ErrorDialogStyle.DialogWithOneButton);
			}
		}
		if (resConfigInfoList != null && resConfigInfoList.isClearCache) {
			FileHelper.DeleteFile(Application.persistentDataPath, HotUpdateProControl.resConfigPath);
			Caching.CleanCache();
			PlayerPrefs.DeleteKey("localResVersion");
		}

		int localResVersion = PlayerPrefs.GetInt ("localResVersion", 0);
		Debug.Log ("localResVersion=" + localResVersion);
		if (resConfigInfoList != null) {
			int allUpdateLength=0;
			for (int i=0; i<resConfigInfoList.resConfigInfo.Count; i++) {
				if(resConfigInfoList!=null&&resConfigInfoList.resConfigInfo[i].buildNum>localResVersion){
					List<AssetConfig> netVersionUpdateInfo=null;
					using (WWW updateInfo = new WWW(HotUpdateProControl.serverPath+resConfigInfoList.resConfigInfo[i].version+"/config.unity3d")) {
						yield return updateInfo;
						if(string.IsNullOrEmpty (updateInfo.error)){
							print((updateInfo.assetBundle.mainAsset as TextAsset).ToString());
							netVersionUpdateInfo=JsonHelper.DeSerialize<List<AssetConfig>>((updateInfo.assetBundle.mainAsset as TextAsset).ToString());
							updateInfo.assetBundle.Unload(true);
						}else{
//							DialogManager.Instance.PopupErrorMessageBox("", 
//							                                            ConfigHelper.LoadLocalStringByKey("IDS_COMMON_SERVER_FAILED"),
//							                                            ()=>{System.Diagnostics.Process.GetCurrentProcess().Kill();},null,string.Empty,ConfigHelper.LoadLocalStringByKey("IDS_COMMON_CONFIRM")
//							,ErrorDialogStyle.DialogWithOneButton);
						}
					}
					if(netVersionUpdateInfo!=null){
						allUpdateLength+=netVersionUpdateInfo.Count;
					}
				}
			}

			int proIndex=0;
            //progress.value=0;
			bool isDoneSuccess=true;
			for (int i=0; i<resConfigInfoList.resConfigInfo.Count; i++) {
				if(resConfigInfoList!=null&&resConfigInfoList.resConfigInfo[i].buildNum>localResVersion){
					if (resConfigInfoList!=null&&resConfigInfoList.resConfigInfo[i].isClearCache) {
						FileHelper.DeleteFile(Application.persistentDataPath, HotUpdateProControl.resConfigPath);
						Caching.CleanCache();
					}

					List<AssetConfig> netVersionUpdateInfo=null;
					using (WWW updateInfo = new WWW(HotUpdateProControl.serverPath+resConfigInfoList.resConfigInfo[i].version+"/config.unity3d")) {
						yield return updateInfo;
						if(string.IsNullOrEmpty (updateInfo.error)){
							print((updateInfo.assetBundle.mainAsset as TextAsset).ToString());
							netVersionUpdateInfo=JsonHelper.DeSerialize<List<AssetConfig>>((updateInfo.assetBundle.mainAsset as TextAsset).ToString());
							updateInfo.assetBundle.Unload(true);
						}else{
//							DialogManager.Instance.PopupErrorMessageBox("", 
//							                                            ConfigHelper.LoadLocalStringByKey("IDS_COMMON_SERVER_FAILED"),
//							                                            ()=>{System.Diagnostics.Process.GetCurrentProcess().Kill();},null,string.Empty,ConfigHelper.LoadLocalStringByKey("IDS_COMMON_CONFIRM")
//							,ErrorDialogStyle.DialogWithOneButton);
							break;
						}
					}
					if(netVersionUpdateInfo!=null){
						for(int j=0;j<netVersionUpdateInfo.Count;j++){
							using (WWW download = WWW.LoadFromCacheOrDownload(HotUpdateProControl.serverPath+resConfigInfoList.resConfigInfo[i].version+"/"+netVersionUpdateInfo[j].StreamingPath,resVersion)) {
								yield return download;
								if(string.IsNullOrEmpty (download.error)){
									print("upload"+netVersionUpdateInfo[j].StreamingPath);
									
//									//加载到游戏中
//									yield return Instantiate(download.assetBundle.mainAsset);
									
									download.assetBundle.Unload(false);
									proIndex++;
                                    //progress.value=proIndex*1.0f/allUpdateLength;
								}else{
//									DialogManager.Instance.PopupErrorMessageBox("", 
//									                                            ConfigHelper.LoadLocalStringByKey("IDS_COMMON_SERVER_FAILED"),
//									                                            ()=>{System.Diagnostics.Process.GetCurrentProcess().Kill();},null,string.Empty,ConfigHelper.LoadLocalStringByKey("IDS_COMMON_CONFIRM")
//									,ErrorDialogStyle.DialogWithOneButton);
									isDoneSuccess=false;
									break;
								}
							}
						}
						if(!isDoneSuccess)
							break;
						updateConfig(netVersionUpdateInfo,resConfigInfoList.resConfigInfo[i].version);
					}
					PlayerPrefs.SetInt("localResVersion",resConfigInfoList.resConfigInfo[i].buildNum);
				}
			}
			CommonAsset.isLoadConfig = false;
		}

		print("end update");
//		SceneHelper.SetInvisible (gameObject);
		nextTodo ();
	}

	void updateConfig(List<AssetConfig> update_configList,string version){
		
		string resConfig=FileHelper.LoadFile (Application.persistentDataPath, HotUpdateProControl.resConfigPath);
		if (string.IsNullOrEmpty (resConfig)) {
			for(int i=0;i<update_configList.Count;i++){
				update_configList[i].StreamingPath=version+"/"+update_configList[i].StreamingPath;
			}
			string replaceResConfig = JsonHelper.Serialize (update_configList);

			FileHelper.CreateFile(Application.persistentDataPath, HotUpdateProControl.resConfigPath, replaceResConfig);
		}else{
			List<AssetConfig> configList = JsonHelper.DeSerialize<List<AssetConfig>> (resConfig);
			List<AssetConfig> configReplaceList = JsonHelper.DeSerialize<List<AssetConfig>> (resConfig);
			for (int i=0; i<update_configList.Count; i++) {
				bool isExist=false;
				for(int j=0;j<configList.Count;j++){
					if(update_configList[i].ResPath==configList[j].ResPath){
						configReplaceList[j].StreamingPath=version+"/"+update_configList[i].StreamingPath;
						isExist=true;
						break;
					}
				}
				if(!isExist){
					AssetConfig assetConfig=new AssetConfig();
					assetConfig.ResPath=update_configList[i].ResPath;
					assetConfig.StreamingPath=version+"/"+update_configList[i].StreamingPath;
					configReplaceList.Add(assetConfig);
				}
					
			}
			string replaceResConfig = JsonHelper.Serialize (configReplaceList);
			FileHelper.DeleteFile(Application.persistentDataPath, HotUpdateProControl.resConfigPath);
			FileHelper.CreateFile(Application.persistentDataPath, HotUpdateProControl.resConfigPath, replaceResConfig);
		}
		
	}

}

public class ResConfigInfoList{
	public List<ResConfigInfo> resConfigInfo{ get; set; }
	public bool isClearCache{ get; set; }

}


public class ResConfigInfo{
	public bool isClearCache{ get; set; }
	public string version{ get; set; }
	public int buildNum{ get; set; }
	public int gameVersion{ get; set; }
}