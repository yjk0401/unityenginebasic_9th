namespace Statement_if
{
    internal class Program
    {
        static void Main(string[] args)
        {

            bool conditional1 = true;
            bool conditional2 = false;


            if (conditional1)
            {
                Console.WriteLine("조건 1이 참");
            }
            else if (conditional2) 
            {
                Console.WriteLine("위 조건들이 모두 거짓이면서 조건 2가 참");
            }
            else 
            {
                Console.WriteLine("위 조건들이 모두 거짓");            
            }
        }
    }
}