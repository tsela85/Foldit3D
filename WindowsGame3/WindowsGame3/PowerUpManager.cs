﻿using System;
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
        private Effect effect;

        public PowerUpManager(Texture2D texture, Effect e)
        {
            this.texture = texture;
            powerups = new List<PowerUp>();
            effect = e;
        }

        #region Levels

        public void initLevel(List<IDictionary<string, string>> data)
        {
            foreach (IDictionary<string, string> item in data)
            {
                powerups.Add(new PowerUp(texture, ConvertType(Convert.ToInt32(item["type"])), Convert.ToInt32(item["x"]), Convert.ToInt32(item["y"]), effect));
            }
        }

        public void restartLevel()
        {
            powerups.Clear();
        }
        #endregion

        #region Draw
        public void Draw()
        {
            foreach (PowerUp p in powerups)
                p.Draw();
        }
        #endregion

        #region Update
        public void Update(GameState state)
        {
            foreach (PowerUp p in powerups)
                p.Update(state);
        }
        #endregion

        #region Public Methods
        public void calcBeforeFolding(Vector2 point1, Vector2 point2, int direction)
        {
            foreach (PowerUp p in powerups)
                p.calcBeforeFolding(point1, point2, direction);
        }

        public void foldData(Vector3 vec, Vector3 point, float angle)
        {
            foreach (PowerUp p in powerups)
            {
                p.foldData(vec, point, angle);
            }
        }
        #endregion

        #region Collision
        public static void checkCollision(Player player)
        {
            foreach (PowerUp p in powerups)
            {
                if (p.getBox().Contains(player.getBox()) == ContainmentType.Contains)
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
