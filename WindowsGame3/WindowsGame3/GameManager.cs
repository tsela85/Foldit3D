using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Foldit3D
{
    enum GameState { normal, folding, scored };

    class GameManager
    {
        SpriteFont font, scoreFont;
        //Board board;
        static GameState gamestate;
        HoleManager holeManager;
        PlayerManager playerManager;
        PowerUpManager powerupManager;
        Board board;


        int level;
        int endLevel;
        int folds;


        ///////////////////////////
        List<IDictionary<string, string>> levels = new List<IDictionary<string, string>>();

        // XXX need to recieve all the instances: bordMan, playerMan, holeMan etc.
        public GameManager(SpriteFont f, SpriteFont sf, HoleManager h, PlayerManager p, PowerUpManager pu,
            Board bo)
        {
            font = f;
            scoreFont = sf;
            holeManager = h;
            playerManager = p;
            powerupManager = pu;
            board = bo;
            gamestate = GameState.normal;
            folds = 0;
            level = 1;
            endLevel = 1;
        }

        public void loadCurrLevel() 
        {
            playerManager.restartLevel();
            playerManager.initLevel(XMLReader.Get(level, "player"));
            holeManager.restartLevel();
            holeManager.initLevel(XMLReader.Get(level, "holes"));
            powerupManager.restartLevel();
            powerupManager.initLevel(XMLReader.Get(level, "powerups"));
            Vector3[] points = new Vector3[4] {
                new Vector3(-25f, 0f, 20f),
                new Vector3(25f, 0f, 20f),
                new Vector3(25f, 0f, -20f),
                new Vector3(-25f, 0f, -20f)
             };
            Vector2[] texCords = new Vector2[4] {
                new Vector2(0,0),
                new Vector2(1,0),
                new Vector2(1,1),
                new Vector2(0,1)  
             };
            Game1.camera.Initialize();
            board.Initialize(4, points, texCords);
        } 

        #region Update
        public void Update(GameTime gameTime)
        {
            playerManager.Update(gameTime, gamestate);
            board.update();
            //gamestate = Game1.input.Update(gameTime);
            Game1.input.Update(gameTime);
            Game1.camera.UpdateCamera(gameTime);
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                folds = 0;
            }
            if ((gamestate == GameState.scored) && (Mouse.GetState().LeftButton == ButtonState.Pressed))
            {
                folds = 0;
                level++;
                if (level<=endLevel)
                    loadCurrLevel();
            }
            if (gamestate == GameState.folding)
            {
                // NEED to recive points from the bord
                //playerManager.calcBeforeFolding(Vector2 point1, Vector2 point2);
                folds++;
            }
        }
        #endregion

        #region Draw
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            Game1.device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;

          //  rs.FillMode = FillMode.WireFrame;
            Game1.device.RasterizerState = rs;

            //holeManager.Draw(spriteBatch);
            //powerupManager.Draw(spriteBatch);
         //   playerManager.Draw(spriteBatch);

            //rs.FillMode = FillMode.WireFrame;
            Game1.device.RasterizerState = rs;

            board.Draw();
            holeManager.Draw();
            powerupManager.Draw();
            playerManager.Draw();

            //spriteBatch.DrawString(font, "Fold the page, till the ink-stain is in the hole", new Vector2(50, 15), Color.Black);
            //spriteBatch.DrawString(font, "Mouse Left Button - choose, Mouse Right Button - cancel", new Vector2(50, graphics.PreferredBackBufferHeight - 50), Color.Black);
            //spriteBatch.DrawString(font, "folds: " + folds, new Vector2(graphics.PreferredBackBufferWidth - 150, 15), Color.Black);
            //spriteBatch.DrawString(font, "level: " + level, new Vector2(graphics.PreferredBackBufferWidth - 150, graphics.PreferredBackBufferHeight - 50), Color.Black);
            //spriteBatch.DrawString(font, "press R to restart level", new Vector2(50, 150), Color.Black
            //        , (MathHelper.Pi / 2) + 0.02f, new Vector2(0, 0), 1, SpriteEffects.None, 0);
            //spriteBatch.DrawString(font, "Click on the page edges to fold it", new Vector2(1185, 100), Color.Black
            //        , (MathHelper.Pi / 2), new Vector2(0, 0), 1, SpriteEffects.None, 0);
            //if (gamestate == GameState.scored)
            //{
            //    string output = "    WINNER!! \n only " + folds + " folds";
            //    Vector2 FontOrigin = scoreFont.MeasureString(output) / 2;
            //    spriteBatch.DrawString(scoreFont, output, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), Color.Black
            //        , 0, FontOrigin, 1.0f, SpriteEffects.None, 0);
          //  }
        }
        #endregion

        #region Win
        public static void winLevel()
        {
            gamestate = GameState.scored;
            // print to screen 
        }
        #endregion
    }
}
