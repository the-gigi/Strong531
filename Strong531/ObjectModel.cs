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
        public WeeklyPlan()
        {
            foreach (Lift lift in Enum.GetValues(typeof(Lift)))
            {
                this[lift] = new Set[]{};
            }
        }
    }

    public class RepMax : Dictionary<Lift, decimal>
    {
        public RepMax()
        {
        }

        public RepMax(RepMax rm)
        {
            foreach (KeyValuePair<Lift, decimal> entry in rm)
            {
                Add(entry.Key, entry.Value);
            }
        }
    }

    
    public struct Cycle
    {
        public Cycle(RepMax trainingMax)
        {
            TrainingMax = trainingMax;
            Weeks = new Dictionary<Week, WeeklyPlan>();

            foreach (Week week in Enum.GetValues(typeof(Week)))
            {
                Weeks[week] = new WeeklyPlan();
            }
        }

        public RepMax TrainingMax;
        public Dictionary<Week, WeeklyPlan> Weeks;
    }

    public struct Plan
    {
        public Plan(RepMax trueOneRepMax, int cycleCount)
        {
            Cycles = new List<Cycle>(cycleCount);
            TrueOneRepMax = trueOneRepMax;
        }
        public RepMax TrueOneRepMax;
        public List<Cycle> Cycles;
    }
}
