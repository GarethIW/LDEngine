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

}
