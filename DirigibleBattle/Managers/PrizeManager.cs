using GameLibrary.Dirigible;
using GameLibrary.DirigibleDecorators;
using PrizesLibrary.Prizes;
using System.Collections.Generic;
using System;

namespace DirigibleBattle.Managers
{
    public class PrizeManager : IGameObjectManager
    {
        private Random _random;
        private NetworkManager networkManager;
        private GameManager gameManager;

        public PrizeManager()
        {
            _random = new Random();
        }


        public void SetManagers(NetworkManager networkManager, GameManager gameManager)
        {
            this.networkManager = networkManager;
            this.gameManager = gameManager;
        }


        public void PrizeTimer_Tick(object sender, EventArgs e)
        {
            if (networkManager.CurrentPrizeList.Count < 3 && (gameManager.FirstPlayer.NumberOfPrizesReceived < 15 || gameManager.SecondPlayer.NumberOfPrizesReceived < 15))
            {
                Prize newPrize = networkManager.PrizeFactory.AddNewPrize();
                networkManager.CurrentPrize = newPrize;
                networkManager.CurrentPrizeList.Add(networkManager.CurrentPrize);
            }
            for (int i = 0; i < gameManager.PrizeList.Count; i++)
            {
                if (gameManager.FirstPlayer.NumberOfPrizesReceived >= 15 && gameManager.SecondPlayer.NumberOfPrizesReceived >= 15)
                {
                    networkManager.CurrentPrizeList.RemoveAt(gameManager.PrizeList.Count - 1);
                }
            }
        }

        public void ApplyPrize(List<Prize> prizeList, ref AbstractDirigible player)
        {
            for (int i = 0; i < prizeList.Count; i++)
            {
                networkManager.CurrentPrize = prizeList[i];


                if (player.GetCollider().IntersectsWith(networkManager.CurrentPrize.GetCollider()) && player.NumberOfPrizesReceived < 15)
                {
                    if (networkManager.CurrentPrize.GetType().Equals(typeof(AmmoPrize)))
                    {
                        int ammoBoostCount = 5;
                        player = new AmmoBoostDecorator(player, ammoBoostCount);
                        player.NumberOfPrizesReceived++;
                    }
                    if (networkManager.CurrentPrize.GetType().Equals(typeof(ArmorPrize)))
                    {
                        int armorBoostCount = 15;
                        player = new ArmorBoostDecorator(player, armorBoostCount);
                        player.NumberOfPrizesReceived++;
                    }
                    if (networkManager.CurrentPrize.GetType().Equals(typeof(FuelPrize)))
                    {
                        int fuelBoostCount = 500;
                        player = new FuelBoostDecorator(player, fuelBoostCount);
                        player.NumberOfPrizesReceived++;
                    }
                    if (networkManager.CurrentPrize.GetType().Equals(typeof(HealthPrize)))
                    {
                        int healthBoostCount = 20;
                        player = new HealthBoostDecorator(player, healthBoostCount);
                        player.NumberOfPrizesReceived++;
                    }
                    if (networkManager.CurrentPrize.GetType().Equals(typeof(SpeedBoostPrize)))
                    {
                        float speedBoostCount = 0.015f;
                        player = new SpeedBoostDecorator(player, speedBoostCount);
                        player.NumberOfPrizesReceived++;
                    }
                    prizeList.Remove(networkManager.CurrentPrize);
                    i--;
                }
            }
        }

    }
}
