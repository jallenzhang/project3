/****************************************************************************
Copyright (c) 2013-2015 scutgame.com

http://www.scutgame.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
****************************************************************************/

using System;
using ProtoBuf;
using ZyGames.Framework.Game.Context;
using ZyGames.Framework.Model;

namespace GameServer.Script.Model
{
    [Serializable, ProtoContract]
    [EntityTable(CacheType.Dictionary, "ConnData")]
    public class GameUser : BaseEntity,IUser
    {

        public GameUser()
            : base(false)
        {
        }


        [ProtoMember(1)]
        [EntityField(true)]
        public int UserId { get; set; }


        [ProtoMember(2)]
        [EntityField]
        public String IEMI
        {
            get;
            set;
        }


        [ProtoMember(3)]
        [EntityField]
        public String NickName
        {
            get;
            set;
        }


        [ProtoMember(4)]
        [EntityField]
        public String RoomId
        {
            get;
            set;
        }

        [ProtoMember(5)]
        [EntityField]
        public bool Searching { get; set; }

        [ProtoMember(6)]
        [EntityField]
        public int RivalId { get; set; }


        private bool _isonline;

        public string GetPassportId()
        {
            return IEMI;
        }

        public int GetUserId()
        {
            return UserId;
        }

        public bool IsOnlining
        {
            get { return _isonline; }
        }

        public void RefleshOnlineDate()
        {
            //throw new NotImplementedException();
            _isonline = true;
        }

        public string Token
        {
            get
            {
                return IEMI;
            }
            set
            {
                
            }
        }

        protected override int GetIdentityId()
        {
            return UserId;
        }
    }

}