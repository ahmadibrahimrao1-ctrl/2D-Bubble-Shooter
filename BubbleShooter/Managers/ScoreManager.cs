using System;

namespace BubbleShooter.Managers
{
    public class ScoreManager
    {
        private int _score;
        private int _highScore;

        // ===== Public Properties =====

        
        public int Score
        {
            get { return _score; }
        }

        
        public int HighScore
        {
            get { return _highScore; }
        }

        private const int PointsPerBubble = 10;

        private const int DropBonusPerBubble = 15;

        public ScoreManager()
        {
            _score = 0;
            _highScore = 0;
        }

        
        public void AddClusterPoints(int bubbleCount)
        {
            int points = PointsPerBubble * bubbleCount;
            if (bubbleCount > 3)
            {
                points += (bubbleCount - 3) * 5;
            }
            AddPoints(points);
        }

        public void AddDropBonus(int bubbleCount)
        {
            AddPoints(DropBonusPerBubble * bubbleCount);
        }

        public void AddPoints(int points)
        {
            _score += points;
            if (_score > _highScore)
            {
                _highScore = _score;
            }
        }

        public void Reset()
        {
            _score = 0;
        }
    }
}
