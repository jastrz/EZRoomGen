
using System.Collections.Generic;
using EZRoomGen.Generation.Utils;

namespace EZRoomGen.Generation
{
    /// <summary>
    /// Generates dungeon layouts using Cellular Automata.
    /// Returns a 2D float grid where values represent floor heights (0 = wall, >0 = floor).
    /// </summary>
    public class DungeonLayoutGenerator : ILayoutGenerator
    {
        private DungeonLayoutGeneratorSettings _settings;
        private System.Random _random;

        public DungeonLayoutGenerator(DungeonLayoutGeneratorSettings settings = null)
        {
            _settings = settings ?? new DungeonLayoutGeneratorSettings();
        }

        public float[,] Generate(int width, int height)
        {
            _random = new System.Random(_settings.seed);

            float[,] grid;

            grid = GenerateCellular(width, height, _settings.density, _settings.height);

            if (_settings.smoothEdges)
            {
                LayoutGenerationUtils.SmoothEdges(grid, width, height, _settings.height);
            }

            return grid;
        }

        private float[,] GenerateCellular(int width, int height, float density, float defaultHeight)
        {
            var grid = new float[width, height];

            // Initialize with random walls
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                    {
                        grid[x, y] = 0f; // Border walls
                    }
                    else
                    {
                        grid[x, y] = _random.NextDouble() < density ? 0f : _settings.height;
                    }
                }
            }

            // Cellular automata iterations
            for (int i = 0; i < _settings.iterations; i++)
            {
                grid = CellularStep(grid, width, height, defaultHeight);
            }

            // Ensure connectivity
            FloodFillLargestArea(grid, width, height);

            // Widen paths if needed
            if (_settings.pathWidth > 1)
            {
                grid = WidenPaths(grid, width, height);
            }

            return grid;
        }

        private float[,] CellularStep(float[,] grid, int width, int height, float defaultHeight)
        {
            var newGrid = new float[width, height];

            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    int wallCount = CountWallNeighbors(grid, x, y, width, height);

                    if (wallCount >= 5)
                        newGrid[x, y] = 0f;
                    else if (wallCount <= 3)
                        newGrid[x, y] = defaultHeight;
                    else
                        newGrid[x, y] = grid[x, y];
                }
            }

            return newGrid;
        }

        private int CountWallNeighbors(float[,] grid, int x, int y, int width, int height)
        {
            int count = 0;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    int nx = x + dx;
                    int ny = y + dy;

                    if (nx < 0 || nx >= width || ny < 0 || ny >= height || grid[nx, ny] < 0.5f)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private void FloodFillLargestArea(float[,] grid, int width, int height)
        {
            var areas = new List<HashSet<(int, int)>>();
            var visited = new bool[width, height];

            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    if (grid[x, y] > 0.5f && !visited[x, y])
                    {
                        var area = FloodFill(grid, x, y, width, height, visited);
                        areas.Add(area);
                    }
                }
            }

            if (areas.Count == 0) return;

            var largest = areas[0];
            foreach (var area in areas)
            {
                if (area.Count > largest.Count)
                    largest = area;
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[x, y] > 0.5f && !largest.Contains((x, y)))
                    {
                        grid[x, y] = 0f;
                    }
                }
            }
        }

        private HashSet<(int, int)> FloodFill(float[,] grid, int startX, int startY, int width, int height, bool[,] visited)
        {
            var area = new HashSet<(int, int)>();
            var queue = new Queue<(int, int)>();
            queue.Enqueue((startX, startY));
            visited[startX, startY] = true;

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();
                area.Add((x, y));

                foreach (var (dx, dy) in new[] { (0, 1), (1, 0), (0, -1), (-1, 0) })
                {
                    int nx = x + dx;
                    int ny = y + dy;

                    if (nx >= 0 && nx < width && ny >= 0 && ny < height &&
                        !visited[nx, ny] && grid[nx, ny] > 0.5f)
                    {
                        visited[nx, ny] = true;
                        queue.Enqueue((nx, ny));
                    }
                }
            }

            return area;
        }

        private float[,] WidenPaths(float[,] grid, int width, int height)
        {
            var newGrid = new float[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    newGrid[x, y] = grid[x, y];
                }
            }

            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    if (grid[x, y] > 0.5f)
                    {
                        for (int dx = 0; dx < _settings.pathWidth; dx++)
                        {
                            for (int dy = 0; dy < _settings.pathWidth; dy++)
                            {
                                int nx = x + dx;
                                int ny = y + dy;
                                if (nx < width - 1 && ny < height - 1)
                                {
                                    newGrid[nx, ny] = _settings.height;
                                }
                            }
                        }
                    }
                }
            }

            return newGrid;
        }
    }
}
