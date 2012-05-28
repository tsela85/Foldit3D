using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Foldit3D
{
    class PowerUpManager
    {
        Texture2D texture;
        private static List<PowerUp> powerups;

        public PowerUpManager(Texture2D texture)
        {
            this.texture = texture;
            powerups = new List<PowerUp>();
        }

        #region Levels

        public void initLevel(List<IDictionary<string, string>> data)
        {
            foreach (IDictionary<string, string> item in data)
            {
                powerups.Add(new PowerUp(texture, ConvertType(Convert.ToInt32(item["type"])), Convert.ToInt32(item["x"]), Convert.ToInt32(item["y"])));
            }
        }

        public void restartLevel()
        {
            powerups.Clear();
        }
        #endregion

        #region Draw
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (PowerUp p in powerups)
                p.Draw(spriteBatch);
        }
        #endregion

        #region Collision
        public static void checkCollision(Player player)
        {
            foreach (PowerUp p in powerups)
            {
                if (p.WorldRectangle.Contains(player.WorldRectangle.Center))
                {
                    p.doYourThing(player);
                }
            }
        }
        #endregion

        #region Private Methods
        private PowerUpType ConvertType(int type)
        {
            switch (type)
            {
                case 0:
                    return PowerUpType.HoleSize;
                case 1:
                    return PowerUpType.HolePos;
                case 2:
                    return PowerUpType.PlayerSize;
                case 3:
                    return PowerUpType.PlayerPos;
                case 4:
                    return PowerUpType.SplitPlayer;
                case 5:
                    return PowerUpType.DryPlayer;
                case 6:
                    return PowerUpType.NormalPlayer;
                default:
                    return PowerUpType.NormalPlayer;
            }
        }
        #endregion

    }
}
