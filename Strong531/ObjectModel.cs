using System;
using System.Collections.Generic;


namespace Strong531
{
    public enum Lift
    {
        Press,
        Deadlift,
        Bench,
        Squat
    }

    public enum Week
    {
        OfFive = 0,
        OfThree,
        OfOne,
        Deload
    }

    public struct Set {
        public decimal Weight;
        public int Reps;
    }

    public class WeeklyPlan : Dictionary<Lift, Set[]>
    {

    }

    public class RepMax : Dictionary<Lift, decimal>
    {

    }


    public struct Cycle
    {
        public RepMax TrainingMax;
        public Dictionary<Week, WeeklyPlan> Weeks;
    }

    public struct Plan
    {
        public RepMax TrueOneRepMax ;
        public List<Cycle> Cycles;
    }
}
