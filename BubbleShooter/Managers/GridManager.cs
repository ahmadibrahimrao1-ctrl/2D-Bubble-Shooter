using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BubbleShooter.Enums;
using BubbleShooter.GameObjects;

namespace BubbleShooter.Managers
{
    public class GridManager
    {
        // ===== Constants =====
        public const int MaxRows = 20;
        public const int MaxCols = 15;
        public const float OffsetX = 0f;     // Left margin of the grid
        public const float OffsetY = 0f;     // Top margin of the grid
        private const int MinClusterSize = 3; // Minimum bubbles to pop

        // ===== Private fields =====
        private Bubble[,] _grid;
        private Random _random;

        // ===== Public Properties =====

        public Bubble[,] Grid
        {
            get { return _grid; }
        }

        public int Rows { get { return MaxRows; } }

        public int Cols { get { return MaxCols; } }

        // ===== Constructor =====

        
        public GridManager()
        {
            _grid = new Bubble[MaxRows, MaxCols];
            _random = new Random();
        }

        public void InitializeGrid(int preFilledRows, float boardWidth)
        {
            _grid = new Bubble[MaxRows, MaxCols];
            BubbleColor[] colors = (BubbleColor[])Enum.GetValues(typeof(BubbleColor));

            for (int row = 0; row < preFilledRows; row++)
            {
                int colsInRow = (row % 2 == 0) ? MaxCols : MaxCols - 1;
                for (int col = 0; col < colsInRow; col++)
                {
                    BubbleColor color = colors[_random.Next(colors.Length)];
                    PointF center = GetCellCenter(row, col);
                    Bubble bubble = new Bubble(
                        center.X - Bubble.Radius,
                        center.Y - Bubble.Radius,
                        color);
                    bubble.GridRow = row;
                    bubble.GridCol = col;
                    bubble.BoardWidth = boardWidth;
                    _grid[row, col] = bubble;
                }
            }
        }

        public PointF GetCellCenter(int row, int col)
        {
            float x;
            if (row % 2 == 0)
            {
                // Even row: bubbles start at left edge
                x = OffsetX + col * Bubble.Diameter + Bubble.Radius;
            }
            else
            {
                // Odd row: offset right by half a bubble diameter
                x = OffsetX + col * Bubble.Diameter + Bubble.Diameter;
            }

            // Rows are spaced vertically by ~0.866 * Diameter (hex packing)
            float y = OffsetY + row * (Bubble.Diameter * 0.866f) + Bubble.Radius;

            return new PointF(x, y);
        }

        /// <summary>
        /// Finds the nearest empty grid cell to the given screen position.
        /// Used to snap a flying bubble into the grid after collision.
        /// </summary>
        /// <param name="position">The screen position of the bubble center.</param>
        /// <returns>A tuple (row, col) of the nearest empty cell, or (-1,-1) if none found.</returns>
        public (int row, int col) GetNearestEmptyCell(PointF position)
        {
            float minDist = float.MaxValue;
            int bestRow = -1, bestCol = -1;

            for (int row = 0; row < MaxRows; row++)
            {
                int colsInRow = (row % 2 == 0) ? MaxCols : MaxCols - 1;
                for (int col = 0; col < colsInRow; col++)
                {
                    if (_grid[row, col] != null) continue; // Skip occupied cells

                    PointF cellCenter = GetCellCenter(row, col);
                    float dx = position.X - cellCenter.X;
                    float dy = position.Y - cellCenter.Y;
                    float dist = dx * dx + dy * dy; // Squared distance (no need for sqrt)

                    if (dist < minDist)
                    {
                        minDist = dist;
                        bestRow = row;
                        bestCol = col;
                    }
                }
            }

            return (bestRow, bestCol);
        }

        // ===== Bubble Placement =====

        /// <summary>
        /// Places a bubble into the grid at the specified cell.
        /// </summary>
        /// <param name="bubble">The bubble to snap into the grid.</param>
        /// <param name="row">Target row.</param>
        /// <param name="col">Target column.</param>
        public void SnapBubble(Bubble bubble, int row, int col)
        {
            if (row < 0 || row >= MaxRows || col < 0) return;
            int colsInRow = (row % 2 == 0) ? MaxCols : MaxCols - 1;
            if (col >= colsInRow) return;

            PointF center = GetCellCenter(row, col);
            bubble.Position = new PointF(center.X - Bubble.Radius, center.Y - Bubble.Radius);
            bubble.GridRow = row;
            bubble.GridCol = col;
            bubble.IsFlying = false;
            bubble.Velocity = new PointF(0, 0);
            _grid[row, col] = bubble;
        }

        // ===== Cluster Matching (Recursive Flood-Fill) =====

