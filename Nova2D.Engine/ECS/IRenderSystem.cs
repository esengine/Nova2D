namespace Nova2D.Engine.ECS
{
    /// <summary>
    /// Represents a system that performs rendering logic each frame.
    /// </summary>
    public interface IRenderSystem
    {
        /// <summary>
        /// Called every frame to render entities or visuals.
        /// </summary>
        /// <param name="scene">The scene being rendered.</param>
        void Render(Scene scene);
    }
}