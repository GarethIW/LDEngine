using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TimersAndTweens;
using Timer = TimersAndTweens.Timer;

namespace LDEngine.Entities
{
    class RotBox : Entity
    {
        private Color _tint = Color.White;

        public RotBox(Texture2D spritesheet, Rectangle hitbox, List<Vector2> hitPolyPoints, Vector2 hitboxoffset) 
            : base(spritesheet, hitbox, hitPolyPoints, hitboxoffset)
        {
            TimerController.Instance.Create("", () => { Rotation += 0.1f; }, 100, true);
        }

        public override void Update(GameTime gameTime)
        {
            _tint = Color.White;

            base.Update(gameTime);
        }

        public override void OnPolyCollision(Entity collided)
        {
            _tint = Color.Red;

            base.OnPolyCollision(collided);
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(SpriteSheet, Position, null, _tint, Rotation, new Vector2(SpriteSheet.Width, SpriteSheet.Height)/2, 2f, SpriteEffects.None, 0);
            base.Draw(sb);
        }
    }
}
