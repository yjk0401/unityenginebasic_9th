using System.Threading;

namespace HorseRace
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Horese[] horeses = new Horese[5];
            for (int i = 0; i < horeses.Length; i++)
            {
                horeses[i].name = $"{1 + i}번마";
            }
            string[] rankArray = new string[5];
            int currentGrade = 0;

            int sec = 0;


            while (currentGrade < horeses.Length)
            {


                // 말 달리는 내용
                for (int i = 0; i < horeses.Length; i++)
                {
                    if (horeses[i].GoalIn) 
                    {
                        Console.WriteLine("goal_in");
                    }
                    else
                    {

                    }
                }


                Thread.Sleep(1000);
            }


            Console.WriteLine("gameset");

            for (int i = 0; i < rankArray.Length; i++)
            {
                Console.WriteLine($"{rankArray[i]} 가 {i + 1} 등");
            }
        }
        static int GoalLIne = 200;

    }
}