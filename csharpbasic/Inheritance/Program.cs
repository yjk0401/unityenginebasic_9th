using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace Inheritance
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Knight knight = new Knight();
            knight.hp = 10;
            HpBar knightHpBar = new HpBar();
            IDamageable damageable = knight;

            knight.onHpChanged += knightHpBar.Refresh; 
            knight.onHpChanged += knightHpBar.Refresh;
            knight.onHpChanged += knightHpBar.Refresh;
            //knight.onHpChanged = knightHpBar.Refresh;
            knight.onHpChanged += (value) =>
            {
                Console.WriteLine(value);
            };
            // 람다식 표현
            // 컴파일러가 알아서 유추할 수 있는 내용을 모두 지운 뒤, => 이 기호로 람다식이라는 명사만 해주면 됌.

            while (true)
            {
                knight.hp -= 1;
                Thread.Sleep(100);
            }



            //Creature creature1 = new Creature(); // 추상클래스는 인스턴스화 불가능
            //creature1.Breath();

            NPC npc1 = new NPC();
            npc1.Breath();
            npc1.Interation();

            //공변성 : 하위 타입을 기반 타입으로 참조할 수 있는 성질
            Creature creature2 = new NPC();
            //NPC npc2 = new Creature(); 
            // 인스턴스 맵버함수 호풀시 caller(this)에 인스턴스 참조를 넘겨주는데,
            // 기반 타입객체를 넘겨주면 할당되지 않은 맵버에 접근하게되므로 불가능
            creature2.Breath();
            ((NPC)creature2).Interation();

            // is : 왼쪽 객체가 오른쪽 타입으로 캐스팅이 가능하다면 true, 아니면 false 를 반환하는 키워드
            if (creature2 is NPC)
            {
                ((NPC)creature2).Interation();
            }

            // as : 왼쪽 객체를 오른쪽 타입으로 캐스팅 시도하고 성공시 캐스팅된 타입참조반환, 실패시 null 반환
            NPC tmpNPc = creature2 as NPC;
            if (tmpNPc != null)
            {
                tmpNPc.Interation();
            }

            //creature2.Interation(); 
            // 객체가 어떤 맵버 들을 가지고 있던지, 참조변수 타입에 따라서 맵버접근이 가능함.

            //npc2.Interation();
            Character[] characters = { new Knight(), new Magician(), new SwordMan() };
            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].UniqueSkill();

                if (characters[i] is Knight)
                    Console.WriteLine("기사 발견!");

                if (characters[i] is Magician)
                    Console.WriteLine("마법사 발견!");

                if (characters[i] is SwordMan)
                    Console.WriteLine("전사 발견!");
            }


        }

        public void Test() 
        {
            PrintSomething(3);
            PrintSomething<double>(3); 
            PrintSomething<long>(3);
            // Generic 타입 자체가 코드영역에 할당되는게 아니고
            // (런타임중에 제네릭 참조해서 새로운 함수를 만들어서 스택영역에 쌓는게 아님)
            // 컴파일  타임에 컴파일러가 사용되고 있는 형태를 참수로 새로 만들어서 코드영역에 할당함
            // ex) Something<T>() 함수가 있고, Something<int>() 라는 함수호출을 어딘가에서 하고있으면
            // 컴파일타임에 Something<int>() 를 만든다.

            Dummy<int> dummy1 = new Dummy<int>();
            Dummy<float> dummy2 = new Dummy<float>();
            Dummy<Knight> dummy3 = new Dummy<Knight>();
            Dummy<IDamageable> dummy4 = new Dummy<IDamageable>();
        }


        public void PrintSomething(int value) 
        {
            Console.WriteLine(value);
        }

        public void PrintSomething(float value) 
        {
            Console.WriteLine(value);
        }  

        public void PrintSomething(string value) 
        {
            Console.WriteLine(value);
        }

        public void PrintSomething<T>(T value) 
        {
            Console.WriteLine(value);
        }

    }

    // 제네릭 클래스
    public class Dummy<T> 
    {
        public T value;

        public void SaySomething() 
        {
            T value1 = value;
            What<T>();
            
        }

        public void What<T>() 
        {
        
        }
    }

}