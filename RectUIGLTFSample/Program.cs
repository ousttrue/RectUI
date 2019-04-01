using DesktopDll;
using RectUI.Application;
using RectUI.Assets;
using RectUI.Widgets;
using System;
using System.IO;
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

                Console.WriteLine($"open: {path}");
                var source = await Task.Run(() => AssetSource.Load(path));
                Console.WriteLine($"loaded: {source}");
                var asset = await Task.Run(() => AssetContext.Load(source));
                Console.WriteLine($"build: {source}");
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
                    dialog.Close();
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

                Window.MessageLoop(() =>
                {
                    m_app.Draw();
                });
            

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
