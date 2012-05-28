using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Foldit3D
{
    enum PowerUpType { HoleSize, HolePos, PlayerSize, PlayerPos, SplitPlayer, DryPlayer, NormalPlayer };

    class PowerUp
    {
        Texture2D texture;
        Vector2 worldPosition;
        Rectangle worldRectangle;
        PowerUpType type;

        public PowerUp(Texture2D t, PowerUpType ty, int x, int y) 
        {
            texture = t;
            worldPosition.X = x;
            worldPosition.Y = y;
            worldRectangle = new Rectangle((int)WorldPosition.X,(int)WorldPosition.Y,texture.Width, texture.Height);
            type = ty;
        }

        #region Properties
        public Vector2 WorldPosition
        {
            get { return worldPosition; }
            set { worldPosition = value; }
        }

        public Rectangle WorldRectangle
        {
            get
            {
               return worldRectangle;
            }
        }
        #endregion

        #region Update
        public void doYourThing(Player player)
        {
            switch (type)
            {
                case PowerUpType.HoleSize:
                    HoleManager.cangeAllHolesSize();
                    break;
                case PowerUpType.HolePos:
                    HoleManager.changeAllHolesPlace();
                    break;
                case PowerUpType.PlayerSize:
                    player.changeSize(2);
                    break;
                case PowerUpType.PlayerPos:
                    player.changePos(new Random().Next(100, 1100), new Random().Next(50, 550));
                    break;
                case PowerUpType.SplitPlayer:
                    player.changePlayerType("duplicate", (int)worldPosition.X, (int)worldPosition.Y);
                    break;
                case PowerUpType.DryPlayer:
                    player.changePlayerType("static", (int)worldPosition.X, (int)worldPosition.Y);
                    break;
                case PowerUpType.NormalPlayer:
                    player.changePlayerType("normal", (int)worldPosition.X, (int)worldPosition.Y);
                    break;
            }
        }
        #endregion

        #region Draw
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, worldRectangle, null, Color.Red, 0, new Vector2(worldRectangle.Width / 2, worldRectangle.Height / 2), SpriteEffects.None, 0);
        }
        #endregion
    }
}
