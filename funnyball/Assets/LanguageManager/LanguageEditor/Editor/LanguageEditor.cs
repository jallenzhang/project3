using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class LanguageEditor : EditorWindow 
{
	static LanguageEditor editor;
	static Rect window;
	static string warningMessage = "";
	
	// Add menu named "My Window" to the Window menu
	[MenuItem ("Window/Language Editor")]
	static void Init () 
	{
		// Get existing open window or if none, make a new one:
		editor = (LanguageEditor)EditorWindow.GetWindow (typeof (LanguageEditor), true, "Language Editor");
	}
	
	string newKey = "newKey";
	string newString = "Enter a new string!";
	string newLang = "";
	Language currentLanguage = Language.English;
	Language masterLanguageFile = Language.English;
	Vector2 scrollPosition = Vector2.zero;
    FileStream fs;
	void OnGUI () 
	{
		if(!editor)
			editor = (LanguageEditor)EditorWindow.GetWindow (typeof (LanguageEditor));
		
		if(!LanguageManager.PackLoaded || !Directory.Exists(Application.dataPath + LanguageManager.LanguagePackDirectory) || !File.Exists(Application.dataPath + LanguageManager.LanguagePackDirectory + System.Enum.GetName(typeof(Language), currentLanguage) + ".txt"))
		{
            try
            {
                if (!Directory.Exists(Application.dataPath + LanguageManager.LanguagePackDirectory))
                    Directory.CreateDirectory(Application.dataPath + LanguageManager.LanguagePackDirectory);

                if (!File.Exists(Application.dataPath + LanguageManager.LanguagePackDirectory + System.Enum.GetName(typeof(Language), currentLanguage) + ".txt"))
                {
                    fs = File.Create(Application.dataPath + LanguageManager.LanguagePackDirectory + System.Enum.GetName(typeof(Language), currentLanguage) + ".txt");
                    fs.Close();
                }

                UnityEditor.AssetDatabase.Refresh();

                LanguageManager.LoadLanguageFile(currentLanguage);
            }
            catch
            {
                Debug.Log("Failed to create/load file");
                LanguageManager.PackLoaded = false;
            }
		}
		
		if(currentLanguage != LanguageManager.CurrentLanguage && !Application.isPlaying)
		{
			//Debug.Log("Switching Language Automatically");
			LanguageManager.LoadLanguageFile(currentLanguage);
		}
			
		window = editor.position;
		
		GUI.Label(new Rect(10,10,window.width-20,20),"Key:   #n = New Line");
		
		if(warningMessage.Length > 0)
		{
			GUI.color = Color.red;
			GUI.Box(new Rect(window.width - 320, 5, 310, 30),"");
			
			GUI.color = Color.yellow;
			GUI.Label(new Rect(window.width - 300, 10, 300, 20),warningMessage);
			GUI.color = Color.white;
		}
		
		GUI.Box(new Rect(10,40, window.width-20,37),"");
		newKey = EditorGUI.TextField(new Rect(20, 50, 200, 17),"Reference Key:", newKey);
		newString = EditorGUI.TextField(new Rect(200, 50, window.width - 320, 17),"  ------> Text:", newString);
		if(GUI.Button(new Rect(window.width - 110, 50, 90, 17),"Add")) { if(!LanguageManager.AddText(newKey, newString)) { warningMessage = "ERROR: KEY ALREADY EXISTS!"; } else { warningMessage = ""; } }
		
		GUI.Box(new Rect(10, 80, window.width - 20, window.height - 140),"");
		
		if(LanguageManager.CurrentLanguagePack != null)
		{
			scrollPosition = GUI.BeginScrollView(new Rect(0, 80, window.width - 10, window.height - 140), scrollPosition, new Rect(0,0,window.width - 50, 20 + (25*LanguageManager.CurrentLanguagePack.Keys.Count)));
				for(int i = 0; i < LanguageManager.CurrentLanguagePack.Keys.Count; i++)
				{
					GUI.Box(new Rect(20, 10 + (i*25), window.width - 50 ,15), "");
					GUI.Label(new Rect(25, 10 + (i*25), 110 ,17), LanguageManager.CurrentLanguagePack.Keys[i]);
					LanguageManager.CurrentLanguagePack.Strings[i] = EditorGUI.TextField(new Rect(140, 10 + (i*25), window.width - 200 ,17),LanguageManager.CurrentLanguagePack.Strings[i]);
					if(GUI.Button(new Rect(window.width - 50, 10 + (i*25), 20 ,17),"X")) { LanguageManager.DeleteText(LanguageManager.CurrentLanguagePack.Keys[i]); return; }
				}
		GUI.EndScrollView();
		}
		
		GUI.Box(new Rect(10, window.height - 55, window.width - 20, 50),"");
		
		currentLanguage = (Language)EditorGUI.EnumPopup(new Rect(20, window.height - 45, 200, 17),"Language File: ", currentLanguage);
		
		if(GUI.Button(new Rect(20, window.height - 25, 90, 17), "Add Language"))
		{
			string p = Application.dataPath + "/LanguageEditor/Language.cs";
			string[] languageFiles = Directory.GetFiles(Application.dataPath + LanguageManager.LanguagePackDirectory);
			
			if(newLang=="" || newLang.Contains(" "))
			{
				warningMessage = "WARNING: Language CANNOT Contain Spaces!";
				return;
			}
			
			for(int i = 0; i < languageFiles.Length; i++)
			{
				languageFiles[i] = languageFiles[i].Substring((Application.dataPath + LanguageManager.LanguagePackDirectory).Length);
				languageFiles[i] = languageFiles[i].TrimEnd((".txt").ToCharArray());
				
				if(newLang == languageFiles[i])
				{
					warningMessage = "WARNING: Language Exists!";
					return;
				}
			}
			
			File.Delete(p);
			
			StreamWriter sr = File.CreateText(p);
			
			sr.WriteLine("public enum Language {");
			
			for(int i = 0; i < languageFiles.Length; i++)
			{
				if(i==0 && languageFiles.Length > 1)
					sr.WriteLine(languageFiles[i]);
				else if(i==0 && languageFiles.Length == 1)
					sr.WriteLine(languageFiles[i]);
				else
					sr.WriteLine("," + languageFiles[i]);
			}
			
			sr.WriteLine(","+newLang);
			
			sr.WriteLine("}");
			
			sr.Close();
			
			AssetDatabase.Refresh();
		}
		
		newLang = EditorGUI.TextField(new Rect(20, window.height - 25, 200, 17),"", newLang);
		
		if(GUI.Button(new Rect(250, window.height - 35, 90, 17), "Set Directory")) 
		{ 
			LanguageManager.LanguagePackDirectory = EditorUtility.SaveFolderPanel("Save To Directory", Application.dataPath, ""); 
			LanguageManager.LanguagePackDirectory = LanguageManager.LanguagePackDirectory.Substring(Application.dataPath.Length) + "/";
		}
		EditorGUI.LabelField(new Rect(250, window.height - 35, 400, 17), "", LanguageManager.LanguagePackDirectory);
		
		GUI.Box(new Rect(window.width - 610, window.height - 45, 330, 33),"");
		masterLanguageFile = (Language)EditorGUI.EnumPopup(new Rect(window.width - 500,  window.height - 35, 200, 17),"            Master: ", masterLanguageFile);
		if(GUI.Button(new Rect(window.width - 600, window.height - 35, 100, 17),"Balance Keys") ) { LanguageManager.AddKeysFromMaster(masterLanguageFile, currentLanguage); }
		
		//if(GUI.Button(new Rect(window.width - 240, window.height - 55, 100, 17),"Convert File")) { LanguageManager.ConvertToNewFormat(currentLanguage); }
		if(GUI.Button(new Rect(window.width - 240, window.height - 35, 100, 17),"Save File") && EditorUtility.DisplayDialog("Saving "+ System.Enum.GetName(typeof(Language), currentLanguage).ToUpper() + "!","You are about to save over the file which stores the " + System.Enum.GetName(typeof(Language), currentLanguage) + " language. Are you sure you want to do this?" ,"Save","Cancel")) { LanguageManager.SaveLanguageFile(currentLanguage); AssetDatabase.Refresh();}
		if(GUI.Button(new Rect(window.width - 120, window.height - 35, 100, 17),"Load File") && EditorUtility.DisplayDialog("Loading "+ System.Enum.GetName(typeof(Language), currentLanguage).ToUpper() + "!","All settings will NOT be saved for the " + System.Enum.GetName(typeof(Language), currentLanguage) + " language. Are you sure you want to do this?" ,"Load","Cancel")) { LanguageManager.LoadLanguageFile(currentLanguage); }
	}
}
