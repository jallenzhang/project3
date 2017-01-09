using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

public abstract class GameData<T> : GameData {

	private static Dictionary<int, T> m_dataMap;

	public static Dictionary<int, T> dataMap
	{
		get
		{
			if (m_dataMap == null)
			{
				m_dataMap = GameData.GetDataMap<T>();
			}
			return m_dataMap;
		}
		set
		{
			m_dataMap = value;
		}
	}
}

public abstract class GameData
{

	protected GameData()
	{
	}
	protected static Dictionary<int, T> GetDataMap<T>()
	{
		Dictionary<int, T> dictionary;
		Type type = typeof(T);
		FieldInfo field = type.GetField("fileName");
		if (field != null)
		{
			string fileName = field.GetValue(null) as string;
			dictionary = GameDataControler.Instance.FormatData(fileName, typeof(Dictionary<int, T>), type) as Dictionary<int, T>;
		}
		else
		{
			dictionary = new Dictionary<int, T>();
		}
		return dictionary;
	}

	public int id { get; protected set; }
}

