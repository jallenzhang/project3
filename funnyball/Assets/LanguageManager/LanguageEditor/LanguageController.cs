using UnityEngine;
using System.Collections;

public class LanguageController : MonoBehaviour 
{	
	public Language currentLanguage = Language.English;
	
	// Use this for initialization
	void Start () {
		LanguageManager.LoadLanguageFile(currentLanguage);
	}
}
