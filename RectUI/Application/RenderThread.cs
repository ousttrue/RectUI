using DesktopDll;
using RectUI.Assets;
using RectUI.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


namespace RectUI.Application
{
    public class RenderThread : IDisposable
    {
        Thread m_thread;
        public void Dispose()
        {
            if (m_thread != null)
            {
                EnqueueExit();
                m_thread.Join();
                m_thread = null;
            }
        }

        enum ThreadCommand
        {
            None,
            Asset,
            AddTarget,
            ResizeTarget,
            DestroyTarget,
            ExecuteCommands, // 描画コマンド
            Quit, // RenderThreadを終了する
        }

        struct Command
        {
            public ThreadCommand ThreadCommand;
            public AssetContext Asset;
            public Window Target;
            public int Width;
            public int Height;
            public CommandList Commands;
        }

        List<Command> m_commandQueue = new List<Command>();
        bool Dequeue(ref Command command)
        {
            lock (((ICollection)m_commandQueue).SyncRoot)
            {
                if (m_commandQueue.Count == 0)
                {
                    return false;
                }
                else
                {
                    command = m_commandQueue[0];
                    m_commandQueue.RemoveAt(0);
                    return true;
                }
            }
        }

        void Enqueue(Command command)
        {
            lock (((ICollection)m_commandQueue).SyncRoot)
            {
                m_commandQueue.Add(command);
            }
        }

        void EnqueueExit()
        {
            Enqueue(new Command
            {
                ThreadCommand = ThreadCommand.Quit,
            });
        }

        public void EnqueueAsset(AssetContext asset)
        {
            Enqueue(new Command
            {
                ThreadCommand = ThreadCommand.Asset,
                Asset = asset,
            });
        }

        public void EnqueueWindow(Window target)
        {
            Enqueue(new Command
            {
                ThreadCommand = ThreadCommand.AddTarget,
                Target = target,
            });
        }

        public void EnqueueWindowResize(Window target, int w, int h)
        {
            Enqueue(new Command
            {
                ThreadCommand = ThreadCommand.ResizeTarget,
                Target = target,
                Width = w,
                Height = h,
            });
        }

        public void EnqueueWindowDestroy(Window target)
        {
            Enqueue(new Command
            {
                ThreadCommand = ThreadCommand.DestroyTarget,
                Target = target,
            });
        }

        public void EnqueueCommand(Window target, CommandList commands)
        {
            Enqueue(new Command
            {
                ThreadCommand = ThreadCommand.ExecuteCommands,
                Target = target,
                Commands = commands,
            });
        }

        public RenderThread()
        {
            m_thread = new Thread(RenderLoop);
            m_thread.Start();
        }

        void RenderLoop()
        {
            var commandMap = new Dictionary<Window, CommandList>();

            using (var context = new Context())
            {
                while (true)
                {
                    commandMap.Clear();
                    while (true)
                    {
                        var command = default(Command);
                        if (!Dequeue(ref command))
                        {
                            break;
                        }

                        switch (command.ThreadCommand)
                        {
                            case ThreadCommand.Quit:
                                return; // quit thread

                            case ThreadCommand.Asset:
                                context.Scene.Asset = command.Asset;
                                break;

                            case ThreadCommand.ExecuteCommands:
                                {
                                    CommandList list;
                                    if(commandMap.TryGetValue(command.Target, out list)){
                                        list.Release();
                                    }
                                    commandMap[command.Target] = command.Commands;
                                }
                                break;

                            case ThreadCommand.AddTarget:
                                context.AddWindow(command.Target);
                                break;

                            case ThreadCommand.DestroyTarget:
                                context.DestroyWindow(command.Target);
                                break;

                            case ThreadCommand.ResizeTarget:
                                context.ResizeWindow(command.Target, command.Width, command.Height);
                                break;
                        }
                    }

                    if (commandMap.Any())
                    {
                        foreach (var kv in commandMap)
                        {
                            var list = kv.Value;
                            context.Draw(kv.Key, list.Rpc.MsgPackBytes);
                            list.Release();
                        }
                    }
                    else
                    {
                        // 30FPSくらい
                        Thread.Sleep(30);
                    }
                }
            }
        }
    }
}
