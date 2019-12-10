using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _10a
{
  class Program
  {
    class Asteroid
    {
      public int X { get; set; }
      public int Y { get; set; }
      public int AsteroidsInSight { get; set; }
      public void UpdateAsteroidsInSight(HashSet<Asteroid> asteroids)
      {
        HashSet<double> angles = new HashSet<double>();
        foreach (var asteroid in asteroids)
        {
          if (asteroid == this) 
            continue;
          var angle = (Math.Atan2(asteroid.Y - this.Y, asteroid.X - this.X) * 180 / Math.PI);
          angles.Add(angle);
        }

        this.AsteroidsInSight = angles.Count;
      }

      public override string ToString()
      {
        return $"{this.X}, {this.Y} =) {this.AsteroidsInSight}";
      }
    }


    static void Main(string[] args)
    {
      var asteroids = ReadFile("input.txt");
      var maxAsteroid = new Asteroid();

      foreach (var asteroid in asteroids)
      {
        asteroid.UpdateAsteroidsInSight(asteroids);
        if (maxAsteroid.AsteroidsInSight < asteroid.AsteroidsInSight)
          maxAsteroid = asteroid;
      }

      Console.WriteLine(maxAsteroid);
    }

    static HashSet<Asteroid> ReadFile(string fileName)
    {
      var stream = File.OpenRead(fileName);
      var sr = new System.IO.StreamReader(stream);
      int y = 0;
      var asteroids = new HashSet<Asteroid>();

      while (!sr.EndOfStream)
      {
        var line = sr.ReadLine();
        for (int x = 0; x < line.Length; x++)
        {
          if (line[x] != '.')
            asteroids.Add(new Asteroid() { X = x, Y = y });
        }
        y++;
      }
      return asteroids;
    }
  }
}