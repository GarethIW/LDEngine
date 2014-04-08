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
        public List<object> CollidesWith; 

        private int _maxEntities;

        public EntityPool(int maxEntities, Func<Texture2D, Entity> createFunc, Texture2D spriteSheet)
        {
            Instance = this;

            _maxEntities = maxEntities;

            Entities = new List<Entity>();
            for(int i=0;i<maxEntities;i++) Entities.Add(createFunc(spriteSheet));

            CollidesWith = new List<object>();
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (Entity e in Entities.Where(ent => ent.Active)) e.Update(gameTime);
        }
        public virtual void Update(GameTime gameTime, Map gameMap)
        {
            foreach (Entity e in Entities.Where(ent => ent.Active))
            {
                e.Update(gameTime, gameMap);
                CheckCollisions(e);
            }
        }

        public virtual void HandleInput(InputState input)
        {
            foreach (Entity e in Entities.Where(ent => ent.Active)) e.HandleInput(input);
        }

        public virtual void Draw(SpriteBatch sb, Camera camera)
        {
            sb.Begin(SpriteSortMode.Deferred, null,SamplerState.PointClamp,null,null,null,camera.CameraMatrix);
            foreach (Entity e in Entities.Where(ent=>ent.Active)) e.Draw(sb); 
            sb.End();
        }

        public Entity Spawn(Action<Entity> spawnFunc)
        {
            // Find an entity in our pool that isn't active
            Entity retEntity = Entities.FirstOrDefault(ent => !ent.Active);

            // If we don't have a spare entitiy, return null
            if (retEntity == null) return null;

            // First, run our reset function to reset required default values (might be speed or life etc.)
            retEntity.Reset();

            // Then, run the spawn function to set new values (position, etc)
            spawnFunc(retEntity);

            // Make it alive!
            retEntity.Active = true;

            return retEntity;
        }

       

        private void CheckCollisions(Entity e)
        {
            foreach (object o in CollidesWith)
            {
                if (o is EntityPool)
                {
                    foreach (Entity collEnt in ((EntityPool)o).Entities)
                    {
                        if (!collEnt.Active) continue;
                        if (collEnt == e) continue;

                        Rectangle intersect = Rectangle.Intersect(e.HitBox, collEnt.HitBox);
                        if (intersect.IsEmpty) continue;

                        e.OnCollision(collEnt, intersect);
                    }
                }

                if (o is Entity)
                {
                    Entity collEnt = (Entity)o;

                    if (!collEnt.Active) continue;
                    if (collEnt == e) continue;

                    Rectangle intersect = Rectangle.Intersect(e.HitBox, collEnt.HitBox);
                    if (intersect.IsEmpty) continue;

                    e.OnCollision(collEnt, intersect);
                    collEnt.OnCollision(e,intersect);
                }
            }
        }
    }
}
