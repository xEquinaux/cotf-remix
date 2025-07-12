using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cotf.Base;
using cotf.World;
using cotf.Collections;

namespace cotf
{
    public class Condition
    {
        public Condition(Skill reward, int count)
        {
            this.reward = reward;
            this.count = count;
        }
        public bool satisfied;
        public int count;
        public Skill reward;
        public static Condition GetCondition(short type)
        {
            switch (type)
            {
                case SkillID.None:
                    return new Condition(Skill.SetActive(SkillID.Melee), 1);
                case SkillID.FireBall:
                    return new Condition(Skill.SetActive(SkillID.FireBall), 10);
                default:
                    return new Condition(Skill.None, 0);
            }
        }

        public bool Count(int num)
        {
            return num >= count;
        }
    }
}
