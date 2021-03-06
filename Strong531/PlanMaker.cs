﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Strong531
{
    public class PlanMaker
    {
        /// <summary>
        /// Take 90% of the 1RM for each lift
        /// </summary>
        /// <param name="oneRepMax">the true 1RM for all lifts</param>
        /// <returns></returns>
        public static RepMax CalculateTrainingMax(RepMax oneRepMax)
        {
            var trainingMax = new RepMax();
            foreach (Lift lift in Enum.GetValues(typeof(Lift)))
            {
                if (oneRepMax.ContainsKey(lift))
                {
                    trainingMax[lift] = oneRepMax[lift] * 0.9m;    
                }
            }

            return trainingMax;
        }
        
        /// <summary>
        /// Add 10 to lower-body lifts and 5 to upper-body lifts
        /// </summary>
        /// <remarks>
        /// <para>
        /// this function updates in-place the provided training max
        /// </para>
        /// <para>
        /// - the lower-body lifts are Deadlift and Squat.
        /// </para>
        /// <para>
        /// - the upper-body lifts are Bench and Press.
        /// </para>
        /// </remarks>
        /// <param name="trainingMax"></param>
        /// <returns></returns>
        private static void UpdateTrainingMax(ref RepMax trainingMax)
        {
            var lowerBodyLifts = new HashSet<Lift>{Lift.Deadlift, Lift.Squat}; 
            foreach (Lift lift in Enum.GetValues(typeof(Lift)))
            {
                if (!trainingMax.ContainsKey(lift))
                {
                    continue;
                }
                var increase = lowerBodyLifts.Contains(lift) ? 10 : 5;
                trainingMax[lift] += increase;
            }
        }
        
        /// <summary>
        /// Makes a plan
        /// </summary>
        /// <param name="trueOneRepMax">the true 1RM max</param>
        /// <param name="cycleCount">how many cycles to plan</param>
        /// <returns>Plan</returns>
        public static Plan MakePlan(RepMax trueOneRepMax, int cycleCount)
        {
            var plan = new Plan(trueOneRepMax, cycleCount);
            var trainingMax = CalculateTrainingMax(trueOneRepMax); 
            for (var i = 0; i < cycleCount; ++i)
            {
                var cycle = MakeCycle(new RepMax(trainingMax));
                plan.Cycles.Add(cycle);
                UpdateTrainingMax(ref trainingMax);
            }

            return plan;            
        }
        
        public static Cycle MakeCycle(RepMax trainingMax) 
        {
            var cycle = new Cycle(trainingMax);
            foreach (Lift lift in Enum.GetValues(typeof(Lift)))
            {
                if (!trainingMax.ContainsKey(lift))
                {
                    continue;
                }
                var weight = trainingMax[lift];
                var warmupSets = CalculateWarmupSets(weight);
                foreach (Week week in Enum.GetValues(typeof(Week)))
                {
                    var workSets = CalculateWorkSets(weight, week);
                    cycle.Weeks[week][lift] = warmupSets.Concat(workSets).ToArray();
                }
            }

            return cycle;            
        }

        private static int roundToNearestFive(decimal number)
        {
            return (int)(number + 2.5m) / 5 * 5;
        }
        
        /// <summary>
        /// Calculate warmup sets at 40%, 50% and 60% of training max
        ///
        /// The numbers are rounded to the nearest to 5 pound boundary 
        /// </summary>
        /// <param name="trainingMax"></param>
        /// <returns>Array of 3 warmup sets</returns>
        public static Set[] CalculateWarmupSets(decimal trainingMax)
        {
            var res = new Set[3];
            for (var i = 0; i < 3; ++i)
            {
                res[i].Reps = i < 2 ? 5 : 3;
                res[i].Weight = roundToNearestFive((0.4m + i * 0.1m) * trainingMax);
            }
            return res;
        }

        /// <summary>
        /// Calculate work sets according to Wendler's formula
        ///
        /// In week one all sets are 5 reps at 65%, 70% and 75%
        /// In week two all sets are 3 reps at 75%, 80% and 85%
        /// In week three the sets are 5, 3, 1 at 85%, 90% and 95%
        ///
        /// The percentages are off the training max
        /// 
        /// The numbers are rounded down to 5 pound boundary 
        /// </summary>
        /// <param name="trainingMax"></param>
        /// <param name="week"></param>
        /// <returns></returns>
        public static Set[] CalculateWorkSets(decimal trainingMax, Week week)
        {
            if (week == Week.Deload)
            {
                return new Set[]{};
            }
            
            var res = new Set[3];
            for (var i = 0; i < 3; ++i)
            {
                res[i].Reps = week == Week.OfOne ? 5 - i * 2 : 5 - (int)week * 2;                
                res[i].Weight = roundToNearestFive((0.65m + 0.05m * (int)week + i * 0.1m) * trainingMax);
            }
            return res;
        }
    }
}
