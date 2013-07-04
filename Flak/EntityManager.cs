using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Flak
{
    class EntityManager
    {
        List<Entity> NewEntities { get; set; }
        List<Entity> RemovedEntities { get; set; }
        List<Entity> Entities { get; set; }

        public EntityManager()
        {
            Entities = new List<Entity>();
            NewEntities = new List<Entity>();
            RemovedEntities = new List<Entity>();
        }

        public void Update()
        {
            Entities.AddRange(NewEntities);
            NewEntities.Clear();

            foreach (Entity entity in RemovedEntities)
            {
                entity.Dispose();
                Entities.Remove(entity);
            }
            RemovedEntities.Clear();

            foreach (Entity entity in Entities)
                entity.Update();

            ManageCollisions();
        }

        public void Draw(SpriteBatch spritebatch)
        {
            foreach (Entity entity in Entities)
            {
                entity.Draw(spritebatch);
                spritebatch.Draw();
            }
        }

        public void ManageCollisions()
        {
            for (int i = 0; i < Entities.Count; i++)
                for (int j = i + 1; j < Entities.Count; j++)
                {
                    //use bounding circle collision
                    Entity a = Entities[i];
                    Entity b = Entities[j];

                    if (a.DoesCollide(b))
                    {
                        a.HandleCollision(b);
                        b.HandleCollision(a);
                    }
                }
        }

        public void Add(Entity entity)
        {
            NewEntities.Add(entity);
        }

        public void Remove(Entity entity)
        {
            RemovedEntities.Add(entity);
        }

        public void Clear()
        {
            Entities.Clear();
        }
    }
}
