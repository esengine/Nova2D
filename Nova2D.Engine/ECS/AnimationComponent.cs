using System;
using System.Collections.Generic;
using System.Numerics;
using Nova2D.Engine.Graphics;

namespace Nova2D.Engine.ECS
{
    /// <summary>
    /// Stores frame animation data and playback state for an entity.
    /// Designed to work with SpriteComponent and SpriteBatch2D.
    /// </summary>
    public class AnimationComponent
    {
        /// <summary>
        /// List of source rectangles (in pixels) representing each animation frame.
        /// </summary>
        public List<Rectangle> Frames { get; } = new();

        /// <summary>
        /// Duration of each frame in seconds.
        /// </summary>
        public float FrameTime { get; set; } = 0.1f;

        /// <summary>
        /// Whether the animation should loop.
        /// </summary>
        public bool Loop { get; set; } = true;

        /// <summary>
        /// Whether the animation has finished (only applies if not looping).
        /// </summary>
        public bool Finished { get; private set; } = false;

        private int _currentFrame = 0;
        private float _timer = 0f;

        /// <summary>
        /// Gets the current frame rectangle to be used for rendering.
        /// </summary>
        public Rectangle GetCurrentFrame()
        {
            return Frames.Count > 0 ? Frames[_currentFrame] : default;
        }

        /// <summary>
        /// Resets the animation to the first frame.
        /// </summary>
        public void Reset()
        {
            _currentFrame = 0;
            _timer = 0f;
            Finished = false;
        }

        /// <summary>
        /// Updates the animation based on delta time.
        /// </summary>
        public void Update(float deltaTime)
        {
            if (Finished || Frames.Count == 0)
                return;

            _timer += deltaTime;
            if (_timer >= FrameTime)
            {
                _timer -= FrameTime;
                _currentFrame++;

                if (_currentFrame >= Frames.Count)
                {
                    if (Loop)
                    {
                        _currentFrame = 0;
                    }
                    else
                    {
                        _currentFrame = Frames.Count - 1;
                        Finished = true;
                    }
                }
            }
        }
    }
}
