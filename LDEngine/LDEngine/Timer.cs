using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimersAndTweens
{
    enum TimerState
    {
        Running,
        Paused,
        Finished
    }

    class Timer
    {
        public double CurrentTime;
        public double TargetTime;
        public bool Looping;
        public string Name;

        public TimerState State;

        private readonly Action _callback;

        public Timer(string name, Action callback, double time, bool loop)
        {
            Name = name;
            _callback = callback;
            TargetTime = time;
            Looping = loop;

            CurrentTime = 0;
        }

        public void Update(GameTime gameTime)
        {
            if(State!=TimerState.Running) return;

            CurrentTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (CurrentTime >= TargetTime)
            {
                CurrentTime = 0;

                if (!Looping) State = TimerState.Finished;

                _callback();
            }
        }

        public void Pause()
        {
            State = TimerState.Paused;
        }

        public void Resume()
        {
            State = TimerState.Running;
        }

        public void Reset()
        {
            CurrentTime = 0;
        }

        public void Kill()
        {
            State = TimerState.Finished;
        }
    }
}
