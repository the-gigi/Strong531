using System;
using Strong531;

namespace Strong531ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var repMax = new RepMax();
            repMax[Lift.Press] = 100;
            repMax[Lift.Bench] = 200;
            repMax[Lift.Squat] = 300;
            repMax[Lift.Deadlift] = 400;
            
            var plan = PlanMaker.MakePlan(repMax, 2);
            Console.Out.WriteLine(plan.ToString());
        }
    }
}
