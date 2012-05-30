using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

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
        protected float rotAngle;
        protected bool reverse = false;
        protected Color color = Color.White;
        protected int frameWidth;
        protected int frameHeight;
        protected PlayerManager playerManager;
        protected bool dataWasCalced = false;
        protected VertexPositionTexture[] vertices;
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

        public Player(Texture2D texture, List<List<Vector3>> points, PlayerManager pm, Effect effect)
        {
            this.texture = texture;
            frameHeight = texture.Height;
            frameWidth = texture.Width;
            playerManager = pm;
            this.effect = effect;
            setUpVertices(points);
        }

        #region Update and Draw
        public void Update(GameTime gameTime, GameState state)
        {
            if (state != GameState.folding)
            {
                Trace.WriteLine(state);
                moving = true;
                for(int i=0;i<vertices.Length; i++)
                    vertices[i].Position = Vector3.Transform(vertices[i].Position, worldMatrix);
            }
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


       /* public void foldOver()
        {
            rotAngle = 0;
            reverse = false;
            center = Vector2.Zero;
            dataWasCalced = false;
            HoleManager.checkCollision(this);
            PowerUpManager.checkCollision(this);
        }*/

        #region Virtual Methods

        public virtual void foldData(Vector3 axis, Vector3 point, float a) { }

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
        private void setUpVertices(List<List<Vector3>> points)
        {
            vertices = new VertexPositionTexture[6];

            for (int i = 0; i < 6; i++)
            {
                vertices[i].Position = points.ElementAt(i).ElementAt(0);
                vertices[i].TextureCoordinate.X = points.ElementAt(i).ElementAt(1).X;
                vertices[i].TextureCoordinate.Y = points.ElementAt(i).ElementAt(1).Y;
            }

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
