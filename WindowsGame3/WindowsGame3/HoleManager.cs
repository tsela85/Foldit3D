using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Foldit3D
{
    class HoleManager
    {
        Texture2D texture;
        private static List<Hole> holes;

        public HoleManager(Texture2D texture)
        {
            this.texture = texture;
            holes = new List<Hole>();
        }

        #region Levels

        public void initLevel(List<IDictionary<string, string>> data)
        {
            foreach (IDictionary<string, string> item in data)
            {
                holes.Add(new Hole(texture, Convert.ToInt32(item["x"]), Convert.ToInt32(item["y"])));
            }
        }

        public void restartLevel()
        {
            holes.Clear();
        }
        #endregion

        #region Draw
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Hole hole in holes)
                hole.Draw(spriteBatch);
        }
        #endregion

        #region Collision
        public static void checkCollision(Player player)
        {
            foreach (Hole h in holes)
            {
                if (h.WorldRectangle.Contains(player.WorldRectangle.Center))
                {
                    // WIN!!!
                    GameManager.winLevel();
                    break;
                }
            }
        }
        #endregion

        #region ChangeHoles
        public static void changeAllHolesPlace()
        {
            foreach (Hole h in holes)
                h.initializeHole(new Random().Next(100, 1100), new Random().Next(50, 550));
        }
        public static void cangeAllHolesSize()
        {
            foreach (Hole h in holes)
                h.changeSize(2);
        }
        #endregion
    }
}
