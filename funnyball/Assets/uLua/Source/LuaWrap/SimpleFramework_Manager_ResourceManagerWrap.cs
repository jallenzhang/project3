using System;
using UnityEngine;
using LuaInterface;
using Object = UnityEngine.Object;

public class SimpleFramework_Manager_ResourceManagerWrap
{
	public static void Register(IntPtr L)
	{
		LuaMethod[] regs = new LuaMethod[]
		{
			new LuaMethod("Initialize", Initialize),
			new LuaMethod("LoadAsset", LoadAsset),
			new LuaMethod("New", _CreateSimpleFramework_Manager_ResourceManager),
			new LuaMethod("GetClassType", GetClassType),
			new LuaMethod("__eq", Lua_Eq),
		};

		LuaField[] fields = new LuaField[]
		{
		};

		LuaScriptMgr.RegisterLib(L, "SimpleFramework.Manager.ResourceManager", typeof(SimpleFramework.Manager.ResourceManager), regs, fields, typeof(View));
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateSimpleFramework_Manager_ResourceManager(IntPtr L)
	{
		LuaDLL.luaL_error(L, "SimpleFramework.Manager.ResourceManager class does not have a constructor function");
		return 0;
	}

	static Type classType = typeof(SimpleFramework.Manager.ResourceManager);

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, classType);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Initialize(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		SimpleFramework.Manager.ResourceManager obj = (SimpleFramework.Manager.ResourceManager)LuaScriptMgr.GetUnityObjectSelf(L, 1, "SimpleFramework.Manager.ResourceManager");
		obj.Initialize();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadAsset(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 3)
		{
			SimpleFramework.Manager.ResourceManager obj = (SimpleFramework.Manager.ResourceManager)LuaScriptMgr.GetUnityObjectSelf(L, 1, "SimpleFramework.Manager.ResourceManager");
			string arg0 = LuaScriptMgr.GetLuaString(L, 2);
			string arg1 = LuaScriptMgr.GetLuaString(L, 3);
			GameObject o = obj.LoadAsset(arg0,arg1);
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		else if (count == 4)
		{
			SimpleFramework.Manager.ResourceManager obj = (SimpleFramework.Manager.ResourceManager)LuaScriptMgr.GetUnityObjectSelf(L, 1, "SimpleFramework.Manager.ResourceManager");
			string arg0 = LuaScriptMgr.GetLuaString(L, 2);
			string arg1 = LuaScriptMgr.GetLuaString(L, 3);
			LuaFunction arg2 = LuaScriptMgr.GetLuaFunction(L, 4);
			obj.LoadAsset(arg0,arg1,arg2);
			return 0;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: SimpleFramework.Manager.ResourceManager.LoadAsset");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Lua_Eq(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Object arg0 = LuaScriptMgr.GetLuaObject(L, 1) as Object;
		Object arg1 = LuaScriptMgr.GetLuaObject(L, 2) as Object;
		bool o = arg0 == arg1;
		LuaScriptMgr.Push(L, o);
		return 1;
	}
}

