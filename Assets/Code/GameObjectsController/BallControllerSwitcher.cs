using System.Collections.Generic;

namespace Code
{
    public class BallControllerSwitcher
    {
        public List<(IGameObject obj, float percents)> OnPlayerShot(Bubble obj, float angle)
        {
            return SimpleBallController.OnPlayerShot(obj, angle);
        }

        public List<(IGameObject obj, float percents)> OnPlayerShot(UniversalBubble obj, float angle)
        {
            return MagicBallController.OnPlayerShot(obj, angle);
        }
    }
}