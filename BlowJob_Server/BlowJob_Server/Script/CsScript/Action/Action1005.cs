using System;
using System.Collections.Generic;
using GameServer.Script.Model;
using ZyGames.Framework.Cache.Generic;
using ZyGames.Framework.Common;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.Game.Service;

namespace GameServer.Script.CsScript.Action
{
    public class Action1005 : BaseStruct
    {
        private string _uname;
        private string _pid;

        private int _uid;

        private GameUser _user;


        public Action1005(HttpGet httpGet)
            : base(1005, httpGet)
        {

        }

        public override void BuildPacket()
        {
            this.PushIntoStack(Current.SessionId);
            this.PushIntoStack(_user.UserId);
            this.PushIntoStack(_user.NickName);
            this.PushIntoStack(1006);

        }

        public override bool GetUrlElement()
        {
            if (httpGet.GetString("Pid", ref _pid) && httpGet.GetInt("UserId", ref _uid) && httpGet.GetString("Uname", ref _uname))
            {
                return true;
            }
            return false;
        }

        public override bool TakeAction()
        {
            var cache = new PersonalCacheStruct<GameUser>();

            _user = (GameUser)Current.User;

            if (_user != null)
            {
                //_user.ModifyLocked(() =>
                //{
                //    _user.NickName = _uname;
                //});

                _user.NickName = _uname;

                cache.AddOrUpdate(_user);

                //var cacheUser = cache.FindKey(_user.IEMI);

                //if (cacheUser==null)
                //{
                //    Console.WriteLine("null-------1005");
                //}
                //else {

                //    cacheUser.ModifyLocked(() =>
                //    {
                //        cacheUser.NickName = _uname;
                //    });

                //    //cache.ReLoad(cacheUser.PersonalId);
                //}

            }
            else {
                    throw new Exception("去你妈的");


            }

            return true;
        }
    }
}
