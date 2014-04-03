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
    class GameplayScreen : GameScreen
    {
        private float textRot = 0f;
        private bool squareBlink = false;

        private Camera camera;
        private Map map;

        private Texture2D heroSheet;

        private Hero hero;

        public GameplayScreen()
        {
            
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

            //TimerController.Instance.Create("blinksquare", () => { squareBlink = !squareBlink; }, 100, true);

            //TweenController.Instance.Create("spintext", TweenFuncs.Linear, (tween) =>
            //{
            //    textRot = MathHelper.TwoPi*tween.Value;
            //}, 3000, false, true);

            //TweenController.Instance.Create("spincam", TweenFuncs.Linear, (tween) =>
            //{
            //    camera.Rotation = MathHelper.TwoPi * tween.Value;
            //}, 10000, false, true, TweenDirection.Reverse);

            //TweenController.Instance.Create("zoomcam", TweenFuncs.Bounce, (tween) =>
            //{
            //    camera.Zoom = 1f + tween.Value;
            //}, 3000, true, true);

            //TweenController.Instance.Create("movecam", TweenFuncs.Linear, (tween) =>
            //{
            //    camera.Target = new Vector2(ScreenManager.Game.RenderWidth*tween.Value, ScreenManager.Game.RenderHeight / 2);
            //}, 10000, true, true);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            camera.Target = hero.Position;
            camera.Update(gameTime);
            hero.Update(gameTime, map);
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 center = new Vector2(ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight)/2f;

            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, null,SamplerState.PointClamp,null,null,null,camera.CameraMatrix);
            map.DrawLayer(ScreenManager.SpriteBatch, "fg", camera);
            hero.Draw(ScreenManager.SpriteBatch);
            ScreenManager.SpriteBatch.End();

            ScreenManager.SpriteBatch.Begin();
            //if(!squareBlink) ScreenManager.SpriteBatch.Draw(ScreenManager.blankTexture,new Vector2(20,20),Color.White);
            //ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, "TRIPPIN' BALLS", center, Color.White, textRot, ScreenManager.Font.MeasureString("TRIPPIN' BALLS") / 2f, 1f, SpriteEffects.None, 1);
            ScreenManager.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
