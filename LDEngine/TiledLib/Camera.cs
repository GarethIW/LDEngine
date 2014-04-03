using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledLib;

namespace TiledLib
{
    public class Camera
    {
        public static Camera Instance;

        public Vector2 Position;
        public int Width;
        public int Height;
        public Vector2 Target;
        public Rectangle ClampRect;
        public float Rotation;
        public float RotationTarget;
        public float Zoom = 1f;

        public Matrix CameraMatrix;

        float Speed = 0.2f;

        double shakeTime;
        float shakeAmount;
        Vector2 shakeOffset = Vector2.Zero;

        /// <summary>
        /// Initialise the camera, using the game map to define the boundaries
        /// </summary>
        /// <param name="vp">Graphics viewport</param>
        /// <param name="map">Game Map</param>
        public Camera(int width, int height, Map map)
        {
            Instance = this;

            Position = new Vector2(0, 0);
            Width = width;
            Height = height;

            ClampRect = new Rectangle(0,0, map.Width * map.TileWidth, map.Height * map.TileHeight);

            if (map.Properties.Contains("CameraBoundsLeft"))
                ClampRect.X = Convert.ToInt32(map.Properties["CameraBoundsLeft"]) * map.TileWidth;
            if (map.Properties.Contains("CameraBoundsTop"))
                ClampRect.Y = Convert.ToInt32(map.Properties["CameraBoundsTop"]) * map.TileHeight;
            if (map.Properties.Contains("CameraBoundsWidth"))
                ClampRect.Width = Convert.ToInt32(map.Properties["CameraBoundsWidth"]) * map.TileWidth;
            if (map.Properties.Contains("CameraBoundsHeight"))
                ClampRect.Height = Convert.ToInt32(map.Properties["CameraBoundsHeight"]) * map.TileHeight;

            // Set initial position and target
            Position.X = ClampRect.X;
            Position.Y = ClampRect.Y;
            Target = new Vector2(ClampRect.X, ClampRect.Y);
        }
        

        /// <summary>
        /// Update the camera
        /// </summary>
        /// 
        public void Update(GameTime gameTime)
        {
            // Clamp target to map/camera bounds
            Target.X = MathHelper.Clamp(Target.X, ClampRect.X, ClampRect.Width - Width);
            Target.Y = MathHelper.Clamp(Target.Y, ClampRect.Y, ClampRect.Height - Height);

            Position.X = MathHelper.Clamp(Position.X, ClampRect.X, ClampRect.Width - Width);
            Position.Y = MathHelper.Clamp(Position.Y, ClampRect.Y, ClampRect.Height - Height);

            if (shakeTime > 0)
            {
                shakeTime -= gameTime.ElapsedGameTime.TotalMilliseconds;
                shakeOffset = new Vector2(0, Helper.RandomFloat(-shakeAmount, shakeAmount));
            }
            else shakeOffset = Vector2.Zero;

            // Move camera toward target
            Position = Vector2.Lerp(Position, Target, Speed);

            //Rotation = TurnToFace(Vector2.Zero, AngleToVector(RotationTarget, 1f), Rotation, 0.02f);

            CameraMatrix = Matrix.CreateTranslation(-Position.X, -Position.Y + shakeOffset.Y, 0)*
                           Matrix.CreateScale(Zoom)*Matrix.CreateRotationZ(Rotation);// *Matrix.CreateTranslation(Width / 2f, Height/2f, 0);
            //CameraMatrix *= Matrix.CreateRotationZ(Rotation);
        }

        public void Shake(double time, float amount)
        {
            shakeTime = time;
            shakeAmount = amount;
        }

       
    }
}
