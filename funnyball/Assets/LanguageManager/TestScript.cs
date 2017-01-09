using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour 
{
	public Language defaultLanguage = Language.English;
	
	private int prevSel = 0;
	private int selection = 0;
	private string[] selections;
	
	void Start()
	{
		// Initialize the language manager with English language
		LanguageManager.LoadLanguageFile(defaultLanguage);
		selections = new string[] { LanguageManager.GetText("english"), LanguageManager.GetText("spanish"), LanguageManager.GetText("russian"), LanguageManager.GetText("french"), LanguageManager.GetText("italian"), LanguageManager.GetText("chinese") };
	}
	
	void OnGUI()
	{
		// To swap text using the Language Manager, you must use "LanguageManager.GetText(string key)" method as demonstrated in the following lines
		GUI.Label(new Rect(10,25,300,20), "Sentence 1: " + LanguageManager.GetText("HelloWorld"));
		GUI.Label(new Rect(10,50,300,20), "Sentence 2: " + LanguageManager.GetText("HelloWorld2"));
		
		selection = GUI.SelectionGrid(new Rect(10,100,Screen.width - 20,50),selection, selections, selections.Length);
		
		if(prevSel != selection)
		{
			prevSel = selection;
			
			switch(selection)
			{
				case 0:
					LanguageManager.LoadLanguageFile(Language.English);
					break;
				case 1:
					LanguageManager.LoadLanguageFile(Language.Spanish);
					break;
				case 2:
					LanguageManager.LoadLanguageFile(Language.Russian);
					break;
				case 3:
					LanguageManager.LoadLanguageFile(Language.French);
					break;
				case 4:
					LanguageManager.LoadLanguageFile(Language.Italian);
					break;
				case 5:
					LanguageManager.LoadLanguageFile(Language.Chinese);
					break;
			}
			
			// In this instance we want the buttons to change there text to display in the selected language, so again we use the "LanguageManager.GetText(string key)" method
			selections = new string[] { LanguageManager.GetText("english"), LanguageManager.GetText("spanish"), LanguageManager.GetText("russian"), LanguageManager.GetText("french"), LanguageManager.GetText("italian"), LanguageManager.GetText("chinese") };
		}
	}
}
