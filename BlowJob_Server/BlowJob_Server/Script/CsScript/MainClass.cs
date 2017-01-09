using GameServer.Script.Model;
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
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.Game.Runtime;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.RPC.IO;
using ZyGames.Framework.RPC.Sockets;
using ZyGames.Framework.Script;

namespace Game.Script
{
    public class MainClass : GameSocketHost, IMainScript
    {
        public MainClass()
        {
        }

        protected override void OnStartAffer()
        {
            Console.WriteLine("服务器成功启动");

        }

        protected override void OnServiceStop()
        {
            GameEnvironment.Stop();
        }

        protected override void OnConnectCompleted(object sender, ConnectionEventArgs e)
        {
            Console.WriteLine("客户端IP:[{0}]已与服务器连接成功", e.Socket.RemoteEndPoint);
            base.OnConnectCompleted(sender, e);
        }

        protected override void OnDisconnected(GameSession session)
        {
            Console.WriteLine("客户端UserId:[{0}]已与服务器断开", session.UserId);

            var cUser = (GameUser)session.User;
            if (cUser != null)
            {
                MessageStructure ms = new MessageStructure();
                //ms.PushIntoStack();
                ms.PushIntoStack(true);

                ms.WriteBuffer(new MessageHead(2010));

                byte[] buffer = ms.PopBuffer();

                //Current.SendAsync(buffer, 0, buffer.Length);

                Console.WriteLine("客户端RivalId:[{0}]将会收到离开消息", cUser.RivalId);


                var rUser = GameSession.GetOnlineAll(10 * 1000).Find(r => r.UserId == cUser.RivalId);



                if (rUser != null)
                {
                    rUser.SendAsync(OpCode.Text, buffer, 0, buffer.Length, asyncResult =>
                    {
                        Console.WriteLine("The results of data send:{0}", asyncResult.Result == ResultCode.Success ? "ok" : "fail");
                    });
                }
                else {
                    Console.WriteLine("客户端RivalUser为空");
                }
            }
            else {
                Console.WriteLine("客户端GameUser为空");
            }

            base.OnDisconnected(session);
        }

        protected override void OnHeartbeat(GameSession session)
        {
            Console.WriteLine("{0}>>Hearbeat package: {1} userid {2} session count {3}", DateTime.Now.ToString("HH:mm:ss"), session.RemoteAddress, session.UserId, GameSession.Count);
            base.OnHeartbeat(session);
        }

        protected override void OnRequested(ActionGetter actionGetter, BaseGameResponse response)
        {
            Console.WriteLine("Client {0} request action {1}", actionGetter.GetSessionId(), actionGetter.GetActionId());
        }
    }
}