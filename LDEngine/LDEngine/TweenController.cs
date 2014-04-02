using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimersAndTweens
{
    class TweenController
    {
        public static TweenController Instance;

        public List<Tween> Tweens = new List<Tween>();

        public TweenController()
        {
            Instance = this;
        }

        public void Update(GameTime gameTime)
        {
            foreach(Tween t in Tweens) t.Update(gameTime);

            Tweens.RemoveAll(t => t.State == TweenState.Finished);
        }

        public Tween Create(string name, TweenFunc func, Action<Tween> callback, double time, bool pingpong, bool loop)
        {
            Tween t = new Tween(name, func, callback, time, pingpong, loop);
            Tweens.Add(t);

            return t;
        }

        public Tween Create(string name, TweenFunc func, Action<Tween> callback, double time, bool pingpong, bool loop, TweenDirection initialDirection)
        {
            Tween t = new Tween(name, func, callback, time, pingpong, loop, initialDirection);
            Tweens.Add(t);

            return t;
        }

        public Tween Get(string name)
        {
            return Tweens.FirstOrDefault(t => t.Name == name);
        }
    }
}
