using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;
using LDEngine.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;

namespace LDEngine.EntityPools
{
    class EntityPool
    {
        public static EntityPool Instance;

        public List<Entity> Entities;

        private Texture2D _spriteSheet;

        public EntityPool()
        {
            Instance = this;

            Entities = new List<Entity>();
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach(Entity e in Entities) e.Update(gameTime);

            Entities.RemoveAll(ent => !ent.Active);
        }
        public virtual void Update(GameTime gameTime, Map gameMap)
        {
            foreach (Entity e in Entities) e.Update(gameTime, gameMap);

            Entities.RemoveAll(ent => !ent.Active);
        }

        public virtual void HandleInput(InputState input)
        {
            foreach (Entity e in Entities) e.HandleInput(input);
        }

        public virtual void Draw(SpriteBatch sb, Camera camera)
        {
            sb.Begin(SpriteSortMode.Deferred, null,null,null,null,null,camera.CameraMatrix);
            foreach (Entity e in Entities) e.Draw(sb); 
            sb.End();
        }
    }
}
