using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Foldit3D
{
    class PlayerManager
    {
        private Texture2D texture;
        private List<Player> players;
        private Effect effect;
        public PlayerManager(Texture2D texture, Effect effect)
        {
            this.texture = texture;
            players = new List<Player>();
            this.effect = effect;
            //List<IDictionary<string, string>> data = XMLReader.Get("player");
            //initLevel(data);
        }

        #region Levels

        public void initLevel(List<IDictionary<string, string>> data)
        {
            foreach (IDictionary<string, string> item in data)
            {
                players.Add(makeNewPlayer(item["type"], Convert.ToInt32(item["x"]), Convert.ToInt32(item["y"])));
            }
        }

        public void restartLevel()
        {
            players.Clear();
        }
        #endregion

        #region Update and Draw

        public void Draw(SpriteBatch spriteBatch) {
            foreach (Player p in players)
            {
                p.Draw(spriteBatch);
            }
        }
        public void Update(GameTime gameTime, GameState state) {
            foreach (Player p in players)
            {
                p.Update( gameTime, state);
            }
        }

        #endregion

        #region Public Methods

        public void calcBeforeFolding(Vector2 point1, Vector2 point2)
        {
            foreach (Player p in players)
            {
                p.calcBeforeFolding(point1,point2);
            }
        }

        public Player makeNewPlayer(String type, int x, int y){
            Player newP = null;
            if (type.CompareTo("normal") == 0)
            {
                newP = new NormalPlayer(texture, x, y, this, effect);
            }
            else if (type.CompareTo("static") == 0)
            {
                newP = new StaticPlayer(texture, x, y, this, effect);
            }
            else if (type.CompareTo("duplicate") == 0)
            {
                newP = new DuplicatePlayer(texture, x, y, this, effect);
            }
            return newP;
        }

        public void foldOver()
        {
            foreach (Player p in players)
            {
                p.foldOver();
            }
        }

        public void changePlayerType(Player p,String type, int x, int y)
        {
            if (players.Contains(p))
            {
                players.Add(makeNewPlayer(type, x, y));
                players.Remove(p);
            }
            else Trace.WriteLine("changePlayerType Error!");
        }


        #endregion

    }
}
