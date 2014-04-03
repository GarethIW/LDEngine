using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;

namespace LDEngine.Entities
{
    class Entity
    {
        public string Name;
        public Vector2 Position;
        public Vector2 Speed;
        public Rectangle HitBox;
        public Texture2D SpriteSheet;
        public bool Active;

        private Vector2 _hitboxOffset;

        public Entity(string name, Texture2D spritesheet, Vector2 position, Vector2 speed, Rectangle hitbox, Vector2 hitboxoffset)
        {
            Name = name;
            Position = position;
            Speed = speed;
            HitBox = hitbox;
            SpriteSheet = spritesheet;
            Active = true;

            _hitboxOffset = hitboxoffset;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!Active) return;

            Position += Speed;

            HitBox.Location = new Point((int)Position.X,(int)Position.Y) + new Point(-(HitBox.Width/2),-(HitBox.Height/2)) + new Point((int)_hitboxOffset.X, (int)_hitboxOffset.Y);
        }
        public virtual void Update(GameTime gameTime, Map gameMap)
        {
            Update(gameTime);
        }

        public virtual void HandleInput(InputState input)
        {
            
        }

        public virtual void Draw(SpriteBatch sb)
        {
            if (!Active) return;
        }
    }
}
