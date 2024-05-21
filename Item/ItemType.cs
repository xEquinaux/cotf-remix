using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cotf
{
    public enum ItemType : byte
    {
        None = 0,
        Broadsword = 1,
        Torch = 2,
        Spear = 3,
        Purse = 4,
        IronCoin = 5,
        Coin = 6,
        Scroll = 10,
        Wand = 11,
        Potion = 12,
        Bow = 13,        // fast
        Mace = 14,       // swing motion effect
        Club = 15,       // forward effect
        Dagger = 16,     // quick motion effect
        Longsword = 17,  // bigger swing
        Shortsword = 18, // fast
                         // | Armor
        Belt = 19,       // v
        BodyArmor = 20,  // Mail
        Gauntlets = 21,  // Gloves
        Cape = 22,
        Helm = 23,       // Coif
        Boots = 24,
        Ring = 25,
        Necklace = 27,
        Shirt = 28,      //  | Extra? 
        Coif = 29,       //  v
        Cap = 30,
        Leggings = 31,
        Wrists = 32,
        CrossBow = 33    // slow
    }
}
