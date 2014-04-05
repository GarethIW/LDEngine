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


        public Camera(int width, int height, int boundswidth, int boundsheight)
        {
            Instance = this;

            Position = new Vector2(0, 0);
            Width = width;
            Height = height;

            ClampRect = new Rectangle((Width / 2), (Height / 2), (boundswidth) - (Width / 2), (boundsheight) - (Height / 2));

            // Set initial position and target
            Position.X = ClampRect.X;
            Position.Y = ClampRect.Y;
            Target = new Vector2(ClampRect.X, ClampRect.Y);
        }

        public Camera(int width, int height, Map map)
            : this(width, height, map.Width * map.TileWidth, map.Height * map.TileHeight)
        {
        }


        /// <summary>
        /// Update the camera
        /// </summary>
        /// 
        public void Update(GameTime gameTime)
        {
            // Clamp target to map/camera bounds
            Target.X = MathHelper.Clamp(Target.X, ClampRect.X, ClampRect.Width);
            Target.Y = MathHelper.Clamp(Target.Y, ClampRect.Y, ClampRect.Height);

            Position.X = MathHelper.Clamp(Position.X, ClampRect.X, ClampRect.Width);
            Position.Y = MathHelper.Clamp(Position.Y, ClampRect.Y, ClampRect.Height);

            if (shakeTime > 0)
            {
                shakeTime -= gameTime.ElapsedGameTime.TotalMilliseconds;
                shakeOffset = new Vector2((int)Helper.RandomFloat(-shakeAmount, shakeAmount), (int)Helper.RandomFloat(-shakeAmount, shakeAmount));
            }
            else shakeOffset = Vector2.Zero;

            // Move camera toward target
            Position = Vector2.Lerp(Position, Target, Speed);

            CameraMatrix = Matrix.CreateTranslation(-Position.X + shakeOffset.X, -Position.Y + shakeOffset.Y, 0) *
                           Matrix.CreateScale(Zoom) * 
                           Matrix.CreateRotationZ(Rotation) * 
                           Matrix.CreateTranslation(Width / 2f, Height / 2f, 0);
        }

        public void Shake(double time, float amount)
        {
            shakeTime = time;
            shakeAmount = amount;
        }

       
    }
}
