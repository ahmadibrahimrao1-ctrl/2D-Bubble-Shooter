using System.Drawing;
using BubbleShooter.GameObjects;

namespace BubbleShooter.Interfaces
{
    /// <summary>
    /// Interface defining a contract for objects that can participate in collision detection.
    /// Demonstrates the Interface Segregation principle of OOP.
    /// </summary>
    public interface ICollidable
    {
        RectangleF Bounds { get; }
        bool CheckCollision(GameObject other);
    }
}
