using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Foldit3D
{
    class DuplicatePlayer : Player
    {

        public DuplicatePlayer(Texture2D texture, int x, int y, PlayerManager pm, Effect effect) : base(texture, x, y, pm, effect) { }

        #region fold

        protected override void rotate()
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
                playerManager.makeNewPlayer("normal", (int)worldPosition.X, (int)worldPosition.Y);
                reverse = true;
                reverseRotation();
            }
        }

        #endregion
    }
}
