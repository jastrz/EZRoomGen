namespace EZRoomGen.Generation
{
    /// <summary>
    /// Settings used for Maze layout generation.
    /// </summary>
    public class MazeLayoutLayoutGeneratorSettings : LayoutGeneratorSettings
    {
        public int loopCount = 5;
        public float deadEndKeepChance = 0.3f;
        public bool smoothEdges = false;

    }
}