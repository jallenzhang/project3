#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：Utils
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.1.16
// 模块描述：工具类。
//----------------------------------------------------------------*/
#endregion
using Mogo.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MsgPack;

namespace Mogo.RPC
{

    /// <summary>
    /// 工具类。
    /// </summary>
    public class Utils
    {
        private Utils()
        {
        }

        /// <summary>
        /// 填充数据长度头。
        /// </summary>
        /// <param name="srcData">源二进制数组</param>
        /// <returns>填充二进制数组长度到头部的数据</returns>
        public static Byte[] FillLengthHead(Byte[] srcData)
        {
            return FillLengthHead(srcData, (UInt16)srcData.Length);
        }

        /// <summary>
        /// 填充数据长度头。
        /// </summary>
        /// <param name="srcData">源二进制数组</param>
        /// <param name="length">源二进制数组数据长度</param>
        /// <returns>填充二进制数组长度到头部的数据</returns>
        public static Byte[] FillLengthHead(Byte[] srcData, UInt16 length)
        {
            Byte[] lengthByteArray = BitConverter.GetBytes(length);//将字符串长度转换为二进制数组
            //Array.Reverse(lengthByteArray);
            Byte[] result = new Byte[length + lengthByteArray.Length];//申请存放字符串长度和字符串内容的空间
            Buffer.BlockCopy(lengthByteArray, 0, result, 0, lengthByteArray.Length);//将长度的二进制数组拷贝到目标空间
            Buffer.BlockCopy(srcData, 0, result, lengthByteArray.Length, length);//将字符串的二进制数组拷贝到目标空间
            return result;
        }
    }
}