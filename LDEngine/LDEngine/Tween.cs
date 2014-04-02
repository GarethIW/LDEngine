using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimersAndTweens
{
    public delegate float TweenFunc(float progress);

    enum TweenState
    {
        Running,
        Paused,
        Finished
    }

    enum TweenDirection
    {
        Forward,
        Reverse
    }

    class Tween
    {
        public double CurrentTime;
        public double TargetTime;
        public bool Looping;
        public bool PingPong;
        public TweenDirection CurrentDirection;
        public TweenDirection InitialDirection;
        public string Name;

        public TweenState State;

        public float Value;

        private readonly Action<Tween> _callback;
        private readonly TweenFunc _tweenFunc;

        public Tween(string name, TweenFunc func, Action<Tween> callback, double time, bool pingpong, bool loop)
        {
            Name = name;
            _callback = callback;
            _tweenFunc = func;
            TargetTime = time;
            Looping = loop;
            PingPong = pingpong;

            CurrentTime = 0;
            CurrentDirection = TweenDirection.Forward;
            InitialDirection = TweenDirection.Forward;
            Value = 0f;

            if (PingPong) TargetTime /= 2; // If we're pingponging, halve the time so that TargetTime is one complete cycle
        }

        public Tween(string name, TweenFunc func, Action<Tween> callback, double time, bool pingpong, bool loop, TweenDirection initialDirection)
            : this(name, func, callback, time, pingpong, loop)
        {
            CurrentDirection = initialDirection;
            InitialDirection = initialDirection;
            if (initialDirection == TweenDirection.Reverse) CurrentTime = TargetTime;
        }

        public void Update(GameTime gameTime)
        {
            if(State!=TweenState.Running) return;

            switch (CurrentDirection)
            {
                case TweenDirection.Forward:
                    CurrentTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (CurrentTime >= TargetTime)
                    {
                        if (PingPong)
                        {
                            if (InitialDirection == TweenDirection.Reverse)
                            {
                                if(Looping) CurrentDirection = TweenDirection.Reverse;
                                else State = TweenState.Finished;
                            }
                            else CurrentDirection = TweenDirection.Reverse;
                            CurrentTime -= (CurrentTime - TargetTime);
                        }
                        else
                        {
                            if (Looping) CurrentTime = (CurrentTime-TargetTime);
                            else State = TweenState.Finished;
                        }
                    }
                    break;
                case TweenDirection.Reverse:
                    CurrentTime -= gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (CurrentTime <= 0)
                    {
                        if (PingPong)
                        {
                            if (InitialDirection == TweenDirection.Forward)
                            {
                                if(Looping) CurrentDirection = TweenDirection.Forward;
                                else State = TweenState.Finished;
                            }
                            else CurrentDirection = TweenDirection.Forward;
                            CurrentTime = -CurrentTime;
                        }
                        else
                        {
                            if (Looping) CurrentTime = TargetTime+CurrentTime;
                            else State = TweenState.Finished;
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            float pos = (1f/(float)TargetTime) * (float)CurrentTime;
            Value = _tweenFunc(pos);
            _callback(this);
        }

        public void Pause()
        {
            State = TweenState.Paused;
        }

        public void Resume()
        {
            State = TweenState.Running;
        }

        public void Kill()
        {
            State = TweenState.Finished;
        }
    }

    public static class TweenFuncs
    {
        public static readonly TweenFunc Linear = DoLinear;
        public static readonly TweenFunc QuadraticEaseIn = DoQuadraticEaseIn;
        public static readonly TweenFunc QuadraticEaseOut = DoQuadraticEaseOut;
        public static readonly TweenFunc QuadraticEaseInOut = DoQuadraticEaseInOut;
        public static readonly TweenFunc CubicEaseIn = DoCubicEaseIn;
        public static readonly TweenFunc CubicEaseOut = DoCubicEaseOut;
        public static readonly TweenFunc CubicEaseInOut = DoCubicEaseInOut;
        public static readonly TweenFunc QuarticEaseIn = DoQuarticEaseIn;
        public static readonly TweenFunc QuarticEaseOut = DoQuarticEaseOut;
        public static readonly TweenFunc QuarticEaseInOut = DoQuarticEaseInOut;
        public static readonly TweenFunc QuinticEaseIn = DoQuinticEaseIn;
        public static readonly TweenFunc QuinticEaseOut = DoQuinticEaseOut;
        public static readonly TweenFunc QuinticEaseInOut = DoQuinticEaseInOut;
        public static readonly TweenFunc SineEaseIn = DoSineEaseIn;
        public static readonly TweenFunc SineEaseOut = DoSineEaseOut;
        public static readonly TweenFunc SineEaseInOut = DoSineEaseInOut;
        public static readonly TweenFunc Bounce = DoBounce;
        public static readonly TweenFunc Elastic = DoElastic;

        private static float DoLinear(float progress) { return progress; }
        private static float DoQuadraticEaseIn(float progress) { return EaseInPower(progress, 2); }
        private static float DoQuadraticEaseOut(float progress) { return EaseOutPower(progress, 2); }
        private static float DoQuadraticEaseInOut(float progress) { return EaseInOutPower(progress, 2); }
        private static float DoCubicEaseIn(float progress) { return EaseInPower(progress, 3); }
        private static float DoCubicEaseOut(float progress) { return EaseOutPower(progress, 3); }
        private static float DoCubicEaseInOut(float progress) { return EaseInOutPower(progress, 3); }
        private static float DoQuarticEaseIn(float progress) { return EaseInPower(progress, 4); }
        private static float DoQuarticEaseOut(float progress) { return EaseOutPower(progress, 4); }
        private static float DoQuarticEaseInOut(float progress) { return EaseInOutPower(progress, 4); }
        private static float DoQuinticEaseIn(float progress) { return EaseInPower(progress, 5); }
        private static float DoQuinticEaseOut(float progress) { return EaseOutPower(progress, 5); }
        private static float DoQuinticEaseInOut(float progress) { return EaseInOutPower(progress, 5); }

        private static float DoSineEaseIn(float progress)
        {
            return (float)Math.Sin(progress * MathHelper.PiOver2 - MathHelper.PiOver2) + 1;
        }

        private static float DoSineEaseOut(float progress)
        {
            return (float)Math.Sin(progress * MathHelper.PiOver2);
        }

        private static float DoSineEaseInOut(float progress)
        {
            return (float)(Math.Sin(progress * MathHelper.Pi - MathHelper.PiOver2) + 1) / 2;
        }

        private static float DoBounce(float progress)
        {
            if ((progress /= 1f) < (1f / 2.75f))
                return (7.5625f * progress * progress);
            else if (progress < (2f / 2.75f))
                return (7.5625f * (progress -= (1.5f / 2.75f)) * progress + .75f);
            else if (progress < (2.5f / 2.75))
                return (7.5625f * (progress -= (2.25f / 2.75f)) * progress + .9375f);
            else
                return (7.5625f * (progress -= (2.625f / 2.75f)) * progress + .984375f);
        }

        public static float DoElastic(float progress)
        {
            if ((progress /= 1f) == 1)
                return 1f;

            float p = 0.3f;
            float s = p / 4f;

            return (1f * (float)Math.Pow(2f, -10f * progress) * (float)Math.Sin((progress * 1f - s) * (2f * (float)Math.PI) / p) + 1f);
        }

        private static float EaseInPower(float progress, int power)
        {
            return (float)Math.Pow(progress, power);
        }

        private static float EaseOutPower(float progress, int power)
        {
            int sign = power % 2 == 0 ? -1 : 1;
            return (float)(sign * (Math.Pow(progress - 1, power) + sign));
        }

        private static float EaseInOutPower(float progress, int power)
        {
            progress *= 2;
            if (progress < 1)
            {
                return (float)Math.Pow(progress, power) / 2f;
            }
            else
            {
                int sign = power % 2 == 0 ? -1 : 1;
                return (float)(sign / 2.0 * (Math.Pow(progress - 2, power) + sign * 2));
            }
        }

    }
}
