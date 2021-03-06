﻿using Reactive.Bindings;
using RectUI.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace RectUI.Assets
{
    enum ShaderType
    {
        Gizmo,
        Unlit,
        Standard,
        Screen,
    }

    class SourceWatcher
    {
        FileSystemWatcher m_watcher;
        public SourceWatcher(string dir, string file)
        {
            m_watcher = new FileSystemWatcher();
            m_watcher.Path = dir;
            m_watcher.NotifyFilter = NotifyFilters.LastWrite;
            m_watcher.Filter = file;
            m_watcher.Changed += Watcher_Changed;
            m_watcher.EnableRaisingEvents = true;

            var path = Path.Combine(dir, file);
            if (File.Exists(path))
            {
                Source.Value = File.ReadAllText(path, Encoding.UTF8);
            }
        }

        private async void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            await Task.Delay(100);
            Source.Value = File.ReadAllText(e.FullPath, Encoding.UTF8);
        }

        public ReactiveProperty<string> Source = new ReactiveProperty<string>();
    }

    class ShaderLoader
    {
        Dictionary<ShaderType, SourceWatcher> m_map = new Dictionary<ShaderType, SourceWatcher>();

        public static ShaderLoader Instance = new ShaderLoader();

        ShaderLoader()
        {
            var shaderDir = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "../../../../RectUI/shaders");
            m_map.Add(ShaderType.Unlit, new SourceWatcher(shaderDir, "unlit.hlsl"));
            m_map.Add(ShaderType.Standard, new SourceWatcher(shaderDir, "unlit.hlsl"));
            m_map.Add(ShaderType.Gizmo, new SourceWatcher(shaderDir, "gizmo.hlsl"));
            m_map.Add(ShaderType.Screen, new SourceWatcher(shaderDir, "screen.hlsl"));
        }

        public IObservable<string> GetShaderSource(ShaderType type)
        {
            return m_map[type].Source;
        }

        public D3D11Shader CreateShader(ShaderType type)
        {
            var source = GetShaderSource(type);
            var shader = new D3D11Shader(type.ToString());

            source
                .Subscribe(x =>
                {
                    shader.SetShader(x, x);
                });

            return shader;
        }
    }
}