        /// <summary>
        /// Recursively finds all connected bubbles of the same color starting from (row, col).
        /// Uses depth-first flood-fill traversal of hex neighbors.
        /// </summary>
        /// <param name="row">Starting row.</param>
        /// <param name="col">Starting column.</param>
        /// <returns>List of matching connected bubbles.</returns>
        public List<Bubble> FindMatchingCluster(int row, int col)
        {
            List<Bubble> cluster = new List<Bubble>();
            if (_grid[row, col] == null) return cluster;

            BubbleColor targetColor = _grid[row, col].BubbleColor;
            bool[,] visited = new bool[MaxRows, MaxCols];

            // Start recursive flood-fill
            FloodFill(row, col, targetColor, visited, cluster);

            return cluster;
        }

        /// <summary>
        /// Recursive flood-fill helper that traverses hex neighbors to find same-color clusters.
        /// </summary>
        private void FloodFill(int row, int col, BubbleColor targetColor, bool[,] visited, List<Bubble> cluster)
        {
            // Boundary and validity checks
            if (row < 0 || row >= MaxRows || col < 0) return;
            int colsInRow = (row % 2 == 0) ? MaxCols : MaxCols - 1;
            if (col >= colsInRow) return;
            if (visited[row, col]) return;
            if (_grid[row, col] == null || !_grid[row, col].IsActive) return;
            if (_grid[row, col].BubbleColor != targetColor) return;

            // Mark as visited and add to cluster
            visited[row, col] = true;
            cluster.Add(_grid[row, col]);

            // Recurse into all 6 hex neighbors
            List<(int r, int c)> neighbors = GetNeighbors(row, col);
            foreach (var (r, c) in neighbors)
            {
                FloodFill(r, c, targetColor, visited, cluster);
            }
        }

        /// <summary>
        /// Removes a cluster of bubbles from the grid and marks them inactive.
        /// </summary>
        /// <param name="cluster">The list of bubbles to pop.</param>
        public void PopCluster(List<Bubble> cluster)
        {
            foreach (Bubble bubble in cluster)
            {
                if (bubble.GridRow >= 0 && bubble.GridCol >= 0)
                {
                    _grid[bubble.GridRow, bubble.GridCol] = null;
                }
                bubble.IsActive = false;
            }
        }

        // ===== Disconnected Bubble Detection (BFS from Ceiling) =====

        /// <summary>
        /// Finds all bubbles that are not connected to the ceiling (row 0) via other bubbles.
        /// Uses BFS starting from all ceiling bubbles to find the connected set,
        /// then returns everything NOT in that set.
        /// </summary>
        /// <returns>List of disconnected (floating) bubbles.</returns>
        public List<Bubble> FindDisconnectedBubbles()
        {
            bool[,] connected = new bool[MaxRows, MaxCols];
            Queue<(int row, int col)> queue = new Queue<(int, int)>();

            // Seed BFS with all bubbles in row 0 (ceiling row)
            int colsInFirstRow = MaxCols; // row 0 is even
            for (int col = 0; col < colsInFirstRow; col++)
            {
                if (_grid[0, col] != null && _grid[0, col].IsActive)
                {
                    connected[0, col] = true;
                    queue.Enqueue((0, col));
                }
            }

            // BFS traversal
            while (queue.Count > 0)
            {
                var (row, col) = queue.Dequeue();
                List<(int r, int c)> neighbors = GetNeighbors(row, col);

                foreach (var (r, c) in neighbors)
                {
                    if (r >= 0 && r < MaxRows && c >= 0)
                    {
                        int colsInR = (r % 2 == 0) ? MaxCols : MaxCols - 1;
                        if (c < colsInR && !connected[r, c] &&
                            _grid[r, c] != null && _grid[r, c].IsActive)
                        {
                            connected[r, c] = true;
                            queue.Enqueue((r, c));
                        }
                    }
                }
            }

            // Collect all grid bubbles NOT in the connected set
            List<Bubble> disconnected = new List<Bubble>();
            for (int r = 0; r < MaxRows; r++)
            {
                int colsInR = (r % 2 == 0) ? MaxCols : MaxCols - 1;
                for (int c = 0; c < colsInR; c++)
                {
                    if (_grid[r, c] != null && _grid[r, c].IsActive && !connected[r, c])
                    {
                        disconnected.Add(_grid[r, c]);
                    }
                }
            }

            return disconnected;
        }

        /// <summary>
        /// Removes disconnected bubbles from the grid.
        /// </summary>
        /// <param name="disconnected">List of floating bubbles to drop.</param>
        public void DropDisconnected(List<Bubble> disconnected)
        {
            foreach (Bubble bubble in disconnected)
            {
                if (bubble.GridRow >= 0 && bubble.GridCol >= 0)
                {
                    _grid[bubble.GridRow, bubble.GridCol] = null;
                }
                bubble.IsActive = false;
            }
        }

        // ===== Game Over Check =====

