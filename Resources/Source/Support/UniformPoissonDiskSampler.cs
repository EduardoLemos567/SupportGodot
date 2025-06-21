using System;
using System.Collections.Generic;
using Godot;
using Support.Geometrics;
using Support.Numerics;
using Support.Rng;

namespace Game
{
    // http://theinstructionlimit.com/fast-uniform-poisson-disk-sampling-in-c <- copied from Renaud Bédard
    // Adapted from java source by Herman Tulleken
    // http://www.luma.co.za/labs/2008/02/27/poisson-disk-sampling/
    // The algorithm is from the "Fast Poisson Disk Sampling in Arbitrary Dimensions" paper by Robert Bridson
    // http://www.cs.ubc.ca/~rbridson/docs/bridson-siggraph07-poissondisk.pdf
    public class UniformPoissonDiskSampler
    {
        private const int DEFAULT_POINTS_PER_ITERATION = 30;
        private const double PI = Math.PI;
        private const double TWO_PI = PI * 2;
        private static readonly double SQRT_TWO = Math.Sqrt(2);
        //parameters
        private readonly ARng rng;
        private readonly Rectangle<double> rect;
        private readonly double minDistance;
        private readonly int pointsPerIteration;
        //state
        private readonly Vec2<double> center;
        private readonly double cellSize;
        private readonly double? rejectionSqrDistance;
        private readonly Vec2<int> gridSize;
        private readonly Vec2<double>?[,] grid;
        private readonly List<Vec2<double>> activePoints;
        private readonly List<Vec2<double>> points;
        public IReadOnlyList<Vec2<double>> GeneratedPoints => points;
        private UniformPoissonDiskSampler(ARng rng, Rectangle<double> rect, double? rejectionDistance, double minDistance, int pointsPerIteration)
        {
            this.rng = rng;
            this.rect = rect;
            this.minDistance = minDistance;
            this.pointsPerIteration = pointsPerIteration;
            this.center = rect.Center;
            this.cellSize = minDistance / SQRT_TWO;
            this.rejectionSqrDistance = rejectionDistance is null ? null : rejectionDistance * rejectionDistance;
            this.gridSize = (rect.Size / cellSize + 1).CastTo<int>();
            this.grid = new Vec2<double>?[gridSize.x, gridSize.y];
            this.activePoints = [];
            this.points = [];
            Generate();
        }
        public static UniformPoissonDiskSampler CircleSampler(ARng rng,
                                                              in Vec2<double> center,
                                                              double radius,
                                                              double minDistance,
                                                              int pointsPerIteration = DEFAULT_POINTS_PER_ITERATION)
        {
            return new(rng,
                       new(default, new(radius * 2)) { Center = center },
                       radius,
                       minDistance,
                       pointsPerIteration);
        }
        public static UniformPoissonDiskSampler RectangleSampler(ARng rng,
                                                                 in Rectangle<double> rect,
                                                                 double minDistance,
                                                                 int pointsPerIteration = DEFAULT_POINTS_PER_ITERATION)
        {
            return new(rng,
                       rect,
                       null,
                       minDistance,
                       pointsPerIteration);
        }
        private void Generate()
        {
            AddFirstPoint();
            while (activePoints.Count > 0)
            {
                var listIndex = rng.GetNumber<int>(0, activePoints.Count);
                var point = activePoints[listIndex];
                var anyAdded = false;
                for (var k = 0; k < pointsPerIteration; k++)
                {
                    if (AddNextPoint(point))
                    {
                        anyAdded = true;
                    }
                }
                if (!anyAdded) { activePoints.RemoveAt(listIndex); }
            }
        }
        private void AddFirstPoint()
        {
            var size = rect.Size;
            while (true)
            {
                var randomPoint = rect.Min + rng.GetVec2<double>(Vec2<double>.Zero, size);
                if (rejectionSqrDistance.HasValue && center.SqrDistance(randomPoint) > rejectionSqrDistance.Value)
                {
                    continue;
                }
                var index = Denormalize(randomPoint, rect.Min, cellSize);
                grid[index.x, index.y] = randomPoint;
                activePoints.Add(randomPoint);
                points.Add(randomPoint);
                break;
            }
        }
        private bool AddNextPoint(in Vec2<double> point)
        {
            var randomPoint = GenerateRandomPointAround(point, minDistance);
            if (randomPoint.x >= rect.Min.x && randomPoint.x < rect.Max.x &&
                randomPoint.y > rect.Min.y && randomPoint.y < rect.Max.y &&
                (rejectionSqrDistance == null || center.SqrDistance(randomPoint) <= rejectionSqrDistance))
            {
                var randomPointIndex = Denormalize(randomPoint, rect.Min, cellSize);
                for (var x = Mathf.Max(0, randomPointIndex.x - 2); x < Mathf.Min(gridSize.x, randomPointIndex.x + 3); x++)
                {
                    for (var y = Mathf.Max(0, randomPointIndex.y - 2); y < Mathf.Min(gridSize.y, randomPointIndex.y + 3); y++)
                    {
                        if (grid[x, y].HasValue && grid[x, y]!.Value.SqrDistance(randomPoint) < minDistance * minDistance)
                        {
                            return false;
                        }
                    }
                }
                activePoints.Add(randomPoint);
                points.Add(randomPoint);
                grid[randomPointIndex.x, randomPointIndex.y] = randomPoint;
                return true;
            }
            return false;
        }
        private Vec2<double> GenerateRandomPointAround(in Vec2<double> center, double minimumDistance)
        {
            var angle = rng.GetNumber(0, TWO_PI);
            // angle * radius
            var newPoint = new Vec2<double>(Math.Sin(angle), Math.Cos(angle)) * (minimumDistance + rng.GetNumber(0, minimumDistance));
            return center + newPoint;
        }
        private static Vec2<int> Denormalize(in Vec2<double> point, in Vec2<double> origin, double cellSize) => ((point - origin) / cellSize).CastTo<int>();
    }
}