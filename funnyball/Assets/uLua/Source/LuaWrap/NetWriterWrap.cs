using System;
using System.Collections.Generic;
using LuaInterface;

public class NetWriterWrap
{
    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[]
		{
            new LuaMethod("getMd5String", getMd5String),
            new LuaMethod("GetUrl", GetUrl),
            new LuaMethod("IsSocket", IsSocket),
            new LuaMethod("PostData", PostData),
            new LuaMethod("resetData", resetData),
            new LuaMethod("SetBodyData", SetBodyData),
            new LuaMethod("SetHeadBuffer", SetHeadBuffer),
            new LuaMethod("SetMd5Key", SetMd5Key),
            new LuaMethod("setSessionID", setSessionID),
            new LuaMethod("setStime", setStime),
            new LuaMethod("SetUrl", SetUrl),
            new LuaMethod("setUserID", setUserID),
            new LuaMethod("url_encode", url_encode),
            new LuaMethod("writeBuf", writeBuf),
            new LuaMethod("writeFloat", writeFloat),
            new LuaMethod("writeInt32", writeInt32),
            new LuaMethod("writeInt64", writeInt64),
            new LuaMethod("writeString", writeString),
            new LuaMethod("writeWord", writeWord),
			new LuaMethod("New", _CreateNetWriter),
			new LuaMethod("GetClassType", GetClassType),
		};

        LuaField[] fields = new LuaField[]
		{
			new LuaField("Instance", get_Instance, null),
            new LuaField("IsGet", get_IsGet, null),
            new LuaField("MsgId", get_MsgId, null),
            new LuaField("ResponseContentType", get_ResponseContentType, null),
            new LuaField("SessionID", get_SessionID, null),
			new LuaField("St", get_St, null),
            new LuaField("UserID", get_UserID, null),
		};

