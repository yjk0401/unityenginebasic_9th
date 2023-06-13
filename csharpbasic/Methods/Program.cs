using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Function vs Meshod
// Fun ction : 연산을 수행할 수 있는 기능
// Method : 격체/클래스를 단위로 호출되는 Function
// Method 가 Function 에 포함됨

// 함수 형태 3가지
// 함수 선언 -> f(x) 라는게 있다 라고 명시하는 것
// 함수 정의 -> f(x) = ax + b 라고 연산부분을 명시하는 것
// 함수 호출 -> f(2)  처럼 변수자리에 인수를 넣고 함수 이름을 씀

// f(a, b) = a + b
namespace Methods
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int sum = Sum(1, 2);
            Console.WriteLine(sum);
        }
        static int Sum(int a, int b)
        { 
            return a + b;
        }


    }
}
