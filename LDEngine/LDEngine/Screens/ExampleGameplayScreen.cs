using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;
using LDEngine.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;
using TimersAndTweens;

namespace LDEngine.Screens
{
    class ExampleGameplayScreen : GameScreen
    {
        private Camera camera;
        private Map map;

        private ParticleController particleController = new ParticleController();


        private Texture2D heroSheet;
        private Hero hero;

        private float textRot = 0f;

        public ExampleGameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            map = content.Load<Map>("map");
            MapObject spawn = ((MapObjectLayer) map.GetLayer("spawn")).Objects[0];

            camera = new Camera(ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight, map);
            camera.Target = new Vector2(ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight)/2f;

            heroSheet = content.Load<Texture2D>("testhero");
            hero = new Hero("hero", heroSheet, new Vector2(spawn.Location.Center.X,spawn.Location.Bottom), Vector2.Zero, new Rectangle(0,0,16,16), new Vector2(0,-8f));

            particleController.LoadContent(content);

            TimerController.Instance.Create("shake", () => camera.Shake(500, 2f), 3000, true);

            TweenController.Instance.Create("spintext", TweenFuncs.Linear, (tween) =>
            {
                textRot = MathHelper.TwoPi * tween.Value;
            }, 3000, false, true);

            //// More crazy tween examples
            //TweenController.Instance.Create("spincam", TweenFuncs.Linear, (tween) =>
            //{
            //    camera.Rotation = MathHelper.TwoPi * tween.Value;
            //}, 10000, false, true, TweenDirection.Reverse);

            //TweenController.Instance.Create("zoomcam", TweenFuncs.Bounce, (tween) =>
            //{
            //    camera.Zoom = 1f + tween.Value;
            //}, 3000, true, true);

            particleController.Add(new Vector2(195, 150),
                                  Vector2.Zero,
                                  0, 1000, 0,
                                  false, false,
                                  new Rectangle(18, 0, 100, 100),
                                  Color.White,
                                  ParticleFunctions.PermaLight,
                                  1f, 0f,
                                  1, ParticleBlend.Multiplicative);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (!ScreenManager.Game.IsActive) return;

            camera.Target = hero.Position;
            camera.Update(gameTime);
            hero.Update(gameTime, map);
            particleController.Update(gameTime, map);

            particleController.Add(new Vector2(17, 40),
                                   new Vector2(Helper.RandomFloat(2f), -1.5f),
                                   100, 3000, 1000,
                                   true, true,
                                   new Rectangle(0, 0, 2, 2),
                                   new Color(new Vector3(1f, 0f, 0f) * (0.25f + Helper.RandomFloat(0.5f))),
                                   ParticleFunctions.FadeInOut,
                                   1f, 0f,
                                   1, ParticleBlend.Alpha);

            particleController.Add(new Vector2(150, 176),
                                   new Vector2(-0.05f + Helper.RandomFloat(0.1f), -0.1f),
                                   1000, Helper.Random.NextDouble() * 3000, Helper.Random.NextDouble() * 3000,
                                   false, false,
                                   new Rectangle(0, 0, 16, 16),
                                   new Color(new Vector3(1f) * (0.25f + Helper.RandomFloat(0.5f))),
                                   ParticleFunctions.Smoke,
                                   0.1f, 0f,
                                   1, ParticleBlend.Additive);

            particleController.Add(new Vector2(250, 50),
                                   new Vector2(-1f + Helper.RandomFloat(2f), -1f + Helper.RandomFloat(2f)),
                                   100, 500, 1000,
                                   false, false,
                                   new Rectangle(0, 0, 16, 16),
                                   Color.White,
                                   ParticleFunctions.FadeLight,
                                   Helper.RandomFloat(0.5f), 0f,
                                   1, ParticleBlend.Multiplicative);


            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 center = new Vector2(ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight) / 2f;
            SpriteBatch sb = ScreenManager.SpriteBatch;

            sb.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, camera.CameraMatrix);
            map.DrawLayer(sb, "bg", camera);
            map.DrawLayer(sb, "fg", camera);
            hero.Draw(sb);
            sb.End();

            particleController.Draw(ScreenManager.SpriteBatch, camera, 1);

            sb.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);
            sb.DrawString(ScreenManager.Font, "LD ENGINE", center, Color.White, textRot, ScreenManager.Font.MeasureString("LDENGINE") / 2f, 1f, SpriteEffects.None, 1);
            sb.End();

            ScreenManager.FadeBackBufferToBlack(1f-TransitionAlpha);

            base.Draw(gameTime);
        }
    }
}
