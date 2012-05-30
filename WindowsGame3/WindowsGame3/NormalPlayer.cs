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

        public override void foldData(Vector3 axis, Vector3 point, float a)
        {
            float angle = MathHelper.ToDegrees(a);
            
            
          //  if (angle > -167 && angle < 0 && moving)
            if ((a > -MathHelper.Pi + Game1.closeRate) && (moving))
            {
                worldMatrix = Matrix.Identity;
                worldMatrix *= Matrix.CreateTranslation(-point);
                worldMatrix *= Matrix.CreateFromAxisAngle(axis, -a);
                worldMatrix *= Matrix.CreateTranslation(point);
            }
            else if (moving)
            {

                for (int i = 0; i < vertices.Length; i++)
                {
                    worldMatrix = Matrix.Identity;
                    worldMatrix *= Matrix.CreateTranslation(-point);
                    worldMatrix *= Matrix.CreateFromAxisAngle(axis, MathHelper.Pi);
                    worldMatrix *= Matrix.CreateTranslation(point);
                    vertices[i].Position = Vector3.Transform(vertices[i].Position, worldMatrix);
                }
                worldMatrix = Matrix.Identity;
                moving = false;
            }
        }

        #endregion
    }
}
