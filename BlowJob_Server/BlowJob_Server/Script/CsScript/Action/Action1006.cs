using System;
using System.Collections.Generic;
using GameServer.Script.Model;
using ZyGames.Framework.Cache.Generic;
using ZyGames.Framework.Common;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.RPC.Sockets;
using System.Text;
using ZyGames.Framework.RPC.IO;

namespace GameServer.Script.CsScript.Action
{
    public class Action1006 : BaseStruct
    {
        private string _pid;

        private int _uid;

        private string _message;



        public Action1006(HttpGet httpGet)
            : base(1006, httpGet)
        {

        }

        public override void BuildPacket()
        {
            //this.PushIntoStack(Current.SessionId);
            //this.PushIntoStack(_user.UserId);
            //this.PushIntoStack(_user.NickName);
            //this.PushIntoStack(1006);

        }

        public override bool GetUrlElement()
        {

            if (httpGet.GetString("Pid", ref _pid) && httpGet.GetInt("UserId", ref _uid)&&httpGet.GetString("Message",ref _message))
            {
                return true;
            }
            return false;
        }

        public override bool TakeAction()
        {

            Console.WriteLine("send message:{0}", _message);

            //var cache = new ShareCacheStruct<GameUser>();
            //_uid = 1;
            //_user = cache.FindKey(_uid.ToString());

            //if (_user != null) {
            //    _user.ModifyLocked(() => {
            //        _user.NickName = _uname;
            //    });
            //}
            //_user.NickName = _uname;

            return true;
        }


        public override void TakeActionAffter(bool state)
        {
            var list = GameSession.GetOnlineAll(10 * 1000);

            //byte[] data = Encoding.UTF8.GetBytes(_message);

            MessageStructure ms = new MessageStructure();
            //ms.WriteByte(_message);
            ms.PushIntoStack(_message);
            ms.WriteBuffer(new MessageHead(2006));

            byte[] buffer = ms.PopBuffer();

            list.ForEach(r => {
                if(r.SessionId!=Current.SessionId)
                    r.SendAsync(OpCode.Text, buffer, 0, buffer.Length, asyncResult =>
                {
                    Console.WriteLine("The results of data send:{0}", asyncResult.Result == ResultCode.Success ? "ok" : "fail");
                });
            });
            base.TakeActionAffter(state);
        }
    }
}
