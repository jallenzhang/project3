using System.Collections;
using System;
using System.Collections.Generic;
using Mogo.Util;
using System.Reflection;

public class GameDataControler : DataLoader {
	private static GameDataControler m_instance;// = new GameDataControler();

	public static GameDataControler Instance
	{
		get
		{
			if(m_instance==null)
				m_instance=new GameDataControler();
			return m_instance;
		}
	}

	public object FormatData(string fileName, Type dicType, Type type)
	{
//		if (SystemSwitch.get_UseHmf())
//		{
//			return this.FormatHmfData(base.m_resourcePath + fileName + base.m_fileExtention, dicType, type);
//		}
		//return this.FormatXMLData(base.m_resourcePath + fileName + base.m_fileExtention, dicType, type);
		return this.FormatXMLData(base.m_resourcePath + fileName , dicType, type);
	}

	private object FormatXMLData(string fileName, Type dicType, Type type)
	{
		object obj2 = null;
		try
		{
			Dictionary<int, Dictionary<string, string>> dictionary=new Dictionary<int, Dictionary<string, string>>();
			obj2 = dicType.GetConstructor(Type.EmptyTypes).Invoke(null);
			if (!XMLParser.LoadIntMap(fileName, base.m_isUseOutterConfig, out dictionary))
			{
				return obj2;
			}
			PropertyInfo[] properties = type.GetProperties();
			foreach (KeyValuePair<int, Dictionary<string, string>> pair in dictionary)
			{
				object obj3 = type.GetConstructor(Type.EmptyTypes).Invoke(null);
				foreach (PropertyInfo info in properties)
				{
					if (info.Name == "id")
					{
						info.SetValue(obj3, pair.Key, null);
					}
					else if (pair.Value.ContainsKey(info.Name))
					{
						object obj4 = Utils.GetValue(pair.Value[info.Name], info.PropertyType);
						info.SetValue(obj3, obj4, null);
					}
				}
				dicType.GetMethod("Add").Invoke(obj2, new object[] { pair.Key, obj3 });
			}
		}
		catch (Exception exception)
		{
			LoggerHelper.Error("FormatData Error: " + fileName + "  " + exception.Message, true);
		}
		return obj2;
	}
}
