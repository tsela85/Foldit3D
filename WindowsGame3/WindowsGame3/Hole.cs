using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Foldit3D
{
    class Hole
    {
        Texture2D texture;
        Vector2 worldPosition;
        Rectangle worldRectangle;
        public Hole(Texture2D texture,int x, int y)
        {
            this.texture = texture;
            worldPosition.X = x;
            worldPosition.Y = y;
            worldRectangle = new Rectangle((int)WorldPosition.X, (int)WorldPosition.Y, texture.Width, texture.Height);
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

        #region Draw
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, worldRectangle, null, Color.LightSeaGreen, 0, new Vector2(worldRectangle.Width / 2, worldRectangle.Height / 2), SpriteEffects.None, 0);
        }
        #endregion

        #region PublicMethods
        public void initializeHole(int posX, int posY)
        {
            worldPosition.X = posX;
            worldPosition.Y = posY;
        }

        public void changeSize(double factor)
        {
            worldRectangle.Height = (int)(worldRectangle.Height * factor);
            worldRectangle.Width = (int)(worldRectangle.Width * factor);
        }
        #endregion
    }
}
