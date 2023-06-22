using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inheritance
{
    // 클래스 상속
    // 클래스이름 : 부모 클래스 이름
    internal class NPC : Creature
    {
        public int Id;

        public void Interation() 
        {
            Console.WriteLine("NPC와 상호작용 시작.");
        }

        // override : 재정의 키워드
        // 가상/추상 맵버를 재성의 할 때 사용
        public override void Breath()
        {
            // base 키워드
            // 기반 타입 찹조 키워드
            Console.Write("NPC가 ");
            base.Breath();
        }
    }
}
