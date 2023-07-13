using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseRace
{
    internal class Horese
    {
        public string name;

        public int dIstance;
        public float distance;


        private Random random = new Random();

        public void Run() 
        {
            distance += (1.0f + random.NextSingle()) * 10.0f;
        }

    }
}
