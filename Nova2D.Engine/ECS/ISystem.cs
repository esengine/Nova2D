namespace Nova2D.Engine.ECS
{
    /// <summary>
    /// Represents a system that processes entities each frame.
    /// </summary>
    public interface ISystem
    {
        /// <summary>
        /// Updates the system.
        /// </summary>
        /// <param name="deltaTime">Elapsed time in seconds since last update.</param>
        /// <param name="scene">Current scene containing entities.</param>
        void Update(float deltaTime, Scene scene);
    }
}