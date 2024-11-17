namespace DirigibleBattle.Managers
{
    public class WindManager
    {
        private NetworkManager networkManager;
        private GameManager gameManager;

        private bool isFirstPlayerWindLeft = false;
        private bool isSecondPlayerWindLeft = false;

        public void SetManagers(NetworkManager networkManager, GameManager gameManager)
        {
            this.networkManager = networkManager;
            this.gameManager = gameManager;
        }

        public void WindDirection()
        {
            if ((gameManager.FirstPlayer.GetCollider().X <= ColliderManager.screenBorderCollider.X) && !isFirstPlayerWindLeft)
            {
                gameManager.FirstPlayer.ChangeWindDirection(false);
            }
            else if ((gameManager.FirstPlayer.GetCollider().X + gameManager.FirstPlayer.GetCollider().Width >= ColliderManager.screenBorderCollider.X + ColliderManager.screenBorderCollider.Width - 0.04f) && !isFirstPlayerWindLeft)
            {
                gameManager.FirstPlayer.ChangeWindDirection(false);
            }
            else
                gameManager.FirstPlayer.ChangeWindDirection(true);
            if ((gameManager.SecondPlayer.GetCollider().X <= ColliderManager.screenBorderCollider.X) && !isSecondPlayerWindLeft)
            {
                gameManager.SecondPlayer.ChangeWindDirection(false);
            }
            else if ((gameManager.SecondPlayer.GetCollider().X + gameManager.SecondPlayer.GetCollider().Width >= ColliderManager.screenBorderCollider.X + ColliderManager.screenBorderCollider.Width - 0.04f) && !isSecondPlayerWindLeft) // || 
            {
                gameManager.SecondPlayer.ChangeWindDirection(false);
            }
            else
                gameManager.SecondPlayer.ChangeWindDirection(true);
        }
    }
}
