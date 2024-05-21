using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cotf.Base
{
    public interface ITrait
    {
        public abstract int Quality(Item item);
        public abstract void Cursed(Item item, bool cursed);
        public abstract void Enchanted(Item item, bool enchanted);
        public abstract void RemoveEffect(Player player);
            //  With things directly effecting player,
            //  should have buffer variables to compare with:
            //  A default player variables class perhaps
        public abstract string GetName(Item item);
            //return item.name = $"{prefix} {item.Name} {suffix}";
    }
    public interface IAffix
    {
        public abstract void Apply(ITrait trait);
        public abstract void Effect(Item item);
        public abstract void Effect(Player player);
    }
    public class Prefix : ITrait, IAffix
    {
        public virtual void Apply(ITrait trait)
        {
        }

        public virtual void Effect(Item item)
        {
        }

        public virtual void Effect(Player player)
        {
        }

        public virtual void Cursed(Item item, bool cursed)
        {
        }
        
        public virtual void Enchanted(Item item, bool enchanted)
        {
        }

        public virtual string GetName(Item item)
        {
            return item.name;
        }

        public virtual int Quality(Item item)
        {
            return 0;
        }

        public virtual void RemoveEffect(Player player)
        {
        }
    }
    public class Suffix : ITrait, IAffix
    {
        public virtual void Apply(ITrait trait)
        {
        }

        public virtual void Effect(Item item)
        {
        }

        public virtual void Effect(Player player)
        {
        }

        public virtual void Cursed(Item item, bool cursed)
        {
        }

        public virtual void Enchanted(Item item, bool enchanted)
        {
        }

        public virtual string GetName(Item item)
        {
            return item.name;
        }

        public virtual int Quality(Item item)
        {
            return 0;
        }

        public virtual void RemoveEffect(Player player)
        {
        }
    }
}
