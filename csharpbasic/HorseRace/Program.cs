using System.Threading;

namespace HorseRace
{
    internal class Program
    {
        const float Goal_LIne = 200.0f;
        const int Total_Horse = 5;

        static void Main(string[] args)
        {
            
            Horese[] horeses = new Horese[5];
            for (int i = 0; i < horeses.Length; i++)
            {
                horeses[i] = new Horese();
                horeses[i].name = $"{1 + i}번마";
            }
            string[] rankArray = new string[5];
            int currentGrade = 0;
            int elapsedSec = 0;


            while (currentGrade < Total_Horse)
            {
                Console.WriteLine($"======================================================{elapsedSec++} 초 경과======================================================");

                for (int i = 0; i < Total_Horse; i++)
                {
                    if (horeses[i].distance < Goal_LIne) 
                    {
                        horeses[i].Run();
                        Console.WriteLine($"{horeses[i].name} 이(가) 달린 거리 : {horeses[i].distance}");

                        if (horeses[i].distance >= Goal_LIne)
                        {
                            rankArray[currentGrade] = horeses[i].name;
                            currentGrade++;
                        }

                        if (currentGrade >= Total_Horse)
                        {
                            Console.WriteLine($"============================경기 끝=============================================");

                        }


                    }
                    
                }


                Thread.Sleep(1000);
                elapsedSec++;
            }


            Console.WriteLine("gameset");

            for (int i = 0; i < rankArray.Length; i++)
            {
                Console.WriteLine($"{rankArray[i]} {i + 1}착");
            }
        }
    }
}