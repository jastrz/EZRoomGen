namespace EZRoomGen.Generation.Utils
{
    /// <summary>
    /// Utility class with common methods used for procedural room layout generation.
    /// </summary>
    public static class LayoutGenerationUtils
    {
        /// <summary>
        /// Smooths the edges of a grid by converting isolated cells near floor regions into floor,
        /// based on their neighboring cells. Useful for reducing jagged floor edges in procedural layouts.
        /// </summary>
        /// <param name="grid">The 2D array representing the layout grid.</param>
        /// <param name="width">Width of the grid.</param>
        /// <param name="height">Height of the grid.</param>
        /// <param name="defaultHeight">The value to set for new floor cells.</param>
        public static void SmoothEdges(float[,] grid, int width, int height, float defaultHeight)
        {
            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    if (grid[x, y] < 0.5f)
                    {
                        int floorCount = 0;
                        if (grid[x - 1, y] > 0.5f) floorCount++;
                        if (grid[x + 1, y] > 0.5f) floorCount++;
                        if (grid[x, y - 1] > 0.5f) floorCount++;
                        if (grid[x, y + 1] > 0.5f) floorCount++;

                        if (floorCount >= 3)
                        {
                            grid[x, y] = defaultHeight;
                        }
                    }
                }
            }
        }
    }
}