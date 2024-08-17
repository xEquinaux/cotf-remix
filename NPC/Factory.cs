using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace cotf.NPC
{
	public class Factory
	{
		static int rand = (int)(DateTime.Now.Second / 0.6f);
		static long GetSeed()
		{
			return DateTime.Now.Ticks;
		}
		
		public Factory()
		{
		}
		
		public class NpcF
		{
			public NpcF() { }
			static Mutation mutation = new Mutation(true);
			public static Npc Mutate(Npc n)
			{
				var array = mutation.GetNumber(Main.rand.Next(10));
				decimal grade = Math.Max(0.1M, Main.FloorNum / (array[Main.rand.Next(8)] / 100M));
				if (grade == 0)
					grade = 0.1M;
				n.lifeMax = (int)(n.lifeMax * grade);
				n.life = n.lifeMax;
				n.knockBack = (float)((decimal)n.knockBack * grade);
				n.speed = Math.Max(0.1f, (float)((decimal)n.speed * grade) - 1);
				n.damage = (int)(n.damage * grade);
				return n;
			}
		}

		public class Mutation
		{
			/// <summary>
			/// Constructs a mutation.
			/// </summary>
			/// <param name="init">Whether or not to initialize the Step variable immediately or not.</param>
			public Mutation(bool init)
			{
				if (init)
				Step = Generate();
			}
			/// <summary>
			/// Steps of 10 values for generating behaviors.
			/// </summary>
			public long[] Step;
			/// <summary>
			/// An array of 8 bytes based on one of the 10 values in Step.
			/// </summary>
			/// <param name="index">An integer index between 0 and 9.</param>
			/// <returns></returns>
			public byte[] GetNumber(int index)
			{
				if (Step.Length == 0)
				{
					Step = Generate();
				}
				if (index > 9) index = 9;
				if (index < 0) index = 0;
				var array = BitConverter.GetBytes(Step[index]);
				return new[]
				{
					array[0],
					array[1],
					array[2],
					array[3],
					array[4],
					array[5],
					array[6],
					array[7]
				};
			}
			/// <summary>
			/// Initializer for the Step variable.
			/// </summary>
			public long[] Generate()
			{
				Step = new long[10];
				for (int i = 0; i < Step.Length; i++)
				{
					Task.WhenAll(Task.Delay(Factory.rand));
					Step[i] = Factory.GetSeed();
				}
				return Step;
			}
		}
	}
}
