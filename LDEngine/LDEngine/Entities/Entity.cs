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
        public Vector2 Position;
        public Vector2 Speed;
        public float Rotation;
        public Rectangle HitBox;
        public List<Vector2> HitPolyPoints;
        public Texture2D SpriteSheet;
        public bool Active;

        private List<Vector2> _normalPolyPoints; 
        private Vector2 _hitboxOffset;

        public Entity(Texture2D spritesheet, Rectangle hitbox, List<Vector2> hitPolyPoints, Vector2 hitboxoffset)
        {
            HitBox = hitbox;
            SpriteSheet = spritesheet;
            Active = false;

            _hitboxOffset = hitboxoffset;

            if (hitPolyPoints != null)
                _normalPolyPoints = hitPolyPoints;
            else
                _normalPolyPoints = new List<Vector2>()
                {
                    new Vector2(-(HitBox.Width / 2), -(HitBox.Height / 2)),
                    new Vector2((HitBox.Width / 2), -(HitBox.Height / 2)),
                    new Vector2((HitBox.Width / 2), (HitBox.Height / 2)),
                    new Vector2(-(HitBox.Width / 2), (HitBox.Height / 2)),
                };

            HitPolyPoints = new List<Vector2>();
            foreach(var pp in _normalPolyPoints) HitPolyPoints.Add(pp);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!Active) return;

            Position += Speed;

            HitBox.Location = new Point((int)Position.X,(int)Position.Y) + new Point(-(HitBox.Width/2),-(HitBox.Height/2)) + new Point((int)_hitboxOffset.X, (int)_hitboxOffset.Y);

            for (int index = 0; index < _normalPolyPoints.Count; index++)
            {
                HitPolyPoints[index] = _normalPolyPoints[index];
                HitPolyPoints[index] = Vector2.Transform(_normalPolyPoints[index], Matrix.CreateRotationZ(Rotation));
                HitPolyPoints[index] = HitPolyPoints[index] + _hitboxOffset + Position;
            }
        }
        public virtual void Update(GameTime gameTime, Map gameMap)
        {
            Update(gameTime);
        }

        public virtual void OnBoxCollision(Entity collided, Rectangle intersect)
        {}
        public virtual void OnPolyCollision(Entity collided)
        { }

        public virtual void HandleInput(InputState input)
        {
            
        }

        public virtual void Draw(SpriteBatch sb)
        {
            if (!Active) return;
        }

        // Reset values to defaults when an entity is spawned from a pool
        public virtual void Reset()
        {
            
        }
    }
}
