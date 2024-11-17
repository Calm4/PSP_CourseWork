using OpenTK.Input;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmmunitionLibrary;
using GameLibrary;
using GameLibrary.Dirigible;

namespace DirigibleBattle.Managers
{
    public class PlayerManager
    {
        private KeyboardState keyboardState;
        private NetworkManager networkManager;
        private GameManager gameManager;

        public readonly List<Key> CurrentPlayerInput = new List<Key>()
            {
                Key.W,
                Key.S,
                Key.A,
                Key.D,
            };


        public readonly List<Key> CurrentPlayerFire = new List<Key>()
        {
            Key.Z,
            Key.X,
            Key.C,
        };

        int currentPlayerTicks = 50;

        public void SetManagers(NetworkManager networkManager, GameManager gameManager)
        {
            this.networkManager = networkManager;
            this.gameManager = gameManager;
        }

        public void PlayerControl( int textureIdLeft, int textureIdRight)
        {
            // W S A D
            // Вверх Низ Лево Право
            KeyboardState keyboardState = Keyboard.GetState();
            Vector2 moveVectorFirstPlayer = Vector2.Zero;
            var playArea = ColliderManager.screenBorderCollider;

            if (keyboardState.IsKeyDown(CurrentPlayerInput[0]) && (networkManager.CurrentPlayer.GetCollider().Y < playArea.Width - playArea.Y))
            {
                moveVectorFirstPlayer += new Vector2(0f, -0.001f);
            }
            if (keyboardState.IsKeyDown(CurrentPlayerInput[1]))
            {
                moveVectorFirstPlayer += new Vector2(0f, 0.001f);
            }

            if (keyboardState.IsKeyDown(CurrentPlayerInput[2]) && (networkManager.CurrentPlayer.GetCollider().X > playArea.X))
            {
                networkManager.CurrentPlayer.DirigibleID = textureIdLeft;
                networkManager.CurrentPlayer.IsTurnedLeft = true;
                moveVectorFirstPlayer += new Vector2(-0.001f, 0f);
            }

            if (keyboardState.IsKeyDown(CurrentPlayerInput[3]) && (networkManager.CurrentPlayer.GetCollider().X < playArea.Width - 0.1f))
            {
                networkManager.CurrentPlayer.DirigibleID = textureIdRight;
                networkManager.CurrentPlayer.IsTurnedLeft = false;

                moveVectorFirstPlayer += new Vector2(0.001f, 0f);

            }
            if (moveVectorFirstPlayer != Vector2.Zero)
            {
                moveVectorFirstPlayer = Vector2.Normalize(moveVectorFirstPlayer) * networkManager.CurrentPlayer.Speed;
            }
            networkManager.CurrentPlayer.Move(moveVectorFirstPlayer);
        }
        public void PlayerShoot()
        {
            currentPlayerTicks++;
            keyboardState = Keyboard.GetState();

            bool playerFireCommon = keyboardState.IsKeyDown(CurrentPlayerFire[0]);
            bool playerFireFast = keyboardState.IsKeyDown(CurrentPlayerFire[1]);
            bool playerFireHeavy = keyboardState.IsKeyDown(CurrentPlayerFire[2]);

            /*Console.WriteLine(playerFireCommon + " playerFireCommon");
            Console.WriteLine(playerFireFast + " playerFireFast");
            Console.WriteLine(playerFireHeavy + " playerFireHeavy");*/

            var dirigibleRightTexture = networkManager.CurrentPlayer == gameManager.FirstPlayer ? TextureManager.firstDirigibleTextureRight : TextureManager.secondDirigibleTextureRight;

            if ((playerFireCommon || playerFireFast || playerFireHeavy) && currentPlayerTicks >= 50)
            {
                currentPlayerTicks = 0;
                if (networkManager.CurrentPlayer.Ammo > 0)
                {
                    Bullet bullet = null;
                    if (playerFireCommon)
                    {
                        bullet = new CommonBullet(networkManager.CurrentPlayer.GetGunPosition() - new Vector2(0f, -0.05f), TextureManager.commonBulletTexture, networkManager.CurrentPlayer.DirigibleID == dirigibleRightTexture);
                    }
                    if (playerFireFast)
                    {
                        bullet = new FastBullet(networkManager.CurrentPlayer.GetGunPosition() - new Vector2(0f, -0.05f), TextureManager.fastBulletTexture, networkManager.CurrentPlayer.DirigibleID == dirigibleRightTexture);
                    }
                    if (playerFireHeavy)
                    {
                        bullet = new HeavyBullet(networkManager.CurrentPlayer.GetGunPosition() - new Vector2(0f, -0.05f), TextureManager.heavyBulletTexture, networkManager.CurrentPlayer.DirigibleID == dirigibleRightTexture);
                    }


                    if (networkManager.CurrentPlayer == gameManager.FirstPlayer)
                    {
                        networkManager._firstPlayerBulletList.Add(bullet);
                    }
                    else
                    {
                        networkManager._secondPlayerBulletList.Add(bullet);
                    }

                    networkManager.BulletData = new BulletData()
                    {
                        ShooterID = networkManager.CurrentPlayer.DirigibleID,
                        PositionX = bullet.PositionCenter.X,
                        PositionY = bullet.PositionCenter.Y,
                        IsLeft = bullet.Direction.X > 0,
                        BulletType = SerializeAmmo(bullet)

                    };

                    networkManager.CurrentPlayer.Ammo--;
                }
            }
        }

        private static int SerializeAmmo(Bullet bullet)
        {
            if (bullet is CommonBullet)
            {
                return 0;
            }
            if (bullet is FastBullet)
            {
                return 1;
            }
            if (bullet is HeavyBullet)
            {
                return 2;
            }

            return -1;
        }

        public void CheckPlayerDamage(List<Bullet> bulletList, ref AbstractDirigible player)
        {
            for (int i = bulletList.Count - 1; i >= 0; i--)
            {
                bulletList[i].Fire();

                if (player.GetCollider().IntersectsWith(bulletList[i].GetCollider()))
                {
                    player.GetDamage(bulletList[i].Damage);
                    bulletList.RemoveAt(i);
                    continue;
                }
                if (!bulletList[i].GetCollider().IntersectsWith(ColliderManager.screenBorderCollider))
                {
                    bulletList.RemoveAt(i);
                }
            }
        }

        public void UpdatePlayerTexture()
        {
            UpdateTexture(networkManager.CurrentPlayer);
            UpdateTexture(networkManager.NetworkPlayer);
        }

        private void UpdateTexture(AbstractDirigible player)
        {
            if (player == gameManager.FirstPlayer)
            {
                player.DirigibleID = player.IsTurnedLeft
                    ? TextureManager.firstDirigibleTextureLeft
                    : TextureManager.firstDirigibleTextureRight;
            }
            else
            {
                player.DirigibleID = player.IsTurnedLeft
                    ? TextureManager.secondDirigibleTextureLeft
                    : TextureManager.secondDirigibleTextureRight;
            }
        }

    }
}
