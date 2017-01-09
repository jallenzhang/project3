using System;
using System.Threading;
using System.Collections.Generic;
using GameServer.Script.Model;
using ZyGames.Framework.Cache.Generic;
using ZyGames.Framework.Common;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.Net;

namespace GameServer.Script.CsScript.Action
{
    /// <summary>
    /// 1004_登录
    /// </summary>
    public class Action1004 : BaseStruct
    {
        private string _pid;


        private GameUser _user;


        public Action1004(HttpGet httpGet)
            : base(1004, httpGet)
        {

        }

        public override void BuildPacket()
        {
            this.PushIntoStack(Current.SessionId);
            this.PushIntoStack(_user.UserId);
            this.PushIntoStack(_user.NickName);
            int guideId = string.IsNullOrEmpty(_user.NickName) ? 1005 : 1006;

            this.PushIntoStack(guideId);

        }
         
        public override bool GetUrlElement()
        {
            if (httpGet.GetString("Pid", ref _pid))
            {
                return true;
            }
            return false;
        }

        public override bool TakeAction()
        {

            var iemiCache = new ShareCacheStruct<IemiEntity>();


            var cache = new PersonalCacheStruct<GameUser>();


            var entity = iemiCache.FindKey(_pid);

            if (entity == null) {
                var filter = new DbDataFilter();
                filter.Condition = "IEMI = @IEMI";
                filter.Parameters.Add("IEMI", _pid);
                iemiCache.TryRecoverFromDb(filter,_pid);

                entity = iemiCache.FindKey(_pid);
            }

            if (entity == null)
            {

                Console.WriteLine("1004-------------IemiEntity Is Null");

                entity = new IemiEntity();
                entity.IEMI = _pid;

                _user = new GameUser()
                {
                    UserId = (int)cache.GetNextNo(),
                    IEMI = _pid,
                    Searching = false,
                    RivalId = -1
                };

                entity.UserId = _user.UserId;
                 

                iemiCache.AddOrUpdate(entity);

                cache.AddOrUpdate(_user);

                Current.Bind(_user);
            }
            else {


                int userId = entity.UserId;

                _user = cache.FindKey(userId.ToString());

                if (_user == null)
                {
                    var filter = new DbDataFilter();
                    filter.Condition = "UserId = @UserId";
                    filter.Parameters.Add("UserId", userId);
                    var ii = cache.TryRecoverFromDb(filter, userId.ToString());

                    Console.WriteLine(string.Format("1004-------------UserId:{0},IEMI:{1}", userId, entity.IEMI));
                }

                if (cache.TryFindKey(entity.UserId.ToString(), out _user,entity.UserId.ToString()) == ZyGames.Framework.Model.LoadingStatus.Success)
                {
                    Current.Bind(_user);
                }
                else {

                }
            }
          

            return true;
        }
    }
}
