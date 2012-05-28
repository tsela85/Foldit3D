using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Foldit3D
{
    class Player
    {
        protected bool enabled;
        protected bool moving = true;
        protected Vector2 worldPosition;
        protected Texture2D texture;
        protected float ROTATION_DEGREE = 0.01f;
        protected Vector2 center = Vector2.Zero;
        protected double radius;
        protected float angle; //angle between ball and center
        protected float rotAngle;
        protected bool reverse = false;
        protected Color color = Color.White;
        protected int frameWidth;
        protected int frameHeight;
        protected PlayerManager playerManager;
        protected bool dataWasCalced = false;

        #region Properties

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public bool DataWasCalced
        {
            get { return dataWasCalced; }
            set { dataWasCalced = value; }
        }

        public bool Moving
        {
            get { return moving; }
            set { moving = value; }
        }

        public Vector2 WorldPosition
        {
            get { return worldPosition; }
            set { worldPosition = value; }
        }

        public Rectangle WorldRectangle
        {
            get
            {
                return new Rectangle(
                    (int)WorldPosition.X,
                    (int)WorldPosition.Y,
                    frameWidth,
                    frameHeight);
            }
        }

        #endregion

        public Player(Texture2D texture, int x, int y, PlayerManager pm)
        {
            this.texture = texture;
            worldPosition.X = x;
            worldPosition.Y = y;
            frameHeight = texture.Height;
            frameWidth = texture.Width;
            playerManager = pm;
        }

        #region Update and Draw
        public void Update(GameTime gameTime, GameState state)
        {
            if (state == GameState.folding && dataWasCalced)
            {
                rotate();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, new Rectangle((int)WorldPosition.X, (int)WorldPosition.Y, frameWidth, frameHeight), color);
        }

        #endregion Update and Draw

        #region Fold

        public void calcBeforeFolding(Vector2 loc1, Vector2 loc2)
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

        public void foldOver()
        {
            rotAngle = 0;
            reverse = false;
            center = Vector2.Zero;
            dataWasCalced = false;
            HoleManager.checkCollision(this);
            PowerUpManager.checkCollision(this);
        }

        #region Virtual Methods

        protected virtual void rotate() { }

        protected virtual void reverseRotation()
        {
            if (rotAngle > 0)
            {
                rotAngle -= ROTATION_DEGREE;
                worldPosition.X = (int)(center.X - radius * Math.Cos(rotAngle + angle));
                worldPosition.Y = (int)(center.Y - radius * Math.Sin(rotAngle + angle));
            }
        }
        #endregion

        #endregion

        #region PowerUps

        //factor - by how much to inlarge (or to make smaller) the player
        //for example:  factor = 2 means that the player will be twice as big, factor = 0.5 half of the size 
        public void changeSize(double factor)
        {
            frameHeight = (int)(frameHeight * factor);
            frameWidth = (int)(frameWidth * factor);
        }

        public void changePos(int newX, int newY){
            worldPosition.X = newX;
            worldPosition.Y = newY;
        }

        //!!!! i think that posx and posy need to be the postion of the powerup that the player took
        //newtype = normal/static/duplicate
        public void changePlayerType(String newType, int posX, int posY)
        {
            playerManager.changePlayerType(this, newType, posX, posY);
        }
        #endregion
    }
}
