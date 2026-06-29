using System;
using System.Drawing;
using BubbleShooter.GameObjects;
using BubbleShooter.Interfaces;

namespace BubbleShooter.Managers
{
    public class CollisionManager
    {
        public bool CheckBubbleGridCollision(Bubble flying, Bubble[,] grid, int rows, int cols)
        {
            if (flying == null || !flying.IsFlying) return false;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Bubble gridBubble = grid[r, c];
                    if (gridBubble != null && gridBubble.IsActive)
                    {
                        if (flying.CheckCollision(gridBubble))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool CheckCeilingCollision(Bubble flying, float ceilingY)
        {
            if (flying == null || !flying.IsFlying) return false;
            return flying.Position.Y <= ceilingY;
        }
    }
}
