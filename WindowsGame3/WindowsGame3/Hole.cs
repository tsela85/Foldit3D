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

        protected VertexPositionTexture[] vertices;
        protected Matrix worldMatrix = Matrix.Identity;
        protected Effect effect;

        public Hole(Texture2D texture,int x, int y, Effect e)
        {
            this.texture = texture;
            worldPosition.X = x;
            worldPosition.Y = y;
            worldRectangle = new Rectangle((int)WorldPosition.X, (int)WorldPosition.Y, texture.Width, texture.Height);
            effect = e;
            setUpVertices();
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
        #endregion

        #region Update
        public void Update(GameState state)
        {
            if (state == GameState.folding && dataWasCalced)
                rotate();
        }
        #endregion

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


        public void calcBeforeFolding(Vector2 loc1, Vector2 loc2, int direction)
        {
            // NEED to check if the hole is in the folding area. if NOT: dataWasCalced = false. if YES: dataWasCalced = true.
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

        #region Public Methods
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

        #region 3D
        private void setUpVertices()
        {
            vertices = new VertexPositionTexture[6];

            vertices[0].Position = new Vector3(-5.5f, 0f, -2.5f);
            vertices[0].TextureCoordinate.X = 0;
            vertices[0].TextureCoordinate.Y = 0;

            vertices[1].Position = new Vector3(-2.5f, 0f, -5.5f);
            vertices[1].TextureCoordinate.X = 1;
            vertices[1].TextureCoordinate.Y = 1;

            vertices[2].Position = new Vector3(-5.5f, 0f, -5.5f);
            vertices[2].TextureCoordinate.X = 0;
            vertices[2].TextureCoordinate.Y = 1;

            vertices[3].Position = new Vector3(-2.5f, 0f, -5.5f);
            vertices[3].TextureCoordinate.X = 1;
            vertices[3].TextureCoordinate.Y = 1;

            vertices[4].Position = new Vector3(-5.5f, 0f, -2.5f);
            vertices[4].TextureCoordinate.X = 0;
            vertices[4].TextureCoordinate.Y = 0;

            vertices[5].Position = new Vector3(-2.5f, 0f, -2.5f);
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
