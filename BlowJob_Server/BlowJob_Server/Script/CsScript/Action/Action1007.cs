using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.RPC.IO;
using ZyGames.Framework.RPC.Sockets;

namespace GameServer.Script.CsScript.Action
{
    /// <summary>
    /// 点下去
    /// </summary>
    public class Action1007: BaseStruct
    {

        private string _x;
        private string _y;
        private string _z;

        private string _qx;
        private string _qy;
        private string _qz;

        private string _sx;
        private string _sy;
        private string _sz;

        public Action1007(HttpGet httpGet)
            : base(1007, httpGet)
        {

        }

        public override void BuildPacket()
        {
            this.PushIntoStack(true);
        }

        public override bool GetUrlElement()
        {

            if (httpGet.GetString("x", ref _x) && httpGet.GetString("y", ref _y) && httpGet.GetString("z", ref _z))
            {
                httpGet.GetString("qx", ref _qx);
                httpGet.GetString("qy", ref _qy);
                httpGet.GetString("qz", ref _qz);

                httpGet.GetString("sx", ref _sx);
                httpGet.GetString("sy", ref _sy);
                httpGet.GetString("sz", ref _sz);

                return true;
            }
            return false;
        }

        public override bool TakeAction()
        {
            return true;
        }

        public override void TakeActionAffter(bool state)
        {

            MessageStructure ms = new MessageStructure();
            //ms.WriteByte(_message);
            ms.PushIntoStack(float.Parse(_x));
            ms.PushIntoStack(float.Parse(_y));
            ms.PushIntoStack(float.Parse(_z));
            ms.PushIntoStack(float.Parse(_qx));
            ms.PushIntoStack(float.Parse(_qy));
            ms.PushIntoStack(float.Parse(_qz));
            ms.PushIntoStack(float.Parse(_sx));
            ms.PushIntoStack(float.Parse(_sy));
            ms.PushIntoStack(float.Parse(_sz));
            ms.PushIntoStack(DateTime.Now);
            ms.WriteBuffer(new MessageHead(2007));

            byte[] buffer = ms.PopBuffer();

            var list = GameSession.GetOnlineAll(10 * 1000);

            list.ForEach(r =>
            {
                if (r.SessionId != Current.SessionId)
                    r.SendAsync(OpCode.Text, buffer, 0, buffer.Length, asyncResult =>
                    {
                        Console.WriteLine("The results of data send:{0}", asyncResult.Result == ResultCode.Success ? "ok" : "fail");
                    });
            });
            //Current.SendAsync(buffer,0,buffer.Length);

            base.TakeActionAffter(state);
        }
    }
}
