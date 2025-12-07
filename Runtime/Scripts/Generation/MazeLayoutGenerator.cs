using System.Collections.Generic;
using EZRoomGen.Generation.Utils;

namespace EZRoomGen.Generation
{
    /// <summary>
    /// Generates maze layouts using Recursive Backtracking.
    /// Returns a 2D float grid where values represent floor heights (0 = wall, >0 = floor).
    /// </summary>
    public class MazeLayoutGenerator : ILayoutGenerator
    {
        private MazeLayoutLayoutGeneratorSettings _settings;
        private System.Random _random;

        public MazeLayoutGenerator(MazeLayoutLayoutGeneratorSettings settings = null)
        {
            _settings = settings ?? new MazeLayoutLayoutGeneratorSettings();
        }

        public float[,] Generate(int width, int height)
        {
            _random = new System.Random(_settings.seed);

            float[,] grid;

            grid = GenerateRecursiveBacktracker(width, height, _settings.height);

            if (_settings.loopCount > 0)
            {
                AddLoops(grid, width, height, _settings.loopCount, _settings.height);
            }

            if (_settings.deadEndKeepChance == 0)
            {
                RemoveDeadEnds(grid, width, height);
            }
            else if (_settings.deadEndKeepChance < 1f)
            {
                RemoveSomeDeadEnds(grid, width, height, _settings.deadEndKeepChance);
            }

            if (_settings.smoothEdges)
            {
                LayoutGenerationUtils.SmoothEdges(grid, width, height, _settings.height);
            }

            return grid;
        }

        private float[,] GenerateRecursiveBacktracker(int width, int height, float defaultHeight)
        {
            var grid = new float[width, height];

            // Start from center
            int startX = width / 2;
            int startY = height / 2;

            var visited = new bool[width, height];
            var stack = new Stack<(int x, int y)>();

            stack.Push((startX, startY));
            visited[startX, startY] = true;
            grid[startX, startY] = defaultHeight;

            var directions = new[] { (0, -2), (2, 0), (0, 2), (-2, 0) };

            while (stack.Count > 0)
            {
                var (x, y) = stack.Peek();
                var neighbors = new List<(int x, int y, int dx, int dy)>();

                foreach (var (dx, dy) in directions)
                {
                    int nx = x + dx;
                    int ny = y + dy;

                    if (nx > 0 && nx < width - 1 && ny > 0 && ny < height - 1 && !visited[nx, ny])
                    {
                        neighbors.Add((nx, ny, dx, dy));
                    }
                }

                if (neighbors.Count > 0)
                {
                    var chosen = neighbors[_random.Next(neighbors.Count)];
                    int nx = chosen.x;
                    int ny = chosen.y;
                    int wallX = x + chosen.dx / 2;
                    int wallY = y + chosen.dy / 2;

                    grid[wallX, wallY] = defaultHeight;
                    grid[nx, ny] = defaultHeight;
                    visited[nx, ny] = true;
                    stack.Push((nx, ny));
                }
                else
                {
                    stack.Pop();
                }
            }

            return grid;
        }

        private void AddLoops(float[,] grid, int width, int height, int loopCount, float defaultHeight)
        {
            int added = 0;
            int attempts = 0;
            int maxAttempts = loopCount * 10;

            while (added < loopCount && attempts < maxAttempts)
            {
                attempts++;
                int x = _random.Next(2, width - 2);
                int y = _random.Next(2, height - 2);

                if (grid[x, y] < 0.5f)
                {
                    int floorNeighbors = 0;
                    if (grid[x - 1, y] > 0.5f) floorNeighbors++;
                    if (grid[x + 1, y] > 0.5f) floorNeighbors++;
                    if (grid[x, y - 1] > 0.5f) floorNeighbors++;
                    if (grid[x, y + 1] > 0.5f) floorNeighbors++;

                    if (floorNeighbors >= 2)
                    {
                        grid[x, y] = defaultHeight;
                        added++;
                    }
                }
            }
        }

        private bool IsDeadEnd(float[,] grid, int x, int y)
        {
            int openSides = 0;
            if (grid[x - 1, y] > 0.5f) openSides++;
            if (grid[x + 1, y] > 0.5f) openSides++;
            if (grid[x, y - 1] > 0.5f) openSides++;
            if (grid[x, y + 1] > 0.5f) openSides++;
            return openSides == 1;
        }

        private void RemoveDeadEnds(float[,] grid, int width, int height)
        {
            bool changed = true;
            while (changed)
            {
                changed = false;
                for (int x = 1; x < width - 1; x++)
                {
                    for (int y = 1; y < height - 1; y++)
                    {
                        if (grid[x, y] > 0.5f && IsDeadEnd(grid, x, y))
                        {
                            grid[x, y] = 0f;
                            changed = true;
                        }
                    }
                }
            }
        }

        private void RemoveSomeDeadEnds(float[,] grid, int width, int height, float chance)
        {
            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    if (grid[x, y] > 0.5f && IsDeadEnd(grid, x, y))
                    {
                        if (_random.NextDouble() > chance)
                        {
                            grid[x, y] = 0f;
                        }
                    }
                }
            }
        }
    }
}