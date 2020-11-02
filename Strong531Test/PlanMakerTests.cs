using NUnit.Framework;
using Strong531;

namespace Strong531Test
{
    public class PlanMakerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CalculateTrainintMax_Test()
        {
            var oneRepMax = new RepMax
            {
                {Lift.Press, 110},
                {Lift.Deadlift, 200},
                {Lift.Bench, 155},
                {Lift.Squat, 165},
            };

            var trainingMax = PlanMaker.CalculateTrainingMax(oneRepMax);
            Assert.AreEqual(0.9 * 110 , (double)trainingMax[Lift.Press], 0.1);
            Assert.AreEqual(0.9 * 200, (double)trainingMax[Lift.Deadlift], 0.1);
            Assert.AreEqual(0.9 * 155, (double)trainingMax[Lift.Bench], 0.1);
            Assert.AreEqual(0.9 * 165, (double)trainingMax[Lift.Squat], 0.1);
        }

        [Test]
        public void CalculateWarmupSets_Test()
        {
            var warmupSets = PlanMaker.CalculateWarmupSets(100);
            Assert.AreEqual(3, warmupSets.Length);

            Assert.AreEqual(5, warmupSets[0].Reps);
            Assert.AreEqual(40, warmupSets[0].Weight);

            Assert.AreEqual(5, warmupSets[1].Reps);
            Assert.AreEqual(50, warmupSets[1].Weight);

            Assert.AreEqual(3, warmupSets[2].Reps);
            Assert.AreEqual(60, warmupSets[2].Weight);
        }

        [Test]
        public void CalculateWorkSets_WeekOfFive_Test()
        {
            var workSets = PlanMaker.CalculateWorkSets(100, Week.OfFive);
            Assert.AreEqual(3, workSets.Length);

            Assert.AreEqual(5, workSets[0].Reps);
            Assert.AreEqual(65, workSets[0].Weight);

            Assert.AreEqual(5, workSets[1].Reps);
            Assert.AreEqual(75, workSets[1].Weight);

            Assert.AreEqual(5, workSets[2].Reps);
            Assert.AreEqual(85, workSets[2].Weight);
        }

        [Test]
        public void CalculateWorkSets_WeekOfThree_Test()
        {
            var workSets = PlanMaker.CalculateWorkSets(100, Week.OfThree);
            Assert.AreEqual(3, workSets.Length);

            Assert.AreEqual(3, workSets[0].Reps);
            Assert.AreEqual(70, workSets[0].Weight);

            Assert.AreEqual(3, workSets[1].Reps);
            Assert.AreEqual(80, workSets[1].Weight);

            Assert.AreEqual(3, workSets[2].Reps);
            Assert.AreEqual(90, workSets[2].Weight);
        }

        [Test]
        public void CalculateWorkSets_WeekOfOne_Test()
        {
            var workSets = PlanMaker.CalculateWorkSets(100, Week.OfOne);
            Assert.AreEqual(3, workSets.Length);

            Assert.AreEqual(5, workSets[0].Reps);
            Assert.AreEqual(75, workSets[0].Weight);

            Assert.AreEqual(3, workSets[1].Reps);
            Assert.AreEqual(85, workSets[1].Weight);

            Assert.AreEqual(1, workSets[2].Reps);
            Assert.AreEqual(95, workSets[2].Weight);
        }
        
        [Test]
        public void CalculateWorkSets_DeloadWeek_Test()
        {
            var workSets = PlanMaker.CalculateWorkSets(100, Week.Deload);
            Assert.AreEqual(0, workSets.Length);
        }
    }
}