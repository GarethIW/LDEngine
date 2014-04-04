using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TiledLib;

namespace LDEngine
{
    public enum ParticleState
    {
        Attack,
        Alive,
        Decay,
        Done
    }

    public enum ParticleBlend
    {
        Alpha,
        Additive,
        Multiplicative
    }


    public class Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public ParticleState State;
        public ParticleBlend Blend;
        public int Depth;
        public bool AffectedByGravity;
        public bool CanCollide;
        public float Alpha;

        public float AttackValue;
        public float DecayValue;
        public float LifeValue;

        public double AttackTime;
        public double LifeTime;
        public double DecayTime;

        public double CurrentTime;

        public float Scale;
        public float Rotation;
        public Color Color;
        public Rectangle SourceRect;

        public Action<Particle> ParticleFunction;
    }

    public class ParticleController
    {
        const int MAX_PARTICLES = 3000;

        public Particle[] Particles;
        public Random Rand = new Random();

        public Texture2D _texParticles;

        private BlendState multiplicativeBlend;

        public ParticleController()
        {
            Particles = new Particle[MAX_PARTICLES];
        }

        public void LoadContent(ContentManager content)
        {
            _texParticles = content.Load<Texture2D>("particles");

            for (int i = 0; i < MAX_PARTICLES; i++)
                Particles[i] = new Particle() {State = ParticleState.Done};

            multiplicativeBlend = new BlendState();
           
            multiplicativeBlend.ColorDestinationBlend = Blend.SourceColor;
            multiplicativeBlend.ColorSourceBlend = Blend.DestinationColor;
            //multiplicativeBlend.AlphaSourceBlend = Blend.One;
            //multiplicativeBlend.AlphaDestinationBlend = Blend.SourceAlpha;
            //multiplicativeBlend.AlphaBlendFunction = BlendFunction.Add;
            //multiplicativeBlend.ColorBlendFunction = BlendFunction.Add;

        }

        public void Update(GameTime gameTime, Map gameMap)
        {
            foreach (Particle p in Particles.Where(p => p.State != ParticleState.Done))
            {
                p.CurrentTime += gameTime.ElapsedGameTime.TotalMilliseconds;

                if (p.AffectedByGravity) p.Velocity.Y += 0.05f;

                if (p.CanCollide && gameMap!=null)
                {
                    if (gameMap.CheckCollision(p.Position + new Vector2(p.Velocity.X, 0)).GetValueOrDefault())
                    {
                        p.Velocity.X = -(p.Velocity.X*(0.1f + Helper.RandomFloat(0.4f)));
                        p.Velocity.Y *= 0.9f;
                    }

                    if (gameMap.CheckCollision(p.Position + new Vector2(0, p.Velocity.Y)).GetValueOrDefault())
                    {
                        p.Velocity.Y = -(p.Velocity.Y*(0.1f + Helper.RandomFloat(0.4f)));
                        p.Velocity.X *= 0.9f;
                    }
                }

                p.Position += p.Velocity;

                switch (p.State)
                {
                    case ParticleState.Attack:
                        p.AttackValue = (1f/(float) p.AttackTime)*(float)p.CurrentTime;
                        if (p.CurrentTime >= p.AttackTime)
                        {
                            p.CurrentTime = 0;
                            p.State = ParticleState.Alive;
                        }
                        break;
                    case ParticleState.Alive:
                        p.LifeValue = (1f/(float) p.LifeTime)*(float)p.CurrentTime;
                        if (p.CurrentTime >= p.LifeTime)
                        {
                            p.CurrentTime = 0;
                            p.State = ParticleState.Decay;
                        }
                        break;
                    case ParticleState.Decay:
                        p.DecayValue = (1f/(float) p.DecayTime)*(float)p.CurrentTime;
                        if (p.CurrentTime >= p.DecayTime)
                        {
                            p.CurrentTime = 0;
                            p.State = ParticleState.Done;
                        }
                        break;
                    case ParticleState.Done:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                p.ParticleFunction(p);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Begin();
            foreach (Particle p in Particles)
            {
                if (p.State == ParticleState.Done) return;

                sb.Draw(_texParticles,
                        p.Position,
                        p.SourceRect, p.Color * p.Alpha, p.Rotation, new Vector2(p.SourceRect.Width / 2f, p.SourceRect.Height / 2f), p.Scale, SpriteEffects.None, 1);
            }
            sb.End();
        }
        public void Draw(SpriteBatch sb, Camera gameCamera, int depth)
        {
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, gameCamera.CameraMatrix);
            foreach (Particle p in Particles.Where(p => p.State != ParticleState.Done && p.Blend == ParticleBlend.Alpha && p.Depth == depth))
            {
                sb.Draw(_texParticles, 
                    p.Position,
                    p.SourceRect, p.Color * p.Alpha, p.Rotation, new Vector2(p.SourceRect.Width / 2f, p.SourceRect.Height / 2f), p.Scale, SpriteEffects.None, 1);
            }
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, null, null, null, gameCamera.CameraMatrix);
            foreach (Particle p in Particles.Where(p => p.State != ParticleState.Done && p.Blend == ParticleBlend.Additive && p.Depth == depth))
            {
                sb.Draw(_texParticles,
                    p.Position,
                    p.SourceRect, p.Color * p.Alpha, p.Rotation, new Vector2(p.SourceRect.Width / 2f, p.SourceRect.Height / 2f), p.Scale, SpriteEffects.None, 1);
            }
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, multiplicativeBlend, SamplerState.PointClamp, null, null, null, gameCamera.CameraMatrix);
            foreach (Particle p in Particles.Where(p => p.State != ParticleState.Done && p.Blend== ParticleBlend.Multiplicative && p.Depth == depth))
            {
                sb.Draw(_texParticles,
                    p.Position,
                    p.SourceRect, p.Color, p.Rotation, new Vector2(p.SourceRect.Width / 2f, p.SourceRect.Height / 2f), p.Scale, SpriteEffects.None, 1);
            }
            sb.End();
        }


        public void Add(Vector2 spawnPos, Vector2 velocity, double attackTime, double lifeTime, double decayTime, bool affectedbygravity, bool canCollide, Rectangle sourcerect, Color col, Action<Particle> particleFunc, float startScale, float startRot, int depth, ParticleBlend blend)
        {
            Particle p = Particles.FirstOrDefault(part => part.State == ParticleState.Done);
            if (p!=null)
            {
                p.Position = spawnPos;
                p.Velocity = velocity;
                p.Blend = blend;
                p.Depth = depth;
                p.AttackTime = attackTime;
                p.LifeTime = lifeTime;
                p.DecayTime = decayTime;
                p.AttackValue = 0f;
                p.LifeValue = 0f;
                p.DecayValue = 0f;
                p.AffectedByGravity = affectedbygravity;
                p.CanCollide = canCollide;
                p.SourceRect = sourcerect;
                p.Alpha = 1f;
                p.State = ParticleState.Attack;
                p.Scale = startScale;
                p.Rotation = startRot;
                p.Color = col;
                p.ParticleFunction = particleFunc;
                p.ParticleFunction(p);
            }

        }

        internal void Reset()
        {
            foreach (Particle p in Particles)
            {
                p.State = ParticleState.Done;
            }
        }
    }

    public static class ParticleFunctions
    {
        public static void FadeInOut(Particle p)
        {
            switch (p.State)
            {
                case ParticleState.Attack:
                    p.Alpha = p.AttackValue;
                    break;
                case ParticleState.Alive:
                    p.Alpha = 1f;
                    break;
                case ParticleState.Decay:
                    p.Alpha = 1f - p.DecayValue;
                    break;
            }
        }

        public static void Smoke(Particle p)
        {
            p.Scale += 0.001f;

            switch (p.State)
            {
                case ParticleState.Attack:
                    p.Alpha = p.AttackValue *0.3f;
                    break;
                case ParticleState.Alive:
                    if(p.Alpha<0.3f) p.Alpha += 0.001f;
                    break;
                case ParticleState.Decay:
                    p.Alpha = 0.3f - (p.DecayValue * 0.3f);
                    break;
            }
        }

        public static void PermaLight(Particle p)
        {
            switch (p.State)
            {
                case ParticleState.Attack:
                    p.Alpha = p.AttackValue;
                    break;
                case ParticleState.Alive:
                    p.Alpha = 1f;
                    p.CurrentTime = 0;
                    break;
                case ParticleState.Decay:
                    p.Alpha = 1f - p.DecayValue;
                    break;
            }
        }

        public static void FadeLight(Particle p)
        {
            switch (p.State)
            {
                case ParticleState.Attack:
                    p.Color = new Color(new Vector3(0.5f+(p.AttackValue*0.5f)));
                    break;
                case ParticleState.Alive:
                    p.Color = Color.White;
                    break;
                case ParticleState.Decay:
                    p.Color = new Color(new Vector3(1f - (p.DecayValue * 0.5f)));
                    break;
            }
        }
    }
}
