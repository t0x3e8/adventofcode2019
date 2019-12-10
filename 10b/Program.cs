using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _10b
{
  class Program
  {
    class Asteroid
    {
      public int X { get; set; }
      public int Y { get; set; }
      public double Angle { get; set; }
      public int Distance { get; set; }
      public void UpdateDistanceAndAngle(Asteroid satelit)
      {
        this.Angle = (Math.Atan2(satelit.Y - this.Y, satelit.X - this.X) * 180 / Math.PI) - 90;
        if (this.Angle < 0)
          this.Angle += 360;
        this.Distance = Math.Abs(satelit.Y - this.Y) + Math.Abs(satelit.X - this.X);
      }
    }

    static void Main(string[] args)
    {
      var input = ReadFile("input.txt");
      var station = input.Item1;
      var asteroids = input.Item2;

      foreach (var asteroid in asteroids)
      {
        asteroid.UpdateDistanceAndAngle(station);
      }

      var lastAsteroid = GetAsteroidToBeVaporizedByCount(asteroids, 200);
      Console.WriteLine($"{lastAsteroid.X * 100 + lastAsteroid.Y}");
    }

    private static Asteroid GetAsteroidToBeVaporizedByCount(HashSet<Asteroid> asteroids, int count)
    {
      int counter = 0;
      while (asteroids.Count > 0)
      {
        foreach (var angle in asteroids.Select(a => a.Angle).OrderBy(a => a).Distinct())
        {
          var asteroidsOnSameAngle = asteroids.Select(a => a).Where(a => a.Angle.Equals(angle));
          var asteroidToBeVaporized = asteroidsOnSameAngle.OrderBy(a => a.Distance).First();

          counter++;
          if (counter == count)
            return asteroidToBeVaporized;
          else
            asteroids.Remove(asteroidToBeVaporized);
        }
      }

      return null;
    }

    static Tuple<Asteroid, HashSet<Asteroid>> ReadFile(string fileName)
    {
      var stream = File.OpenRead(fileName);
      var sr = new System.IO.StreamReader(stream);
      int y = 0;
      var asteroids = new HashSet<Asteroid>();
      var station = new Asteroid();

      while (!sr.EndOfStream)
      {
        var line = sr.ReadLine();
        for (int x = 0; x < line.Length; x++)
        {
          if (line[x] == '#')
            asteroids.Add(new Asteroid() { X = x, Y = y });
          else if (line[x] == 'X')
            station = new Asteroid() { X = x, Y = y };
        }
        y++;
      }
      return new Tuple<Asteroid, HashSet<Asteroid>>(station, asteroids);
    }
  }
}