using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameSetting : MonoBehaviour
{
    public const string ClientPasswordKey = "j6=9=1ac";
    public static Language defaultLanguage = Language.Chinese;
    //public const string ServerAddress = "42.96.149.51:9001";
    private static GameSetting setting;
    private static string[] selections;
    static GameSetting()
    {
        setting = FindObjectOfType(typeof(GameSetting)) as GameSetting;
        if (setting == null)
        {
            GameObject obj2 = new GameObject("GameSetting");
            setting = obj2.AddComponent(typeof(GameSetting)) as GameSetting;
        }
        if (setting == null) return;

        setting.Pid = SystemInfo.deviceUniqueIdentifier;
        LanguageManager.LoadLanguageFile(defaultLanguage);
        //Debug.logger.Log("SystemInfo.deviceUniqueIdentifier " + SystemInfo.deviceUniqueIdentifier);
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public static GameSetting Instance { get { return setting; } }


    public string Pid { get; set; }

    public string RoleName { get; set; }

    public bool UseNetwork { get; set; }
}
