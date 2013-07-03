using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flak
{
    public class EntityManager
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
                Entities.Remove(entity);
            NewEntities.Clear();

            foreach (Entity entity in Entities)
                entity.Update();
        }

        public void Draw(SpriteBatch spritebatch)
        {


            foreach (Entity entity in Entities)
            {
                spritebatch.Begin();
                entity.Draw(spritebatch);

                spritebatch.End();
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
