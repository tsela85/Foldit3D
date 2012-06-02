using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Foldit3D
{
    class NormalPlayer : Player
    {
        
        public NormalPlayer(Texture2D texture, List<List<Vector3>> points, PlayerManager pm, Effect effect) : base(texture, points, pm, effect) { }

        #region fold

        public override void foldData(Vector3 axis, Vector3 point, float a,bool beforeFold,bool afterFold)
        {
            float angle = MathHelper.ToDegrees(a);
            if (beforeFold)
            {

                if ((a > -MathHelper.Pi + Game1.closeRate) && (moving))
                {
                    if (angle < -90) isDraw = false;
                    worldMatrix = Matrix.Identity;
                    worldMatrix *= Matrix.CreateTranslation(-point);
                    worldMatrix *= Matrix.CreateFromAxisAngle(axis, -a);
                    worldMatrix *= Matrix.CreateTranslation(point);
                }
                else if (moving)
                {
                    isDraw = true;
                    for (int i = 0; i < vertices.Length; i++)
                    {
                        worldMatrix = Matrix.Identity;
                        worldMatrix *= Matrix.CreateTranslation(-point);
                        worldMatrix *= Matrix.CreateFromAxisAngle(axis, MathHelper.Pi);
                        worldMatrix *= Matrix.CreateTranslation(point);
                        vertices[i].Position = Vector3.Transform(vertices[i].Position, worldMatrix);

                    }
                    for (int j = 0; j < Math.Floor((double)vertices.Length / 2); j++)
                    {
                        VertexPositionTexture temp = new VertexPositionTexture();
                        temp = vertices[j];
                        vertices[j] = vertices[vertices.Length - j - 1];
                        vertices[vertices.Length - j - 1] = temp;
                    }
                    worldMatrix = Matrix.Identity;
                    moving = false;
                }


            }
           

            
        }

        #endregion
    }
}
