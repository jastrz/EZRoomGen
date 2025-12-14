namespace EZRoomGen.Generation
{

    /// <summary>
    /// Interface for grid layout generators that produce 2D height maps.
    /// </summary>
    public interface ILayoutGenerator
    {
        /// <summary>
        /// Generates a layout for the room and returns it as a 2D float array.
        /// </summary>
        /// <param name="width">Width of the layout grid.</param>
        /// <param name="height">Height of the layout grid.</param>
        /// <returns>2D array where [x,y] contains height values (0.0 for walls, 1.0 for floors).</returns>
        float[,] Generate(int width, int height);
    }

}
