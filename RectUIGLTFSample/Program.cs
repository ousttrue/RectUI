using DesktopDll;
using RectUI;
using RectUI.Assets;
using RectUI.Widgets;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RectUIGLTF
{
    class FileDialog
    {
        public RectRegion UI;
        public Window Window;

        event Action<FileInfo> m_fileSelected;

        public class AwaiterContext
        {
            public Awaiter Awaiter;
            public Action Complete;
            public Action<FileSystemInfo> SetResult;
        }
        AwaiterContext m_context;

        public FileDialog(Window parent)
        {
            Window = parent.CreateModal(400, 300);
            Window.OnClose += () =>
            {
                var context = m_context;
                m_context = null;
                context.Complete();
            };

            m_fileSelected += (f) =>
            {
                m_context.SetResult(f);
                parent.Enable();
                Window.Close();
            };

            var source = new DirSource();
            source.Entered += obj =>
            {
                var f = obj as FileInfo;
                if (f == null)
                {
                    // dir...
                    return;
                }
                m_fileSelected?.Invoke(f);
            };

            UI = new PanelRegion
            {
                new ListRegion<FileSystemInfo>(source)
                {
                    Anchor= new Anchor
                    {
                        Left=5,
                        Top = 20,
                        Right = 5,
                        Bottom= 36,
                    }
                },

                new ButtonRegion(_ =>
                {

                })
                {
                    Content = "Open",
                    Rect = new Rect(96, 24),
                    Anchor = new Anchor
                    {
                        Bottom=5, Left=5,
                    }
                },

                new ButtonRegion(_ =>
                {

                })
                {
                    Content = "Cancel",
                    Rect = new Rect(96, 24),
                    Anchor = new Anchor
                    {
                        Bottom=5, Right=5,
                    }
                },
            };
        }

        public Awaitable OpenAsync()
        {
            Window.Show(SW.SHOW);

            if (m_context != null)
            {
                throw new Exception();
            }

            m_context = Awaiter.Create();

            return new Awaitable(m_context.Awaiter);
        }

        public class Awaitable
        {
            Awaiter m_awaiter;

            public Awaitable(Awaiter awaiter)
            {
                m_awaiter = awaiter;
            }

            public Awaiter GetAwaiter()
            {
                return m_awaiter;
            }
        }

        public class Awaiter : INotifyCompletion
        {
            FileSystemInfo m_result;
            bool m_isCompleted;

            Awaiter() { }

            public static AwaiterContext Create()
            {
                var awaiter = new Awaiter();
                return new AwaiterContext
                {
                    Awaiter = awaiter,
                    Complete = () =>
                    {
                        awaiter.m_isCompleted = true;
                        awaiter.m_continuation();
                    },
                    SetResult = f => awaiter.m_result = f,
                };
            }

            public bool IsCompleted => m_isCompleted;

            Action m_continuation;
            public void OnCompleted(Action continuation) {
                m_continuation = continuation;
            }

            public FileSystemInfo GetResult()
            {
                return m_result;
            }
        }
    }

    class Program
    {
        static RectRegion BuildUI(Window dialog, 
            Scene scene,
            Action onOpen)
        {
            return new PanelRegion
            {
                new ButtonRegion(_ => onOpen())
                {
                    Rect = new Rect(200, 40),
                    Anchor=new Anchor{
                        Left = 5,
                        Top = 5,
                    },
                    Content = "open",
                },

                new D3DRegion(scene)
                {
                    Rect = new Rect(200, 40),
                    Anchor=new Anchor{
                        Left = 5,
                        Top = 50,
                        Bottom = 5,
                        Right = 5,
                    },
                },
            };
        }

        [STAThread]
        static void Main(string[] args)
        {
            var scene = new Scene();

            using (var app = new App())
            {
                var window = Window.Create(SW.SHOW);
                var dialog = new FileDialog(window);

                app.Bind(dialog.Window, dialog.UI);

                Action onOpen = async () =>
                {
                    var f = await dialog.OpenAsync();
                    if (f != null)
                    {
                        try
                        {
                            Console.WriteLine($"open: {f.FullName}");
                            var source = await Task.Run(() => AssetSource.Load(f.FullName));
                            Console.WriteLine($"loaded: {source}");
                            var asset = await Task.Run(() => AssetContext.Load(source));
                            Console.WriteLine($"build: {source}");
                            scene.Asset = asset;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                };
                app.Bind(window, BuildUI(dialog.Window, scene, onOpen));

                Window.MessageLoop();
            }
        }
    }
}
