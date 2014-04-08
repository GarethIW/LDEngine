using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;
using TimersAndTweens;

namespace LDEngine.Entities
{
    class Hero : Entity
    {
        public const int MAX_LIFE = 1000;

        public int Life = MAX_LIFE;
        public int FaceDir = 1;

        private const float GRAVITY = 0.03f;

        private SpriteAnim _idleAnim;
        private SpriteAnim _runAnim;

        private Color _tint = Color.White;

        public Hero(Texture2D spritesheet, Rectangle hitbox, Vector2 hitboxoffset) 
            : base(spritesheet, hitbox, hitboxoffset)
        {
            _idleAnim = new SpriteAnim(spritesheet, 0, 1, 16,16,0, new Vector2(8f,16f));
            _runAnim = new SpriteAnim(spritesheet, 1, 7, 16, 16, 60, new Vector2(8f,16f));
            _runAnim.Play();
        }

        public override void Update(GameTime gameTime, Map gameMap)
        {
            Speed.Y += GRAVITY;

            Speed.X = FaceDir;

            _idleAnim.Update(gameTime);
            _runAnim.Update(gameTime);

            Life--;
            if (Life <= 0) Active = false;

            CheckMapCollisions(gameMap);

            _tint = Color.White;

            base.Update(gameTime, gameMap);
        }

        public override void OnCollision(Entity collided, Rectangle intersect)
        {
            // Collides with another Hero
            if (collided.GetType() == typeof (Hero)) _tint = Color.Red;
                

            base.OnCollision(collided, intersect);
        }

        private void CheckMapCollisions(Map gameMap)
        {
            // Check downward collision
            if(Speed.Y>0)
                for (int x = HitBox.Left+2; x <= HitBox.Right-2; x += 2)
                {
                    bool? coll = gameMap.CheckCollision(new Vector2(x, HitBox.Bottom + Speed.Y));
                    if (coll.HasValue && coll.Value) Speed.Y = 0;
                }

            // Check left collision
            if(Speed.X<0)
                for (int y = HitBox.Top+2; y <= HitBox.Bottom-2; y += 2)
                {
                    bool? coll = gameMap.CheckCollision(new Vector2(HitBox.Left - Speed.X, y));
                    if (coll.HasValue && coll.Value)
                    {
                        Speed.X = 0;
                        FaceDir = 1;
                    }
                }

            // Check right collision
            if (Speed.X > 0)
                for (int y = HitBox.Top+2; y <= HitBox.Bottom-2; y += 2)
                {
                    bool? coll = gameMap.CheckCollision(new Vector2(HitBox.Right + Speed.X, y));
                    if (coll.HasValue && coll.Value)
                    {
                        Speed.X = 0;
                        FaceDir = -1;
                    }
                }
        }

        public override void Draw(SpriteBatch sb)
        {
            //_idleAnim.Draw(sb, Position);
            _runAnim.Draw(sb,Position,FaceDir==-1?SpriteEffects.FlipHorizontally:SpriteEffects.None, 1f, 0f, _tint);
            base.Draw(sb);
        }

        public override void Reset()
        {
            Life = MAX_LIFE;
            Speed = Vector2.Zero;

            base.Reset();
        }
    }
}
