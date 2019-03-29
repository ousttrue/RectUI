using DesktopDll;
using RectUI;
using RectUI.Assets;
using RectUI.Widgets;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


namespace RectUIGLTFSample
{
    class FileDialog
    {
        public RectRegion UI;
        public Window Window;

        public event Action<FileSystemInfo> FileChanged;

        public class AwaiterContext
        {
            public Awaiter Awaiter;
            public Action Complete;
            public Action<FileInfo> SetResult;
        }
        AwaiterContext m_context;

        public FileDialog(Window parent, string openDir)
        {
            Window = parent.CreateModal(400, 300);
            Window.OnClose += () =>
            {
                var context = m_context;
                m_context = null;
                context.Complete();
            };

            FileChanged += (obj) =>
            {
                var f = obj as FileInfo;
                if (f != null)
                {
                    m_context.SetResult(f);
                    parent.Enable();
                    Window.Close();
                }
            };

            var source = new DirSource(Path.GetDirectoryName(openDir));

            source.Entered += obj =>
            {
                FileChanged?.Invoke(obj);
            };

            UI = new VBoxRegion()
            {
                new ListRegion<FileSystemInfo>(source)
                {
                    BoxItem = BoxItem.Expand,
                },

                new HBoxRegion(new Rect(200, 40))
                {
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
                    }
                }
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
            public void OnCompleted(Action continuation)
            {
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
            return new VBoxRegion()
            {
                new ButtonRegion(_ => onOpen())
                {
                    Rect = new Rect(200, 40),
                    Content = "open",
                },

                new D3DRegion
                {
                    Content = scene,
                    BoxItem = BoxItem.Expand,
                },
            };
        }

        class Manager : IDisposable
        {
            Scene m_scene = new Scene();
            App m_app = new App();

            public void Dispose()
            {
                m_scene.Dispose();
                m_app.Dispose();
            }

            public Manager()
            {
                State.Restore();

                var task = Load(State.Instance.OpenFile);
            }

            async Task Load(string path)
            {
                if (string.IsNullOrEmpty(path))
                {
                    return;
                }

                Console.WriteLine($"open: {path}");
                var source = await Task.Run(() => AssetSource.Load(path));
                Console.WriteLine($"loaded: {source}");
                var asset = await Task.Run(() => AssetContext.Load(source));
                Console.WriteLine($"build: {source}");
                m_scene.Asset = asset;
            }

            public void Run()
            {
                var window = Window.Create(SW.SHOW);
                var dialog = new FileDialog(window, State.Instance.OpenDir);
                dialog.FileChanged += obj =>
                {
                    var f = obj as FileInfo;
                    if (f != null)
                    {
                        State.Instance.OpenFile = f.FullName;
                    }
                    var d = obj as DirectoryInfo;
                    if (d != null)
                    {
                        State.Instance.OpenDir = d.FullName;
                    }
                };

                m_app.Bind(dialog.Window, dialog.UI);

                Action onOpen = async () =>
                {
                    var f = await dialog.OpenAsync();
                    if (f != null)
                    {
                        try
                        {
                            await Load(f.FullName);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                };
                m_app.Bind(window, BuildUI(dialog.Window, m_scene, onOpen));

                Window.MessageLoop();

                State.Save();
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            using (var man = new Manager())
            {
                man.Run();
            }
        }
    }
}
