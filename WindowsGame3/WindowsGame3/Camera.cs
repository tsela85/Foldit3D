using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Foldit3D
{
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        // the following constants control the speed at which the camera moves
        // how fast does the camera move up, down, left, and right?
        const float CameraRotateSpeed = .1f;
        // how fast does the camera zoom in and out?
        const float CameraZoomSpeed = .03f;
        // the camera can't be further away than this distance
        const float CameraMaxDistance = 120.0f;
        // and it can't be closer than this
        const float CameraMinDistance = 15f;

        // the following constants control how the camera's default position
        const float CameraDefaultArc = -30.0f;
        const float CameraDefaultRotation = 225;
        const float CameraDefaultDistance = 80f;


        private Matrix projection;
        public Matrix Projection
        {
            get { return projection; }
        }

        private Matrix view;
        public Matrix View
        {
            get { return view; }
        }

        private Vector3 cameraPosition = new Vector3(0, 80, 0);
        private Vector3 cameraTarget = Vector3.Zero;
        private Vector3 cameraUpVector = new Vector3(0, 0, -1);

        private Vector3 cameraReference = new Vector3(0.0f, 0.0f, -1.0f);
        private float cameraYaw = 0.0f;
        private float cameraPitch = 0.0f;

        private float spinRate = 120.00f;

        // The next set of variables are used to control the camera used in the sample. 
        // It is an arc ball camera, so it can rotate in a sphere around the target, and
        // zoom in and out.
        float cameraArc = CameraDefaultArc;
        float cameraRotation = CameraDefaultRotation;
        float cameraDistance = CameraDefaultDistance;

        private InputHandler input;

        public Camera(Game game)
            : base(game)
        {
           // input = (IInputHandler)game.Services.GetService(typeof(IInputHandler));
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            input = Game1.input;
            InitializeCamera();
        }

        private void InitializeCamera()
        {
            //Projection
            float aspectRatio = (float)Game.GraphicsDevice.Viewport.Width /
                (float)Game.GraphicsDevice.Viewport.Height;
            Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio,0.2f, 500.0f,out projection);
         //       1.0f, 10000.0f, out projection);

            //View
            Matrix.CreateLookAt(ref cameraPosition, ref cameraTarget,
                ref cameraUpVector, out view);

          
        }

        /// <summary>
        /// Handles input for moving the camera.
        /// </summary>
        public void UpdateCamera(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // should we reset the camera?
            if (input.KeyboardHandler.IsKeyDown(Keys.R))
            {
                cameraArc = CameraDefaultArc;
                cameraDistance = CameraDefaultDistance;
                cameraRotation = CameraDefaultRotation;
            }

            // Check for input to rotate the camera up and down around the model.
            if (input.KeyboardHandler.IsKeyDown(Keys.Up) ||
                input.KeyboardHandler.IsKeyDown(Keys.W))
            {
                cameraArc += time * CameraRotateSpeed;
            }

            if (input.KeyboardHandler.IsKeyDown(Keys.Down) ||
                input.KeyboardHandler.IsKeyDown(Keys.S))
            {
                cameraArc -= time * CameraRotateSpeed;
            }

            //cameraArc += currentGamePadState.ThumbSticks.Right.Y * time *
            //    CameraRotateSpeed;

            // Limit the arc movement.
            cameraArc = MathHelper.Clamp(cameraArc,-90.0f, -10.0f);

            // Check for input to rotate the camera around the model.
            if (input.KeyboardHandler.IsKeyDown(Keys.Right) ||
                input.KeyboardHandler.IsKeyDown(Keys.D))
            {
                cameraRotation += time * CameraRotateSpeed;
            }

            if (input.KeyboardHandler.IsKeyDown(Keys.Left) ||
                input.KeyboardHandler.IsKeyDown(Keys.A))
            {
                cameraRotation -= time * CameraRotateSpeed;
            }

            //cameraRotation += currentGamePadState.ThumbSticks.Right.X * time *
            //    CameraRotateSpeed;

            // Check for input to zoom camera in and out.
            if (input.KeyboardHandler.IsKeyDown(Keys.Z))
                cameraDistance += time * CameraZoomSpeed;

            if (input.KeyboardHandler.IsKeyDown(Keys.X))
                cameraDistance -= time * CameraZoomSpeed;

            //cameraDistance += currentGamePadState.Triggers.Left * time
            //    * CameraZoomSpeed;
            //cameraDistance -= currentGamePadState.Triggers.Right * time
            //    * CameraZoomSpeed;

            // clamp the camera distance so it doesn't get too close or too far away.
            cameraDistance = MathHelper.Clamp(cameraDistance,
                CameraMinDistance, CameraMaxDistance);

            Matrix unrotatedView = Matrix.CreateLookAt(
                new Vector3(0, 0, cameraDistance), Vector3.Zero, Vector3.Down);

            view = Matrix.CreateRotationY(MathHelper.ToRadians(cameraRotation)) *
                          Matrix.CreateRotationX(MathHelper.ToRadians(cameraArc)) *
                          unrotatedView;
        }
    }
}