        /// <summary>
        /// Checks if any bubble in the grid has crossed below the boundary Y coordinate.
        /// </summary>
        /// <param name="boundaryY">The Y coordinate of the game-over boundary line.</param>
        /// <returns>True if the game should end.</returns>
        public bool IsGameOver(float boundaryY)
        {
            for (int r = 0; r < MaxRows; r++)
            {
                int colsInR = (r % 2 == 0) ? MaxCols : MaxCols - 1;
                for (int c = 0; c < colsInR; c++)
                {
                    if (_grid[r, c] != null && _grid[r, c].IsActive)
                    {
                        if (_grid[r, c].Position.Y + Bubble.Diameter >= boundaryY)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        // ===== Utility Methods =====

        /// <summary>
        /// Returns all active bubbles currently in the grid as a flat list.
        /// Used by GameController to add them to the unified gameObjects list.
        /// </summary>
        /// <returns>List of all non-null, active grid bubbles.</returns>
        public List<Bubble> GetAllActiveBubbles()
        {
            List<Bubble> bubbles = new List<Bubble>();
            for (int r = 0; r < MaxRows; r++)
            {
                int colsInR = (r % 2 == 0) ? MaxCols : MaxCols - 1;
                for (int c = 0; c < colsInR; c++)
                {
                    if (_grid[r, c] != null && _grid[r, c].IsActive)
                    {
                        bubbles.Add(_grid[r, c]);
                    }
                }
            }
            return bubbles;
        }

        /// <summary>
        /// Gets a random BubbleColor from the colors currently present in the grid.
        /// Falls back to a fully random color if the grid is empty.
        /// </summary>
        /// <returns>A BubbleColor value.</returns>
        public BubbleColor GetRandomColorFromGrid()
        {
            List<BubbleColor> activeColors = new List<BubbleColor>();
            for (int r = 0; r < MaxRows; r++)
            {
                int colsInR = (r % 2 == 0) ? MaxCols : MaxCols - 1;
                for (int c = 0; c < colsInR; c++)
                {
                    if (_grid[r, c] != null && _grid[r, c].IsActive)
                    {
                        if (!activeColors.Contains(_grid[r, c].BubbleColor))
                        {
                            activeColors.Add(_grid[r, c].BubbleColor);
                        }
                    }
                }
            }

            if (activeColors.Count > 0)
            {
                return activeColors[_random.Next(activeColors.Count)];
            }

            // Fallback: random from all colors
            BubbleColor[] allColors = (BubbleColor[])Enum.GetValues(typeof(BubbleColor));
            return allColors[_random.Next(allColors.Length)];
        }

        // ===== Hex Grid Neighbor Calculation =====

        /// <summary>
        /// Returns the 6 hex neighbors of the cell at (row, col).
        /// Accounts for even/odd row offset in the hex grid layout.
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <param name="col">Column index.</param>
        /// <returns>List of valid neighbor (row, col) tuples.</returns>
        public List<(int r, int c)> GetNeighbors(int row, int col)
        {
            List<(int r, int c)> neighbors = new List<(int, int)>();

            // Hex grid neighbor offsets differ for even and odd rows
            int[][] offsets;
            if (row % 2 == 0)
            {
                // Even row offsets
                offsets = new int[][]
                {
                    new int[] { -1, -1 }, // Top-left
                    new int[] { -1,  0 }, // Top-right
                    new int[] {  0, -1 }, // Left
                    new int[] {  0,  1 }, // Right
                    new int[] {  1, -1 }, // Bottom-left
                    new int[] {  1,  0 }  // Bottom-right
                };
            }
            else
            {
                // Odd row offsets
                offsets = new int[][]
                {
                    new int[] { -1,  0 }, // Top-left
                    new int[] { -1,  1 }, // Top-right
                    new int[] {  0, -1 }, // Left
                    new int[] {  0,  1 }, // Right
                    new int[] {  1,  0 }, // Bottom-left
                    new int[] {  1,  1 }  // Bottom-right
                };
            }

            foreach (int[] offset in offsets)
            {
                int nr = row + offset[0];
                int nc = col + offset[1];

                // Validate bounds
                if (nr >= 0 && nr < MaxRows && nc >= 0)
                {
                    int colsInRow = (nr % 2 == 0) ? MaxCols : MaxCols - 1;
                    if (nc < colsInRow)
                    {
                        neighbors.Add((nr, nc));
                    }
                }
            }

            return neighbors;
        }

        /// <summary>
        /// Draws grid debug overlay lines (optional, can be enabled for debugging).
        /// </summary>
        /// <param name="g">The Graphics surface.</param>
        public void DrawGrid(Graphics g)
        {
            // Draw all active bubbles in the grid
            for (int r = 0; r < MaxRows; r++)
            {
                int colsInR = (r % 2 == 0) ? MaxCols : MaxCols - 1;
                for (int c = 0; c < colsInR; c++)
                {
                    if (_grid[r, c] != null && _grid[r, c].IsActive)
                    {
                        _grid[r, c].Draw(g);
                    }
                }
            }
        }
    }
}
