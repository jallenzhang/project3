using UnityEngine;
using System.Collections;
using System.IO;

public class FileHelper : MonoBehaviour {

	/**
   * path：文件创建目录
   * name：文件的名称
   *  info：写入的内容
   */
	public static void CreateFile(string path,string name,string info)   
	{   
		//文件流信息
		StreamWriter sw;   
		FileInfo t = new FileInfo(path+"//"+ name);   
		if(!t.Exists)   
		{   
			//如果此文件不存在则创建
			sw = t.CreateText();
		}
		else
		{
			//如果此文件存在则打开
			sw = t.AppendText();
		}
		//以行的形式写入信息 
		sw.WriteLine(info);
		//关闭流
		sw.Close();
		//销毁流
		sw.Dispose();   
	}  
	
	
	/**
   * path：读取文件的路径
   * name：读取文件的名称
   */
	public static string LoadFile(string path,string name)   
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			string url = System.IO.Path.Combine(path, name);
			WWW www = new WWW(url);
			while (!www.isDone) ;
//			Debug.Log(string.Format("url:{0} content:{1}", url, www.text));
			return www.text;
		}

		//使用流的形式读取
		StreamReader sr =null;
		try{
			sr = File.OpenText(path+"//"+ name);  
		}catch
		{
			//路径与名称未找到文件则直接返回空
			return null;
		}
		string content = "";
		string line;
		//ArrayList arrlist = new ArrayList();
		while ((line = sr.ReadLine()) != null)
		{
			content+=line;
			//一行一行的读取
			//将每一行的内容存入数组链表容器中
//			arrlist.Add(line);
		}
		//关闭流
		sr.Close(); 
		//销毁流
		sr.Dispose();
		//将数组链表容器返回
		return content;
	}   
	
	/**
   * path：删除文件的路径
   * name：删除文件的名称
   */
	
	public static void DeleteFile(string path,string name)
	{
		File.Delete(path+"//"+ name);
		
	}

	public static bool ExistFile(string path,string name){
		return File.Exists(path+"//"+ name);
	}
}
