using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class LanguagePack
{
	private List<string> stringKeys = new List<string>();
	public List<string> Keys { get { return stringKeys; } }
	
	private List<string> strings = new List<string>();
	public List<string> Strings { get { return strings; } }
	
	public bool AddNewString(string key, string text)
	{
		if(!stringKeys.Contains(key))
		{
			this.stringKeys.Add(key);
			this.strings.Add(text);
			return true;
		}
		else
		{
			Debug.LogWarning("Key with this name already exists");
			return false;
		}
	}
	
	public void RemoveString(string key)
	{
		int index = stringKeys.IndexOf(key);
		strings.RemoveAt(index);
		stringKeys.RemoveAt(index);
	}
	
	public string GetString(string key)
	{
		if(stringKeys.Contains(key))
		{
			return strings[stringKeys.IndexOf(key)];
		}
		else
		{
			//Debug.LogWarning("The Key: \""+key+"\" was not found...");
			return "BAD KEY";
		}
	}
}

public static class LanguageManager
{
	public static bool PackLoaded = false;
	public static Language CurrentLanguage = Language.English;
	private static LanguagePack loadedLanguagePack = null;
	public static LanguagePack CurrentLanguagePack { get { return loadedLanguagePack; } }
	public static string LanguagePackDirectory = "/Resources/Languages/";
	
	static string buffer = "";
	//static XmlSerializer serializer;
	
	public static string GetText(string key)
	{
		if(PackLoaded)
		{
			buffer = loadedLanguagePack.GetString(key);
			buffer = buffer.Replace("#n", System.Environment.NewLine);
			
			return buffer;
		}
		else
		{
			return "INIT FAIL";
		}
	}
	
	public static bool AddText(string key, string text)
	{
		return CurrentLanguagePack.AddNewString(key, text);
	}
	
	public static void DeleteText(string key)
	{
		loadedLanguagePack.RemoveString(key);
	}
	
	//public static StreamWriter writer;
	//public static StringReader reader;
	//public static TextAsset xmlFile;
	
	public static void SaveLanguageFile(Language language)
	{
		//Convert Enum into a String (Not recommended method but tough!)
		string saveName = System.Enum.GetName(typeof(Language), language);
		string filePath = Application.dataPath + LanguagePackDirectory + saveName + ".txt";
		StreamWriter f = new StreamWriter(filePath);
		
		if(CurrentLanguagePack==null)
			loadedLanguagePack = new LanguagePack();
		
		for(int i = -1; i  < CurrentLanguagePack.Keys.Count; i++)
		{
			if(i == -1)
				f.WriteLine(LanguagePackDirectory);
			else			
				f.WriteLine(CurrentLanguagePack.Keys[i] + " = " + CurrentLanguagePack.Strings[i]);
			
			f.WriteLine("\n");
		}
		f.Close();
		
		PackLoaded = true;
	}
	
	/*
	public static void ConvertToNewFormat(Language language)
	{
		Debug.Log("FINISHED CONVERTING");
		LanguagePack pk = LoadToPack(language);
		
		string loadName = System.Enum.GetName(typeof(Language), language);
		string filePath = Application.dataPath + "/Resources/" + LanguagePackDirectory + loadName + "_NEW.txt";
		
		StreamWriter f = new StreamWriter(filePath);
		for(int i = 0; i  < pk.Keys.Count; i++)
		{
			f.WriteLine(pk.Keys[i] + " = " + pk.Strings[i]);
			f.WriteLine("\n");
		}
		f.Close();
	}
	*/
	
	public static void LoadLanguageFile(Language language)
	{
		loadedLanguagePack = LoadToPack(language);
	}
	
	public static void CreateTemplateFile(Language language)
	{		
		LoadLanguageFile(language);
	}
	
	public static LanguagePack LoadToPack(Language language)
	{
		CurrentLanguage = language;
		
		try
		{
			string loadName = System.Enum.GetName(typeof(Language), language);
			string filePath = LanguagePackDirectory + loadName;
			LanguagePack pk = new LanguagePack();
			
			// Read in the STRING contents of xmlFile (TextAsset)	
			TextAsset xmlFile;
			string t;
			
			if(LanguagePackDirectory.StartsWith("/Resources"))
			{	
				filePath = filePath.Substring(11);
				xmlFile = (TextAsset)Resources.Load(filePath, typeof(TextAsset));
				t = xmlFile.text;
			}
			else
			{	
				StreamReader sr = new StreamReader(Application.dataPath + filePath + ".txt");
				t = sr.ReadToEnd();
				sr.Close();
			}
			
			using(StringReader f = new StringReader(t))
			{
				string line;
				while((line = f.ReadLine()) != null)
				{
					try
					{
						pk.AddNewString((string)(line.Split(new string[] { " = " }, System.StringSplitOptions.None)[0]), line.Split(new string[] { " = " }, System.StringSplitOptions.None)[1]);
						//Debug.Log(line);
					} 
					catch 
					{
						if(line.Length > 0)
							LanguagePackDirectory = line;
					}
				}	
			}
			
			PackLoaded = true;
			
			return pk;
		} catch {
			Debug.Log("No File Was Found! Make sure the language file you are trying to load exists!");
			
			PackLoaded = false;
			
			return null;
		}
	}
	
	/*
	public static LanguagePack LoadToPack(Language language)
	{
		string loadName = System.Enum.GetName(typeof(Language), language);
		string filePath = LanguagePackDirectory + loadName;
		
		// Read in the STRING contents of xmlFile (TextAsset)	
		TextAsset xmlFile = (TextAsset)Resources.Load(filePath, typeof(TextAsset));
		
		LanguagePack lPack = new LanguagePack();
		XmlSerializer x = new XmlSerializer(typeof(LanguagePack));
		reader = new StringReader(xmlFile.text);
		lPack = (LanguagePack)x.Deserialize(reader);
		
		reader.Close();
	
		return lPack;
	}
	*/
	
	static LanguagePack masterPack;
	static LanguagePack slavePack;
	public static void AddKeysFromMaster(Language master, Language slave)
	{
		masterPack = LoadToPack(master);
		slavePack = LoadToPack(slave);
		
		// Compare and Add
		
		for(int i = 0; i < masterPack.Keys.Count; i++)
		{
			if(!slavePack.Keys.Contains(masterPack.Keys[i]))
			{
				slavePack.AddNewString(masterPack.Keys[i],masterPack.Strings[i]);
			}
		}
		
		loadedLanguagePack = slavePack;
	}
}
