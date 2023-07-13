using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inheritance
{
    // abstract : 추상키워드
    // 추상화용도로 사용한다는 것을 명시하는 키워드, 동적할당 불가능함.
    internal abstract class Creature
    {
        public int DnaCode;

        // virtual : 가상키워드
        // 함수를 자식이 재정의할 수 있도록 하는 키워드
        // - override : 재정의 키워드
        public virtual void Breath() 
        {
            Console.WriteLine("숨쉬기");        
        }
    }
}