        LuaScriptMgr.RegisterLib(L, "NetWriter", typeof(NetWriter), regs, fields, typeof(object));
    }
    #region Methods
    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int _CreateNetWriter(IntPtr L)
    {
        LuaDLL.luaL_error(L, "NetWriter class does not have a constructor function");
        return 0;
    }

    static Type classType = typeof(NetWriter);
    
    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int getMd5String(IntPtr L)
    {
        int count = LuaDLL.lua_gettop(L);

        if (count == 1 && LuaScriptMgr.CheckTypes(L, 1, typeof(byte[])))
        {
            byte[] objs0 = LuaScriptMgr.GetArrayNumber<byte>(L, 1);
            string str = NetWriter.getMd5String(objs0);
            LuaScriptMgr.Push(L, str);
            return 1;
        }
        else if (count == 1 && LuaScriptMgr.CheckTypes(L, 1, typeof(string)))
        {
            string arg0 = LuaScriptMgr.GetString(L, 1);
            string str = NetWriter.getMd5String(arg0);
            LuaScriptMgr.Push(L, str);
            return 1;
        }
        return 0;
    }


    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }


    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int GetUrl(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        string o = NetWriter.GetUrl();
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int IsSocket(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        bool o = NetWriter.IsSocket();
        LuaScriptMgr.Push(L, o);
        return 1;
    }
    
    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int PostData(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        NetWriter writer = (NetWriter)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetWriter");
        byte[] objs = writer.PostData();
        LuaScriptMgr.PushArray(L, objs);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int resetData(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        NetWriter.resetData();
        return 0;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int SetBodyData(IntPtr L)
    {
        int count = LuaDLL.lua_gettop(L);
        if (count == 1 && LuaScriptMgr.CheckTypes(L, 1, typeof(byte[])))
        {
            NetWriter writer = (NetWriter)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetWriter");
            byte[] objs0 = LuaScriptMgr.GetArrayNumber<byte>(L, 1);
            writer.SetBodyData(objs0);
        }

        return 0;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int SetHeadBuffer(IntPtr L)
    {
        int count = LuaDLL.lua_gettop(L);
        if (count == 1 && LuaScriptMgr.CheckTypes(L, 1, typeof(byte[])))
        {
            NetWriter writer = (NetWriter)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetWriter");
            byte[] objs0 = LuaScriptMgr.GetArrayNumber<byte>(L, 1);
            writer.SetHeadBuffer(objs0);
        }

        return 0;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int SetMd5Key(IntPtr L)
    {
        int count = LuaDLL.lua_gettop(L);
        if (count == 1 && LuaScriptMgr.CheckTypes(L, 1, typeof(string)))
        {
            string o = LuaScriptMgr.GetString(L, 1);
            NetWriter.SetMd5Key(o);
        }

        return 0;
    }

    
    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int setSessionID(IntPtr L)
    {
        int count = LuaDLL.lua_gettop(L);
        if (count == 1 && LuaScriptMgr.CheckTypes(L, 1, typeof(string)))
        {
            string o = LuaScriptMgr.GetString(L, 1);
            NetWriter.setSessionID(o);
        }

        return 0;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int setStime(IntPtr L)
    {
        int count = LuaDLL.lua_gettop(L);
        if (count == 1 && LuaScriptMgr.CheckTypes(L, 1, typeof(string)))
        {
            string o = LuaScriptMgr.GetString(L, 1);
            NetWriter.setStime(o);
        }

        return 0;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int SetUrl(IntPtr L)
    {
        int count = LuaDLL.lua_gettop(L);
        if (count == 1 && LuaScriptMgr.CheckTypes(L, 1, typeof(string)))
        {
            string o = LuaScriptMgr.GetString(L, 1);
            NetWriter.SetUrl(o);
        }
        else if (count == 3)
        {
            string str = LuaScriptMgr.GetString(L, 1);
            ResponseContentType responseType = (ResponseContentType)LuaScriptMgr.GetNetObject(L, 2, typeof(ResponseContentType));
            bool arg3 = LuaScriptMgr.GetBoolean(L, 3);
            NetWriter.SetUrl(str, responseType, arg3);
        }

        return 0;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int setUserID(IntPtr L)
    {
        int count = LuaDLL.lua_gettop(L);
        if (count == 1 && LuaScriptMgr.CheckTypes(L, 1, typeof(ulong)))
        {
            ulong o = (ulong)LuaScriptMgr.GetNetObject(L, 1, typeof(ulong));
            NetWriter.setUserID(o);
        }
        return 0;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int url_encode(IntPtr L)
    {
        int count = LuaDLL.lua_gettop(L);
        if (count == 1 && LuaScriptMgr.CheckTypes(L, 1, typeof(string)))
        {
            string str = LuaScriptMgr.GetString(L, 1);
            NetWriter writer = (NetWriter)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetWriter");
            string o = writer.url_encode(str);
            LuaScriptMgr.Push(L, 0);
            return 1;
        }

        return 0;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int writeBuf(IntPtr L)
    {
        int count = LuaDLL.lua_gettop(L);
        
        if (count == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(byte[]), typeof(int)))
        {
            NetWriter writer = (NetWriter)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetWriter");
            string arg0 = LuaScriptMgr.GetString(L, 1);
            byte[] objs1 = LuaScriptMgr.GetArrayNumber<byte>(L, 2);
            int arg2 = (int)LuaDLL.lua_tonumber(L, 3);
            writer.writeBuf(arg0, objs1, arg2);
        }

        return 0;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int writeFloat(IntPtr L)
    {
        int count = LuaDLL.lua_gettop(L);

        if (count == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(float)))
        {
            NetWriter writer = (NetWriter)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetWriter");
            string arg0 = LuaScriptMgr.GetString(L, 1);
            float arg1 = (float)LuaDLL.lua_tonumber(L, 2);
            writer.writeFloat(arg0, arg1);
        }
        return 0;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int writeInt32(IntPtr L)
    {
        int count = LuaDLL.lua_gettop(L);

        if (count == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(int)))
        {
            NetWriter writer = (NetWriter)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetWriter");
            string arg0 = LuaScriptMgr.GetString(L, 1);
            int arg1 = (int)LuaDLL.lua_tonumber(L, 2);
            writer.writeInt32(arg0, arg1);
        }
        return 0;
    }
    
    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int writeInt64(IntPtr L)
    {
        int count = LuaDLL.lua_gettop(L);

        if (count == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(UInt64)))
        {
            NetWriter writer = (NetWriter)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetWriter");
            string arg0 = LuaScriptMgr.GetString(L, 1);
            UInt64 arg1 = (UInt64)LuaDLL.lua_tonumber(L, 2);
            writer.writeInt64(arg0, arg1);
        }
        return 0;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int writeString(IntPtr L)
    {
        int count = LuaDLL.lua_gettop(L);

        if (count == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string)))
        {
            NetWriter writer = (NetWriter)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetWriter");
            string arg0 = LuaScriptMgr.GetString(L, 1);
            string arg1 = LuaScriptMgr.GetString(L, 2);
            writer.writeString(arg0, arg1);
        }
        return 0;
    }
    
    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int writeWord(IntPtr L)
    {
        int count = LuaDLL.lua_gettop(L);

        if (count == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(UInt16)))
        {
            NetWriter writer = (NetWriter)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetWriter");
            string arg0 = LuaScriptMgr.GetString(L, 1);
            UInt16 arg1 = (UInt16)LuaDLL.lua_tonumber(L, 2);
            writer.writeWord(arg0, arg1);
        }
        return 0;
    }
    #endregion

    #region Field
    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int get_Instance(IntPtr L)
    {
        LuaScriptMgr.PushObject(L, NetWriter.Instance);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int get_IsGet(IntPtr L)
    {
        LuaScriptMgr.PushObject(L, NetWriter.IsGet);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int get_MsgId(IntPtr L)
    {
        LuaScriptMgr.PushObject(L, NetWriter.MsgId);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int get_ResponseContentType(IntPtr L)
    {
        LuaScriptMgr.PushObject(L, NetWriter.ResponseContentType);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int get_SessionID(IntPtr L)
    {
        LuaScriptMgr.PushObject(L, NetWriter.SessionID);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int get_St(IntPtr L)
    {
        LuaScriptMgr.PushObject(L, NetWriter.St);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int get_UserID(IntPtr L)
    {
        LuaScriptMgr.PushObject(L, NetWriter.UserID);
        return 1;
    }
    #endregion
}
