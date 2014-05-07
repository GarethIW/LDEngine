using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;
using LDEngine.Entities;
using LDEngine.EntityPools;
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

        private EntityPool heroPool;
        private EntityPool rotBoxPool;

        private float textScale = 0f;

        private Hero followingHero;

        public ExampleGameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            //map = content.Load<Map>("map");   // Old XNB loader
            map = new Map(content, "map");

            MapObject spawn = ((MapObjectLayer) map.GetLayer("spawn")).Objects[0];

            camera = new Camera(ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight, map);
            camera.Target = new Vector2(ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight)/2f;

            heroPool = new EntityPool(100, 
                                      sheet => new Hero(sheet, new Rectangle(0, 0, 10, 10), new Vector2(0, -5)),
                                      content.Load<Texture2D>("testhero"));
            heroPool.BoxCollidesWith.Add(heroPool);

            rotBoxPool = new EntityPool(100,
                                     sheet => new RotBox(sheet, new Rectangle(0, 0, 32, 32), null, Vector2.Zero),
                                     ScreenManager.blankTexture);
            rotBoxPool.PolyCollidesWith.Add(rotBoxPool);

            rotBoxPool.Spawn(entity =>
                {
                    entity.Position = new Vector2(100,100);
                });
            rotBoxPool.Spawn(entity =>
            {
                entity.Position = new Vector2(140, 100);
            });

            particleController.LoadContent(content);

            // TimerController.Instance.Create("shake", () => camera.Shake(500, 2f), 3000, true);

            TweenController.Instance.Create("spintext", TweenFuncs.SineEaseInOut, (tween) =>
            {
                textScale = 0.8f+ (tween.Value *0.4f);
            }, 3000, true, true);

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
            camera.Update(gameTime);

            heroPool.Update(gameTime,map);
            rotBoxPool.Update(gameTime);

            // This stuff is all example - camera follows random Hero and zooms in when following
            //if (Helper.Random.Next(200) == 0)
            //{
            //    List<Entity> activeHeroes = heroPool.Entities.Where(hero => hero.Active).ToList();
            //    if (activeHeroes.Count > 0) followingHero = (Hero) activeHeroes[Helper.Random.Next(activeHeroes.Count)];
            //}
            //if (Helper.Random.Next(200) == 1 && followingHero != null) followingHero = null;

            //if (followingHero != null && followingHero.Active)
            //{
            //    camera.Target = followingHero.Position;
            //    if (camera.Zoom < 2.5f) camera.Zoom += 0.05f;
            //}
            //else
            //{
            //    camera.Target = new Vector2(ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight)/2f;
            //    if (camera.Zoom > 1f) camera.Zoom -= 0.05f;
            //}
            //////////////////



            particleController.Update(gameTime, map);

            if (Helper.Random.Next(100) == 0)
                heroPool.Spawn(entity =>
                {
                    entity.Position = new Vector2(Helper.Random.Next(ScreenManager.Game.RenderWidth-64)+32, 32);
                    ((Hero)entity).FaceDir = Helper.Random.Next(2) == 0 ? -1 : 1;
                });

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

            ScreenManager.Game.GraphicsDevice.Clear(new Color(75,75,75));

            sb.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, camera.CameraMatrix);
            map.DrawLayer(sb, "bg", camera);
            map.DrawLayer(sb, "fg", camera);
            sb.End();



            heroPool.Draw(sb, camera);

            rotBoxPool.Draw(sb, camera);

            particleController.Draw(ScreenManager.SpriteBatch, camera, 1);

            sb.Begin(SpriteSortMode.Deferred, null, null, null, null);
            sb.DrawString(ScreenManager.Font, "LD ENGINE", new Vector2(50, 20)+Vector2.One, Color.Black, 0f, ScreenManager.Font.MeasureString("LD ENGINE") / 2f, textScale, SpriteEffects.None, 1);
            sb.DrawString(ScreenManager.Font, "LD ENGINE", new Vector2(50, 20), Color.White, 0f, ScreenManager.Font.MeasureString("LD ENGINE") / 2f, textScale, SpriteEffects.None, 1);

            sb.End();

            

            ScreenManager.FadeBackBufferToBlack(1f-TransitionAlpha);

            base.Draw(gameTime);
        }
    }
}
