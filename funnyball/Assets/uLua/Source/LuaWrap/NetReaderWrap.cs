using LuaInterface;
using System;
using System.Collections.Generic;

public class NetReaderWrap
{
    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[]
		{
            new LuaMethod("Decompression", Decompression),
            new LuaMethod("getByte", getByte),
            new LuaMethod("IsSocket", getDateTime),
            new LuaMethod("PostData", getDouble),
            new LuaMethod("resetData", getFloat),
            new LuaMethod("SetBodyData", getInt),
            new LuaMethod("SetHeadBuffer", getLong),
            new LuaMethod("SetMd5Key", getRecordNumber),
            new LuaMethod("setSessionID", getSByte),
            new LuaMethod("setStime", getShort),
            new LuaMethod("SetUrl", getString),
            new LuaMethod("setUserID", getUInt),
            new LuaMethod("url_encode", getULong),
            new LuaMethod("writeBuf", getUShort),
            new LuaMethod("writeFloat", pushNetStream),
            new LuaMethod("writeInt32", readBytes),
            new LuaMethod("writeInt64", readInt64),
            new LuaMethod("writeString", ReadReverseString),
            new LuaMethod("writeWord", readString),
            new LuaMethod("writeWord", recordBegin),
            new LuaMethod("writeWord", recordEnd),
            new LuaMethod("writeWord", ReverseInt),
            new LuaMethod("writeWord", SetBuffer),
			new LuaMethod("New", _CreateNetReader),
			new LuaMethod("GetClassType", GetClassType),
		};

        LuaField[] fields = new LuaField[]
		{
            new LuaField("ActionId", get_ActionId, null),
            new LuaField("Buffer", get_Buffer, null),
            new LuaField("Description", get_Description, null),
            new LuaField("RmId", get_RmId, null),
            new LuaField("StatusCode", get_StatusCode, null),
            new LuaField("StrTime", get_StrTime, null),
            new LuaField("Success", get_Success, null),
		};

