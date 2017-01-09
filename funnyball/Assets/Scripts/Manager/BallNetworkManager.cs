using UnityEngine;
using System.Collections;
using SimpleFramework;

namespace FunnyBall
{
    public class BallNetworkManager : View {
        public string mNetworkAdress = "42.96.149.51:9001";
        void Awake()
        {
            Init();
        }

        void Init()
        {
            NetWriter.SetUrl(mNetworkAdress);
        }

        public void OnInit()
        {
            CallMethod("Start");
        }

        public void Unload()
        {
            CallMethod("Unload");
        }

        /// <summary>
        /// 执行Lua方法
        /// </summary>
        public object[] CallMethod(string func, params object[] args)
        {
            return Util.CallMethod("Network", func, args);
        }

        ///------------------------------------------------------------------------------------
        public static void AddEvent(int _event, ByteBuffer data)
        {
            //sEvents.Enqueue(new KeyValuePair<int, ByteBuffer>(_event, data));
        }

        /// <summary>
        /// 交给Command，这里不想关心发给谁。
        /// </summary>
        void Update()
        {
            //if (sEvents.Count > 0)
            //{
            //    while (sEvents.Count > 0)
            //    {
            //        KeyValuePair<int, ByteBuffer> _event = sEvents.Dequeue();
            //        facade.SendMessageCommand(NotiConst.DISPATCH_MESSAGE, _event);
            //    }
            //}
        }

        /// <summary>
        /// 发送SOCKET消息
        /// </summary>
        public void SendMessage(ByteBuffer buffer)
        {
            Net.Instance.Send((int)ActionType.Login, null, null);
            //SocketClient.SendMessage(buffer);
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        new void OnDestroy()
        {
            
        }
    }

}
