namespace Nova2D.Engine.ECS
{
    /// <summary>
    /// Updates all entities with AnimationComponent and SpriteComponent.
    /// Applies current animation frame to the sprite's source rectangle.
    /// </summary>
    public class AnimationSystem : ISystem
    {
        public void Update(float deltaTime, Scene scene)
        {
            foreach (var entity in scene.Query<AnimationComponent, SpriteComponent>())
            {
                var animation = entity.Get<AnimationComponent>()!;
                var sprite = entity.Get<SpriteComponent>()!;

                animation.Update(deltaTime);
                sprite.SourceRect = animation.GetCurrentFrame();
            }
        }
    }
}