        LuaScriptMgr.RegisterLib(L, "NetReader", typeof(NetReader), regs, fields, typeof(object));
    }

    #region Methods
    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int _CreateNetReader(IntPtr L)
    {
        int count = LuaDLL.lua_gettop(L);

        if (count == 1)
        {
            IHeadFormater arg0 = (IHeadFormater)LuaScriptMgr.GetNetObject(L, 1, typeof(IHeadFormater));
            NetReader obj = new NetReader(arg0);
            LuaScriptMgr.PushObject(L, obj);
            return 1;
        }
        else
        {
            LuaDLL.luaL_error(L, "invalid arguments to method: NetReader.New");
        }

        return 0;
    }

    static Type classType = typeof(NetReader);

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int Decompression(IntPtr L)
    {
        int count = LuaDLL.lua_gettop(L);
        if (count == 1 && LuaScriptMgr.CheckTypes(L, 1, typeof(byte[])))
        {
            byte[] objs0 = LuaScriptMgr.GetArrayNumber<byte>(L, 1);
            byte[] o = NetReader.Decompression(objs0);
            LuaScriptMgr.PushArray(L, o);
            return 1;
        }
        return 0;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int getByte(IntPtr L)
    {
        NetReader reader = (NetReader)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetReader");
        byte o = reader.getByte();
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int getDateTime(IntPtr L)
    {
        NetReader reader = (NetReader)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetReader");
        DateTime o = reader.getDateTime();
        LuaScriptMgr.PushObject(L, o);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int getDouble(IntPtr L)
    {
        NetReader reader = (NetReader)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetReader");
        double o = reader.getDouble();
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int getFloat(IntPtr L)
    {
        NetReader reader = (NetReader)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetReader");
        float o = reader.getFloat();
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int getInt(IntPtr L)
    {
        NetReader reader = (NetReader)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetReader");
        int o = reader.getInt();
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int getLong(IntPtr L)
    {
        NetReader reader = (NetReader)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetReader");
        Int64 o = reader.getLong();
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int getRecordNumber(IntPtr L)
    {
        NetReader reader = (NetReader)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetReader");
        byte o = reader.getRecordNumber();
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int getSByte(IntPtr L)
    {
        NetReader reader = (NetReader)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetReader");
        sbyte o = reader.getSByte();
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int getShort(IntPtr L)
    {
        NetReader reader = (NetReader)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetReader");
        short o = reader.getShort();
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int getString(IntPtr L)
    {
        int count = LuaDLL.lua_gettop(L);
        if (count == 1 && LuaScriptMgr.CheckTypes(L, 1, typeof(int)))
        {
            int objs0 = (int)LuaDLL.lua_tonumber(L, 1);
            NetReader reader = (NetReader)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetReader");
            string o = reader.getString(objs0);
            LuaScriptMgr.Push(L, o);
            return 1;
        }
        return 0;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int getUInt(IntPtr L)
    {
        NetReader reader = (NetReader)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetReader");
        uint o = reader.getUInt();
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int getULong(IntPtr L)
    {
        NetReader reader = (NetReader)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetReader");
        ulong o = reader.getULong();
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int getUShort(IntPtr L)
    {
        NetReader reader = (NetReader)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetReader");
        ushort o = reader.getUShort();
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int pushNetStream(IntPtr L)
    {
        int count = LuaDLL.lua_gettop(L);

        if (count == 3)
        {
            NetReader reader = (NetReader)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetReader");
            byte[] objs0 = LuaScriptMgr.GetArrayNumber<byte>(L, 1);
            NetworkType arg1 = (NetworkType)LuaScriptMgr.GetNetObject(L, 2, typeof(NetworkType));
            ResponseContentType arg2 = (ResponseContentType)LuaScriptMgr.GetNetObject(L, 2, typeof(ResponseContentType));
            bool o = reader.pushNetStream(objs0, arg1, arg2);
            LuaScriptMgr.Push(L, o);
            return 1;
        }

        return 0;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int readBytes(IntPtr L)
    {
        int count = LuaDLL.lua_gettop(L);
        NetReader reader = (NetReader)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetReader");
        if (count == 0)
        {
            byte[] o = reader.readBytes();
            LuaScriptMgr.PushArray(L, o);
            return 1;
        }
        else if (count == 1)
        {
            int arg0 = (int)LuaDLL.lua_tonumber(L, 1);
            byte[] o = reader.readBytes(arg0);
            LuaScriptMgr.PushArray(L, o);
            return 1;
        }

        return 0;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int readInt64(IntPtr L)
    {
        NetReader reader = (NetReader)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetReader");
        Int64 o = reader.readInt64();
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int ReadReverseString(IntPtr L)
    {
        NetReader reader = (NetReader)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetReader");
        string o = reader.ReadReverseString();
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int readString(IntPtr L)
    {
        NetReader reader = (NetReader)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetReader");
        string o = reader.readString();
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int recordBegin(IntPtr L)
    {
        NetReader reader = (NetReader)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetReader");
        bool o = reader.recordBegin();
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int recordEnd(IntPtr L)
    {
        NetReader reader = (NetReader)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetReader");
        reader.recordEnd();
        return 0;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int ReverseInt(IntPtr L)
    {
        NetReader reader = (NetReader)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetReader");
        int o = reader.ReverseInt();
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int SetBuffer(IntPtr L)
    {
        int count = LuaDLL.lua_gettop(L);
        if (count == 1)
        {
            NetReader reader = (NetReader)LuaScriptMgr.GetNetObjectSelf(L, 1, "NetReader");
            byte[] objs0 = LuaScriptMgr.GetArrayNumber<byte>(L, 1);
            reader.SetBuffer(objs0);
        }
        return 0;
    }

    #endregion

    #region Fields
    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int get_ActionId(IntPtr L)
    {
        object o = LuaScriptMgr.GetLuaObject(L, 1);
        NetReader obj = (NetReader)o;

        if (obj == null)
        {
            LuaTypes types = LuaDLL.lua_type(L, 1);

            if (types == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name ActionId");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index ActionId on a nil value");
            }
        }

        LuaScriptMgr.PushObject(L, obj.ActionId);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int get_Buffer(IntPtr L)
    {
        object o = LuaScriptMgr.GetLuaObject(L, 1);
        NetReader obj = (NetReader)o;

        if (obj == null)
        {
            LuaTypes types = LuaDLL.lua_type(L, 1);

            if (types == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name Buffer");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index Buffer on a nil value");
            }
        }

        LuaScriptMgr.PushObject(L, obj.Buffer);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int get_Description(IntPtr L)
    {
        object o = LuaScriptMgr.GetLuaObject(L, 1);
        NetReader obj = (NetReader)o;

        if (obj == null)
        {
            LuaTypes types = LuaDLL.lua_type(L, 1);

            if (types == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name Description");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index Description on a nil value");
            }
        }

        LuaScriptMgr.PushObject(L, obj.Description);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int get_RmId(IntPtr L)
    {
        object o = LuaScriptMgr.GetLuaObject(L, 1);
        NetReader obj = (NetReader)o;

        if (obj == null)
        {
            LuaTypes types = LuaDLL.lua_type(L, 1);

            if (types == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name RmId");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index RmId on a nil value");
            }
        }

        LuaScriptMgr.PushObject(L, obj.RmId);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int get_StatusCode(IntPtr L)
    {
        object o = LuaScriptMgr.GetLuaObject(L, 1);
        NetReader obj = (NetReader)o;

        if (obj == null)
        {
            LuaTypes types = LuaDLL.lua_type(L, 1);

            if (types == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name StatusCode");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index StatusCode on a nil value");
            }
        }

        LuaScriptMgr.PushObject(L, obj.StatusCode);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int get_StrTime(IntPtr L)
    {
        object o = LuaScriptMgr.GetLuaObject(L, 1);
        NetReader obj = (NetReader)o;

        if (obj == null)
        {
            LuaTypes types = LuaDLL.lua_type(L, 1);

            if (types == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name StrTime");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index StrTime on a nil value");
            }
        }

        LuaScriptMgr.PushObject(L, obj.StrTime);
        return 1;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int get_Success(IntPtr L)
    {
        object o = LuaScriptMgr.GetLuaObject(L, 1);
        NetReader obj = (NetReader)o;

        if (obj == null)
        {
            LuaTypes types = LuaDLL.lua_type(L, 1);

            if (types == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name Success");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index Success on a nil value");
            }
        }

        LuaScriptMgr.PushObject(L, obj.Success);
        return 1;
    }

    #endregion
}
