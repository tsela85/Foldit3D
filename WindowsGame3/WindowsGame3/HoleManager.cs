using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Foldit3D
{
    class HoleManager
    {
        Texture2D texture;
        private static List<Hole> holes;
        private Effect effect;

        public HoleManager(Texture2D texture, Effect e)
        {
            this.texture = texture;
            holes = new List<Hole>();
            effect = e;
        }

        #region Levels

        public void initLevel(List<IDictionary<string, string>> data)
        {
            foreach (IDictionary<string, string> item in data)
            {
                holes.Add(new Hole(texture, Convert.ToInt32(item["x"]), Convert.ToInt32(item["y"]), effect));
            }
        }

        public void restartLevel()
        {
            holes.Clear();
        }
        #endregion

        #region Draw
        public void Draw()
        {
            foreach (Hole hole in holes)
                hole.Draw();
        }
        #endregion

        #region Update
        public void Update(GameState state)
        {
            foreach (Hole h in holes)
                h.Update(state);
        }
        #endregion

        #region Public Methods
        public void calcBeforeFolding(Vector2 point1, Vector2 point2, int direction)
        {
            foreach (Hole h in holes)
                h.calcBeforeFolding(point1, point2, direction);
        }
        #endregion

        #region Collision
        public static void checkCollision(Player player)
        {
            foreach (Hole h in holes)
            {
                if (h.getBox().Contains(player.getBox()) == ContainmentType.Contains)
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
