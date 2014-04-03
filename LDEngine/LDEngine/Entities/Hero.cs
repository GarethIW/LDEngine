using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;

namespace LDEngine.Entities
{
    class Hero : Entity
    {
        private const float GRAVITY = 0.1f;
        private SpriteAnim _idleAnim;
        private SpriteAnim _runAnim;

        public Hero(string name, Texture2D spritesheet, Vector2 position, Vector2 speed, Rectangle hitbox, Vector2 hitboxoffset) 
            : base(name, spritesheet, position, speed, hitbox, hitboxoffset)
        {
            _idleAnim = new SpriteAnim(spritesheet, 0, 1, 16,16,0, new Vector2(8f,16f));
            _runAnim = new SpriteAnim(spritesheet, 1, 7, 16, 16, 60, new Vector2(8f,16f));
            _runAnim.Play();
        }

        public override void Update(GameTime gameTime, Map gameMap)
        {
            //Speed.Y += GRAVITY;

            _idleAnim.Update(gameTime);
            _runAnim.Update(gameTime);

            base.Update(gameTime, gameMap);
        }

        public override void Draw(SpriteBatch sb)
        {
            //_idleAnim.Draw(sb, Position);
            _runAnim.Draw(sb,Position);
            base.Draw(sb);
        }
    }
}
