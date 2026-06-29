using System;

namespace BubbleShooter.Enums
{
    /// <summary>
    /// Represents the possible states of the game.
    /// Used by GameController to manage state transitions.
    /// </summary>
    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        GameOver
    }
}
