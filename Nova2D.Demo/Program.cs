﻿using System.Numerics;
using Nova2D.Engine.ECS;
using Nova2D.Engine.Graphics;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Shader = Nova2D.Engine.Graphics.Shader;
using Texture = Nova2D.Engine.Graphics.Texture;

namespace Nova2D.Demo
{
    internal static class Program
    {
        private static IWindow _window;
        private static GL _gl;

        private static Shader? _shader;
        private static Texture? _texture;
        private static SpriteRenderer? _renderer;
        private static Camera2D? _camera;

        private static BitmapFont? _bitmapFont;
        private static BitmapFontRenderer? _fontRenderer;

        private static Shader? _batchShader;
        private static SpriteBatch2D? _spriteBatch;

        private static Scene? _scene;
        private static Entity? _rotatingEntity;

        private static int _frameCounter;
        private static float _timeAccumulator;
        private static float _fps;


        static void Main()
        {
            var options = WindowOptions.Default;
            options.Size = new Silk.NET.Maths.Vector2D<int>(800, 600);
            options.Title = "Nova2D Demo";

            _window = Window.Create(options);
            _window.Load += OnLoad;
            _window.Render += OnRender;
            _window.Update += OnUpdate;
            _window.Closing += OnClose;

            _window.FramebufferResize += size =>
            {
                _gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);
                _camera?.Resize(size.X, size.Y);
            };

            _window.Run();
        }

        private static void OnLoad()
        {
            _gl = GL.GetApi(_window);
            _gl.Enable(GLEnum.Blend);
            _gl.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);

            _texture = new Texture(_gl, Path.Combine("Assets", "Textures", "test.png"));
            // SpriteRenderer (still usable if needed)
            _shader = new Shader(_gl, "Shaders/sprite.vert", "Shaders/sprite.frag");
            _renderer = new SpriteRenderer(_gl, _shader);

            // SpriteBatch2D
            _batchShader = new Shader(_gl, "Shaders/spritebatch.vert", "Shaders/spritebatch.frag");
            _spriteBatch = new SpriteBatch2D(_gl, _batchShader);

            // Load font
            var fontTexture = new Texture(_gl, Path.Combine("Assets", "Fonts", "font_0.png"));
            _bitmapFont = new BitmapFont(fontTexture, Path.Combine("Assets", "Fonts", "font.fnt"));
            _fontRenderer = new BitmapFontRenderer(_bitmapFont);

            _camera = new Camera2D(_window.Size.X, _window.Size.Y);
            _scene = new Scene();

            // Register rendering system
            // _scene.AddSystem(new SpriteRenderSystem(_renderer, _camera));
            // _scene.AddSystem(new SpriteBatchRenderSystem(_spriteBatch, _camera));
            _scene.AddSystem(new SmartSpriteBatchRenderSystem(_spriteBatch, _camera));
            _scene.AddSystem(new AnimationSystem());

            // Create entity
            _rotatingEntity = new Entity();
            _rotatingEntity.Add(new TransformComponent { Position = new Vector2(400, 300) });
            _rotatingEntity.Add(new SpriteComponent(_texture)
            {
                Size = new Vector2(128, 128),
                Color = Vector4.One
            });

            _scene.AddEntity(_rotatingEntity);

            var slimeTexture =
                new Texture(_gl, Path.Combine("Assets", "Textures", "slime spritesheet calciumtrice.png"));

            var slimeAnimation = new AnimationComponent
            {
                FrameTime = 0.1f,
                Loop = true
            };

            for (int i = 0; i < 4; i++)
            {
                slimeAnimation.Frames.Add(new Rectangle(i * 32, 0, 32, 32));
            }

            var slimeEntity = new Entity();
            slimeEntity.Add(new TransformComponent { Position = new Vector2(300, 300) });
            slimeEntity.Add(new SpriteComponent(slimeTexture)
            {
                Size = new Vector2(32, 32),
                Color = Vector4.One
            });
            slimeEntity.Add(slimeAnimation);

            _scene.AddEntity(slimeEntity);
        }

        private static void OnUpdate(double delta)
        {
            if (_rotatingEntity?.Get<TransformComponent>() is { } transform)
            {
                transform.Rotation += (float)delta;
            }

            _scene?.Update((float)delta);

            _timeAccumulator += (float)delta;
            _frameCounter++;

            if (_timeAccumulator >= 1.0f)
            {
                _fps = _frameCounter / _timeAccumulator;
                _frameCounter = 0;
                _timeAccumulator = 0;
            }
        }

        private static void OnRender(double delta)
        {
            _gl.ClearColor(0.1f, 0.1f, 0.1f, 1f);
            _gl.Clear(ClearBufferMask.ColorBufferBit);

            SpriteBatch2D.TotalDrawCallsThisFrame = 0;

            _scene?.Render();

            if (_spriteBatch != null && _fontRenderer != null)
            {
                _spriteBatch.Begin(_camera!.GetMatrix());

                _fontRenderer.DrawText(_spriteBatch, "Hello Nova2D!", new Vector2(20, 20), Vector4.One);

                string drawCallText = $"Draw Calls: {SpriteBatch2D.TotalDrawCallsThisFrame}";
                _fontRenderer.DrawText(_spriteBatch, drawCallText, new Vector2(20, 50), Vector4.One);
                _fontRenderer.DrawText(_spriteBatch, $"FPS: {_fps:F0}", new Vector2(20, 80), Vector4.One);

                _spriteBatch.End(_bitmapFont!.Texture);
            }
        }

        private static void OnClose()
        {
            _texture?.Dispose();
            _shader?.Dispose();
            _batchShader?.Dispose();
            _spriteBatch?.Dispose();
        }
    }
}