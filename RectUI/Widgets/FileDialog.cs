using System;
using System.IO;
using System.Runtime.CompilerServices;


namespace RectUI.Widgets
{
    public class FileDialog
    {
        public RectRegion UI;

        public event Action<FileSystemInfo> FileChanged;

        public class AwaiterContext
        {
            public Awaiter Awaiter;
            public Action Complete;
            public Action<FileInfo> SetResult;
        }
        AwaiterContext m_context;

        /// <summary>
        /// Call when dialog window is closed
        /// </summary>
        public void Close()
        {
            var context = m_context;
            m_context = null;
            context.Complete();
        }

        public FileDialog(string openDir)
        {
            FileChanged += (obj) =>
            {
                var f = obj as FileInfo;
                if (f != null)
                {
                    m_context.SetResult(f);
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

        public event Action BeginDialogOpen;

        public Awaitable OpenAsync()
        {
            BeginDialogOpen?.Invoke();

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
}
