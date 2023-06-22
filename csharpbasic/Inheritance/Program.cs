namespace Inheritance
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Creature creature1 = new Creature();
            creature1.Breath();

            NPC npc1 = new NPC();
            npc1.Breath();
            npc1.Interation();


            //공변성 : 하위 타입을 기반 타입으로 참조할 수 있는 성질
            Creature creature2 = new NPC();
            //NPC npc2 = new Creature(); 
            // 인스턴스 맵버함수 호풀시 caller(this)에 인스턴스 참조를 넘겨주는데,
            // 기반 타입객체를 넘겨주면 할당되지 않은 맵버에 접근하게되므로 불가능
            creature2.Breath();
            //creature2.Interation(); 
            // 객체가 어떤 맵버 들을 가지고 있던지, 참조변수 타입에 따라서 맵버접근이 가능함.

            //npc2.Interation();
        }
    }
}