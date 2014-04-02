using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace VoxelGame
{
    public static class Helper
    {
        public static Random Random = new Random();

        public static float AngleBetween(Vector2 v1, Vector2 v2)
        {
            if(v1!=Vector2.Zero) v1.Normalize();
            if(v2!=Vector2.Zero) v2.Normalize();
            float Angle = (float)Math.Acos(Vector2.Dot(v1, v2));
            return Angle;
        }

        public static float TurnToFace(Vector2 position, Vector2 faceThis,
            float currentAngle, float direction, float turnSpeed)
        {
            // consider this diagram:
            //         C 
            //        /|
            //      /  |
            //    /    | y
            //  / o    |
            // S--------
            //     x
            // 
            // where S is the position of the spot light, C is the position of the cat,
            // and "o" is the angle that the spot light should be facing in order to 
            // point at the cat. we need to know what o is. using trig, we know that
            //      tan(theta)       = opposite / adjacent
            //      tan(o)           = y / x
            // if we take the arctan of both sides of this equation...
            //      arctan( tan(o) ) = arctan( y / x )
            //      o                = arctan( y / x )
            // so, we can use x and y to find o, our "desiredAngle."
            // x and y are just the differences in position between the two objects.
            float x = (faceThis.X - position.X) * direction;
            float y = (faceThis.Y - position.Y) * direction;

            // we'll use the Atan2 function. Atan will calculates the arc tangent of 
            // y / x for us, and has the added benefit that it will use the signs of x
            // and y to determine what cartesian quadrant to put the result in.
            // http://msdn2.microsoft.com/en-us/library/system.math.atan2.aspx
            float desiredAngle = (float)Math.Atan2(y, x);

            // so now we know where we WANT to be facing, and where we ARE facing...
            // if we weren't constrained by turnSpeed, this would be easy: we'd just 
            // return desiredAngle.
            // instead, we have to calculate how much we WANT to turn, and then make
            // sure that's not more than turnSpeed.

            // first, figure out how much we want to turn, using WrapAngle to get our
            // result from -Pi to Pi ( -180 degrees to 180 degrees )
            float difference = WrapAngle(desiredAngle - currentAngle);

            // clamp that between -turnSpeed and turnSpeed.
            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);

            // so, the closest we can get to our target is currentAngle + difference.
            // return that, using WrapAngle again.
            return WrapAngle(currentAngle + difference);
        }

        /// <summary>
        /// Returns the angle expressed in radians between -Pi and Pi.
        /// </summary>
        public static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }

        public static Vector2 PointOnCircle(ref Vector2 C, float R, float A)
        {
            //A = A - 90;
            float endX = (C.X + (R * ((float)Math.Cos((float)A))));
            float endY = (C.Y + (R * ((float)Math.Sin((float)A))));
            return new Vector2(endX, endY);
        }

        public static Vector2 RandomPointInCircle(Vector2 position, float minradius, float maxradius)
        {
            float randomRadius = minradius + ((maxradius-minradius) * (float)Math.Sqrt(Random.NextDouble()));

            double randomAngle = Random.NextDouble() * MathHelper.TwoPi;

            float x = randomRadius * (float)Math.Cos(randomAngle);
            float y = randomRadius * (float)Math.Sin(randomAngle);

            return new Vector2(position.X + x, position.Y + y);
        }

        public static Vector2 AngleToVector(float angle, float length)
        {
            Vector2 direction = Vector2.Zero;
            direction.X = (float)Math.Cos(angle) * length;
            direction.Y = (float)Math.Sin(angle) * length;
            return direction;
        }

        public static float V2ToAngle(Vector2 direction)
        {
            return (float)Math.Atan2(direction.Y, direction.X);
        }

        public static Vector2 PtoV(Point p)
        {
            return new Vector2(p.X, p.Y);
        }

        public static Point VtoP(Vector2 v)
        {
            return new Point((int)v.X, (int)v.Y);
        }

        public static bool IsPointInShape(Vector2 point, List<Vector2> verts)
        {
            bool oddNodes = false;

            int j = verts.Count - 1;
            float x = point.X;
            float y = point.Y;

            for (int i = 0; i < verts.Count; i++)
            {
                Vector2 tpi = verts[i];
                Vector2 tpj = verts[j];

                if (tpi.Y < y && tpj.Y >= y || tpj.Y < y && tpi.Y >= y)
                    if (tpi.X + (y - tpi.Y) / (tpj.Y - tpi.Y) * (tpj.X - tpi.X) < x)
                        oddNodes = !oddNodes;

                j = i;
            }

            return oddNodes;
        }

        public static Matrix GetRotationMatrix(Vector3 source, Vector3 target)
        {
            float dot = Vector3.Dot(source, target);
            if (!float.IsNaN(dot))
            {
                float angle = (float)Math.Acos(dot);
                if (!float.IsNaN(angle))
                {
                    Vector3 cross = Vector3.Cross(source, target);
                    cross.Normalize();
                    Matrix rotation = Matrix.CreateFromAxisAngle(cross, angle);
                    return rotation;
                }
            }
            return Matrix.Identity;
        }

        public static Matrix FaceMatrix(Vector3 Position, Vector3 TargetPosition) 
        { 
            Vector3 tminusp = TargetPosition - Position; 
            Vector3 ominusp = new Vector3(0,0,-1); 
            tminusp.Normalize(); 

            float theta = (float)System.Math.Acos(Vector3.Dot(tminusp, ominusp)); 
            Vector3 cross = Vector3.Cross(ominusp, tminusp); 
            cross.Normalize(); 
            
            Quaternion targetQ = Quaternion.CreateFromAxisAngle(cross, theta);
            Matrix WorldMatrix = Matrix.CreateFromQuaternion(targetQ); 
            return WorldMatrix; 
        }

        public static Vector3 ProjectMousePosition(Vector2 mousePos, Microsoft.Xna.Framework.Graphics.Viewport viewport, Matrix world, Matrix view, Matrix projection, float depth)
        {
            Vector3 nearSource = new Vector3(mousePos.X, mousePos.Y, 0);
            Vector3 farSource = new Vector3(mousePos.X, mousePos.Y, 1);

            Vector3 nearPoint = viewport.Unproject(nearSource, projection, view, world);
            Vector3 farPoint = viewport.Unproject(farSource, projection, view, world); 
            
            Ray ray = new Ray(nearPoint, Vector3.Normalize(farPoint - nearPoint)); 
            Plane plane = new Plane(new Vector3(0,0,-1), depth); 
            
            float denominator = Vector3.Dot(plane.Normal, ray.Direction); 
            float numerator = Vector3.Dot(plane.Normal, ray.Position) + plane.D; 
            float t = -(numerator / denominator); 
            
            return nearPoint + ray.Direction * t;
            
        }

        internal static float RandomFloat(float p)
        {
            return (float)Random.NextDouble() * p;
        }

        public static float Truncate(this float value, int digits)
        {
            double mult = Math.Pow(10.0, digits);
            double result = Math.Truncate(mult * value) / mult;
            return (float)result;
        }
    }
}
