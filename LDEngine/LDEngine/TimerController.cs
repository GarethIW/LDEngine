using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimersAndTweens
{
    class TimerController
    {
        public static TimerController Instance;

        public List<Timer> Timers = new List<Timer>();

        public TimerController()
        {
            Instance = this;
        }

        public void Update(GameTime gameTime)
        {
            foreach(Timer t in Timers) t.Update(gameTime);

            Timers.RemoveAll(t => t.State == TimerState.Finished);
        }

        public Timer Create(string name, Action callback, double time, bool loop)
        {
            Timer t = new Timer(name, callback, time, loop);
            Timers.Add(t);

            return t;
        }

        public Timer Get(string name)
        {
            return Timers.FirstOrDefault(t => t.Name == name);
        }
    }
}
