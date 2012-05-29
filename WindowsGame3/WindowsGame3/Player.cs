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
        protected VertexPositionTexture[] vertices;
        //protected Matrix viewMatrix;
        //protected Matrix projectionMatrix;
        protected Matrix worldMatrix = Matrix.Identity;
        protected Effect effect;

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

        public Player(Texture2D texture, int x, int y, PlayerManager pm, Effect effect)
        {
            this.texture = texture;
            worldPosition.X = x;
            worldPosition.Y = y;
            frameHeight = texture.Height;
            frameWidth = texture.Width;
            playerManager = pm;
            this.effect = effect;
            setUpVertices();
        }

        #region Update and Draw
        public void Update(GameTime gameTime, GameState state)
        {
          /*  if (state == GameState.folding && dataWasCalced)
            {
                rotate();
            }*/
        }

        public void Draw()
        {
            effect.CurrentTechnique = effect.Techniques["TexturedNoShading"];
            effect.Parameters["xWorld"].SetValue(worldMatrix);
            effect.Parameters["xView"].SetValue(Game1.camera.View);
            effect.Parameters["xProjection"].SetValue(Game1.camera.Projection);
            effect.Parameters["xTexture"].SetValue(texture);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                Game1.device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 2, VertexPositionTexture.VertexDeclaration);
            }
        }

        #endregion Update and Draw

        #region Fold
        public void foldData(Vector3 axis, Vector3 point)
        {
            if (rotAngle < 180)
            {
                worldMatrix = Matrix.Identity;
                worldMatrix *= Matrix.CreateTranslation(-point);
                worldMatrix *= Matrix.CreateFromAxisAngle(axis, angle);
                worldMatrix *= Matrix.CreateTranslation(point);
                rotAngle += ROTATION_DEGREE;
            }
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

        #region 3D 
        private void setUpVertices()
        {
            vertices = new VertexPositionTexture[6];

            vertices[0].Position = new Vector3(-10.5f, 0f, -8.5f);
            vertices[0].TextureCoordinate.X = 0;
            vertices[0].TextureCoordinate.Y = 0;

            vertices[1].Position = new Vector3(-8.5f, 0f, -10.5f);
            vertices[1].TextureCoordinate.X = 1;
            vertices[1].TextureCoordinate.Y = 1;

            vertices[2].Position = new Vector3(-10.5f, 0f, -10.5f);
            vertices[2].TextureCoordinate.X = 0;
            vertices[2].TextureCoordinate.Y = 1;

            vertices[3].Position = new Vector3(-8.5f, 0f, -10.5f);
            vertices[3].TextureCoordinate.X = 1;
            vertices[3].TextureCoordinate.Y = 1;

            vertices[4].Position = new Vector3(-10.5f, 0f, -8.5f);
            vertices[4].TextureCoordinate.X = 0;
            vertices[4].TextureCoordinate.Y = 0;

            vertices[5].Position = new Vector3(-8.5f, 0f, -8.5f);
            vertices[5].TextureCoordinate.X = 1;
            vertices[5].TextureCoordinate.Y = 0;
        }

        public BoundingBox getBox()
        {
            Vector3[] p = new Vector3[2];
            p[0] = vertices[2].Position;
            p[1] = vertices[5].Position;
            return BoundingBox.CreateFromPoints(p);
        }
        #endregion
    }
}
