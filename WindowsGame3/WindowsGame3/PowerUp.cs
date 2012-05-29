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
        float ROTATION_DEGREE = 0.01f;
        float rotAngle;
        float angle;
        bool reverse = false;
        Vector2 center = Vector2.Zero;
        double radius;
        bool dataWasCalced = false;

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

        #region Action
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

        #region Update
        public void Update(GameState state)
        {
            if (state == GameState.folding && dataWasCalced)
                rotate();
        }
        #endregion

        #region fold
        public void calcBeforeFolding(Vector2 loc1, Vector2 loc2, int direction)
        {
            // NEED to check if the powerup is in the folding area. if NOT: dataWasCalced = false. if YES: dataWasCalced = true.
            if (isHoleInFoldArea(loc1, loc2, direction))
            {
                double m1 = (((double)(loc2.Y)) - loc1.Y) / (loc2.X - loc1.X);
                double m2 = (-1) / m1;
                double pointX = ((-m2 * worldPosition.X) + worldPosition.Y + (m1 * loc1.X) - loc1.Y) / (m1 - m2);
                double pointY = m1 * pointX + (-loc1.X * m1) + loc1.Y;
                double test = m2 * pointX + (-worldPosition.X * m2) + worldPosition.Y;
                center = new Vector2((int)pointX, (int)pointY);
                radius = Math.Sqrt(Math.Pow((center.X - worldPosition.X), 2) + Math.Pow((center.Y - worldPosition.Y), 2));
                angle = (float)Math.Atan((center.Y - worldPosition.Y) / (center.X - worldPosition.X));
                dataWasCalced = true;
            }
            else dataWasCalced = false;
        }

        public void reverseRotation()
        {
            if (rotAngle > 0)
            {
                rotAngle -= ROTATION_DEGREE;
                worldPosition.X = (int)(center.X - radius * Math.Cos(rotAngle + angle));
                worldPosition.Y = (int)(center.Y - radius * Math.Sin(rotAngle + angle));
            }
        }
        public void rotate()
        {
            if (reverse)
            {
                reverseRotation();
                return;
            }
            if (rotAngle < MathHelper.Pi)
            {
                rotAngle += ROTATION_DEGREE;
                worldPosition.X = (int)(center.X - radius * Math.Cos(rotAngle + angle));
                worldPosition.Y = (int)(center.Y - radius * Math.Sin(rotAngle + angle));
            }
            else
            {
                reverse = true;
                reverseRotation();
            }
        }
        #endregion

        #region Draw
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, worldRectangle, null, Color.Red, 0, new Vector2(worldRectangle.Width / 2, worldRectangle.Height / 2), SpriteEffects.None, 0);
        }
        #endregion

        #region Private Methods
        public bool isHoleInFoldArea(Vector2 loc1, Vector2 loc2, int dir)
        {
            // dir=left
            if (dir == 0)
            {
            }
            // dir=right
            else
            {
            }
            return true;
        }
        #endregion
    }
}
