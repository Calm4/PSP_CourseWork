using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using AmmunitionLibrary;
using GameLibrary;
using PrizesLibrary.Prizes;

namespace DirigibleBattle.Managers
{
    public class RenderManager
    {
        private GameManager _gameManager;
        private NetworkManager _networkManager;

        public RenderManager(GameManager gameManager,NetworkManager networkManager)
        {
            _gameManager = gameManager;
            _networkManager = networkManager;
        }

        public void GlControl_Render(TimeSpan obj)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            ObjectRenderer.RenderObjects(TextureManager.backGroundTexture, new Vector2[4] {
                new Vector2(-1.0f, -1.0f),
                new Vector2(1.0f, -1.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(-1.0f, 1.0f),
            });

            ObjectRenderer.RenderObjects(TextureManager.mountainRange, new Vector2[4] {
                new Vector2(-1.0f, 0.775f),
                new Vector2(1.0f, 0.775f),
                new Vector2(1.0f, 1.0f),
                new Vector2(-1.0f, 1f),
            });
            _gameManager.FirstPlayer.Render();
            _gameManager.SecondPlayer.Render();

            foreach (Bullet bullet in _networkManager._firstPlayerBulletList)
            {
                bullet.Render();
            }
            foreach (Bullet bullet in _networkManager._secondPlayerBulletList)
            {
                bullet.Render();
            }

            foreach (Prize prize in _gameManager.PrizeList)
            {
                prize.Render();
            }
        }
    }
}
