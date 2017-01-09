using UnityEngine;
using System.Collections;
using Mogo.Util;

public abstract class DataLoader
{
	protected readonly string m_resourcePath;
	protected readonly string m_fileExtention;
	protected readonly bool m_isUseOutterConfig = false;//SystemConfig.get_IsUseOutterConfig();

	protected DataLoader()
	{
		this.m_resourcePath = "data/";
		this.m_fileExtention = ".xml";
	}

}
