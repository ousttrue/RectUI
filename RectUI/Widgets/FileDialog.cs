using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace RectUI.Widgets
{
    public class FileDialog
    {
        public RectRegion UI;

        public event Action<FileSystemInfo> FileChanged;
        public event Action Canceled;

        public class AwaiterContext
        {
            public Awaiter Awaiter;
            public Action Complete;
            public Action<FileInfo> SetResult;
        }
        AwaiterContext m_context;
        DirSource m_source;

        /// <summary>
        /// Call when dialog window is closed
        /// </summary>
        public void OnClosed()
        {
            var context = m_context;
            m_context = null;
            context.Complete();
        }

        void Enter(FileSystemInfo obj)
        {
            var d = obj as DirectoryInfo;
            if (d != null)
            {
                m_source.Current = d;
            }
            FileChanged?.Invoke(obj);
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

            m_source = new DirSource(Path.GetDirectoryName(openDir));

            var list = new ListRegion<FileSystemInfo>(m_source);

            list.ItemLeftDoubleClicked += (i, obj) =>
            {
                Enter(obj);
            };

            var current = new TextRegion();

            m_source.Updated += () =>
            {
                current.Label = m_source.CurrentFullName;
            };

            UI = new VBoxRegion()
            {
                new HBoxRegion(new Rect(200, 40))
                {
                    new ButtonRegion
                    {
                        Label = "Up",
                        Action = _ =>
                        {
                            m_source.GoUp();
                        },
                        Rect= new Rect(40, 40)
                    },

                    {BoxItem.Expand, current }
                },

                {BoxItem.Expand, list },

                new HBoxRegion(new Rect(200, 40))
                {
                    new ButtonRegion
                    {
                        Label = "Open",
                        Action = _ => {
                            if (list.SelectedSourceIndex != null)
                            {
                                Enter(list.Selected);
                            }
                        },
                        Rect = new Rect(96, 24),
                    },

                    new ButtonRegion
                    {
                        Label = "Cancel",
                        Action = _ => Canceled?.Invoke(),
                        Rect = new Rect(96, 24),
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
                        //awaiter.m_continuation();
                        SynchronizationContext.Current.Post(_ => awaiter.m_continuation(), null);
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
