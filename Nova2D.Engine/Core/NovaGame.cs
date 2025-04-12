using Nova2D.Engine.ECS;
using Nova2D.Engine.Graphics;
using Nova2D.Engine.UI;

namespace Nova2D.Engine.Core
{
    /// <summary>
    /// Base class for Nova2D games. Inherit and override lifecycle methods to define your game logic.
    /// </summary>
    public abstract class NovaGame
    {
        /// <summary>
        /// Called once after OpenGL and context initialization.
        /// </summary>
        public virtual void OnLoad() { }

        /// <summary>
        /// Called every frame with delta time in seconds.
        /// </summary>
        public virtual void OnUpdate(float deltaTime) { }

        /// <summary>
        /// Called before shutdown, use for resource cleanup.
        /// </summary>
        public virtual void OnClose() { }

        /// <summary>
        /// Adds a game entity to the current scene.
        /// </summary>
        protected void AddEntity(Entity entity) => NovaContext.Scene?.AddEntity(entity);

        /// <summary>
        /// Adds a UI element to the UI canvas.
        /// </summary>
        protected void AddUI(UIElement uiElement) => NovaContext.UICanvas?.Add(uiElement);

        /// <summary>
        /// Loads a font and registers it under a key.
        /// </summary>
        protected void LoadFont(string key, string fntPath)
        {
            var fontTexture = new Texture(NovaContext.GL!, fntPath.Replace(".fnt", "_0.png"));
            var font = new BitmapFont(fontTexture, fntPath);
            NovaContext.Fonts[key] = font;
        }

        /// <summary>
        /// Gets a previously loaded font by key.
        /// </summary>
        protected BitmapFont Font(string key) => NovaContext.Fonts[key];

        /// <summary>
        /// Loads a texture and registers it under a key.
        /// </summary>
        protected void LoadTexture(string key, string path)
        {
            var tex = new Texture(NovaContext.GL!, path);
            NovaContext.Textures[key] = tex;
        }

        /// <summary>
        /// Gets a previously loaded texture by key.
        /// </summary>
        protected Texture Texture(string key) => NovaContext.Textures[key];
    }
}
