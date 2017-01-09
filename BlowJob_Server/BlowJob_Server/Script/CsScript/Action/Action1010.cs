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
    public class Action1010 : BaseStruct
    {

        private int _uid;

        private GameUser _user;

          public Action1010(HttpGet httpGet)
            : base(1010, httpGet)
        {

        }

          public override bool GetUrlElement()
          {
              if (httpGet.GetInt("UserId", ref _uid))
              {
                  return true;
              }
              return false;
          }

          public override bool TakeAction()
          {
              var cache = new PersonalCacheStruct<GameUser>();



              _user = (GameUser)Current.User;// cache.FindKey(_uid.ToString());




              return true;


          }

          public override void TakeActionAffter(bool state)
          {

              //ms.WriteByte(_message);
             
              var cUser=(GameUser)Current.User;



              MessageStructure ms = new MessageStructure();
              //ms.PushIntoStack();
              ms.PushIntoStack(true);

              ms.WriteBuffer(new MessageHead(2010));

              byte[] buffer = ms.PopBuffer();

              //Current.SendAsync(buffer, 0, buffer.Length);

              var rUser = GameSession.GetOnlineAll(10 * 1000).FirstOrDefault(r => r.UserId == cUser.RivalId);

                  //list.ForEach(r =>
                  //{
                  //    if (r.SessionId != Current.SessionId)
                  //    r.SendAsync(OpCode.Text, buffer, 0, buffer.Length, asyncResult =>
                  //    {
                  //        Console.WriteLine("The results of data send:{0}", asyncResult.Result == ResultCode.Success ? "ok" : "fail");
                  //    });
                  //});

              if (rUser != null)
              { 
                rUser.SendAsync(OpCode.Text, buffer, 0, buffer.Length, asyncResult =>
                      {
                          Console.WriteLine("The results of data send:{0}", asyncResult.Result == ResultCode.Success ? "ok" : "fail");
                      });

              }

              if (cUser != null)
              {
                  cUser.ModifyLocked(() =>
                  {
                      cUser.RoomId = string.Empty;
                      cUser.Searching = false;
                      cUser.RivalId = -1;
                  });
              }

              base.TakeActionAffter(state);
          }


    }
}
