using DesktopDll;
using RectUI.Application;
using RectUI.Assets;
using RectUI.Widgets;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace RectUIGLTFSample
{
    class Program
    {
        static RectRegion BuildUI(
            Action onOpen)
        {
            return new VBoxRegion()
            {
                new ButtonRegion
                {
                    Action = _ => onOpen(),
                    Rect = new Rect(200, 40),
                    Label = "open",
                },

                {BoxItem.Expand, new HBoxRegion
                {
                    new RectRegion
                    {
                        Rect = new Rect(200, 40),
                    },

                    {BoxItem.Expand, new D3DRegion()}
                }}
            };
        }

        class Manager : IDisposable
        {
            App m_app = new App();

            public void Dispose()
            {
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

                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} open: {path}");
                var source = await Task.Run(() => AssetSource.Load(path));
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} loaded: {source}");
                var asset = await Task.Run(() => AssetContext.Load(source));
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} build: {source}");
                m_app.SetAsset(asset);
            }

            public void Run()
            {
                var window = Window.Create(SW.SHOW);
                var dialogWindow = window.CreateModal(400, 300);

                var dialog = new FileDialog(State.Instance.OpenDir);

                dialog.BeginDialogOpen += () =>
                {
                    dialogWindow.Show(SW.SHOW);
                };

                dialog.FileChanged += obj =>
                {
                    var f = obj as FileInfo;
                    if (f != null)
                    {
                        State.Instance.OpenFile = f.FullName;

                        window.Enable();
                        dialogWindow.Close();

                        return;
                    }

                    var d = obj as DirectoryInfo;
                    if (d != null)
                    {
                        State.Instance.OpenDir = d.FullName;
                        return;
                    }
                };

                dialogWindow.OnClose += () =>
                {
                    dialog.OnClosed();
                };

                dialog.Canceled += () =>
                 {
                     dialogWindow.Close();
                 };

                m_app.Bind(dialogWindow, dialog.UI);

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
                m_app.Bind(window, BuildUI(onOpen));

                //
                // main loop
                //
                uint last = Winmm.timeGetTime();
                var MS_PER_FRAME = 30;
                while (true)
                {
                    bool quit;
                    MessageLoop.ProcessMessage(out quit);
                    if (quit)
                    {
                        break;
                    }

                    var now = Winmm.timeGetTime();
                    var delta = (int)(now - last);
                    if (delta > MS_PER_FRAME)
                    {
                        last = now;
                        m_app.Draw(); // 描画
                    }
                    else
                    {
                        var sleep = MS_PER_FRAME - delta;
                        if (sleep > 0)
                        {
                            Thread.Sleep(sleep);
                        }
                    }
                }

                State.Save();
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            MessageLoop.EnsureContext();
            using (var man = new Manager())
            {
                man.Run();
            }
        }
    }
}
