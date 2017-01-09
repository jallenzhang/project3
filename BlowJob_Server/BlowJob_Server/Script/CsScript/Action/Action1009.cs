using GameServer.Script.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZyGames.Framework.Cache.Generic;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.RPC.IO;
using ZyGames.Framework.RPC.Sockets;

namespace GameServer.Script.CsScript.Action
{
    /// <summary>
    /// 寻找对手
    /// </summary>
    public class Action1009 : BaseStruct
    {
        private int _uid;
        private GameUser _user;

         public Action1009(HttpGet httpGet)
            : base(1009, httpGet)
        {

        }

        public override bool TakeAction()
        {
            var cache = new PersonalCacheStruct<GameUser>();


            _user = (GameUser)Current.User;//cache.FindKey(_uid.ToString());

            if (_user != null)
            {
                _user.ModifyLocked(() =>
                {
                   // _user.RoomId =Guid.NewGuid().ToString();
                    _user.Searching = true;
                });

                //Console.WriteLine(string.Format("来申请了，UserId:{0}",_uid));
            }


            return true;
        }

        public override bool GetUrlElement()
        {
            if (httpGet.GetInt("UserId", ref _uid))
            {
                return true;
            }
            return false;
        }

        public override void TakeActionAffter(bool state)
        {


            GameSession gameSession=null;

            GameSession.GetOnlineAll(10 * 1000).ForEach(r =>
            {
                var user = (GameUser)r.User;
                if (r.UserId != Current.UserId && user.Searching)
                {
                    gameSession = r;
                    return;
                }
            });

            if (gameSession!=null)
            {
                MessageStructure ms = new MessageStructure();
                //ms.PushIntoStack();
                ms.PushIntoStack(true);

                ms.WriteBuffer(new MessageHead(2009));

                byte[] buffer = ms.PopBuffer();


                Current.SendAsync(OpCode.Text, buffer, 0, buffer.Length, asyncResult =>
                    {
                        Console.WriteLine("The results of data send:{0}", asyncResult.Result == ResultCode.Success ? "ok" : "fail");
                    });

                gameSession.SendAsync(OpCode.Text, buffer, 0, buffer.Length, asyncResult =>
                {
                    Console.WriteLine("The results of data send:{0}", asyncResult.Result == ResultCode.Success ? "ok" : "fail");
                });

                var cUser = (GameUser)Current.User;
                var vUser = (GameUser)gameSession.User;

                cUser.ModifyLocked(() => {
                    cUser.Searching = false;
                    cUser.RivalId = vUser.UserId;
                });
                vUser.ModifyLocked(() =>
                {
                    vUser.Searching = false;
                    vUser.RivalId = cUser.UserId;
                });
            }

            //ms.WriteByte(_message);
            //var cache = new PersonalCacheStruct<GameUser>();

            //var persons= cache.FindGlobal(r=>r.Searching==true);//.Where(r => r.Searching == true);

            //bool start = false;

            //if (persons.Count()>1)
            //{
            //    start = true;
            //}

            //MessageStructure ms = new MessageStructure();
            ////ms.PushIntoStack();
            //ms.PushIntoStack(start);

            //ms.WriteBuffer(new MessageHead(2009));

            //byte[] buffer = ms.PopBuffer();

            ////Current.SendAsync(buffer, 0, buffer.Length);

            //var list = GameSession.GetOnlineAll(10 * 1000);

            //if (start)
            //{
            //    list.ForEach(r =>
            //    {
            //        //if (r.SessionId != Current.SessionId)
            //        r.SendAsync(OpCode.Text, buffer, 0, buffer.Length, asyncResult =>
            //        {
            //            Console.WriteLine("The results of data send:{0}", asyncResult.Result == ResultCode.Success ? "ok" : "fail");
            //        });
            //    });
            //    foreach (var p in persons)
            //    {
            //        p.ModifyLocked(() =>
            //        {
            //            //_user.RoomId = Guid.NewGuid().ToString();
            //            _user.Searching = false;
            //        });
            //    }
            //}

            base.TakeActionAffter(state);
        }
        

    }
}
