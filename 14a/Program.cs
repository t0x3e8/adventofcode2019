using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace _14a
{
    class Chemical
    {
        public int Units { get; set; }
        public string Name { get; set; }
        public static Chemical Parse(string input)
        {
            input = input.Trim();
            var parts = input.Split(' ');

            Chemical ch = new Chemical();
            ch.Units = int.Parse(parts[0]);
            ch.Name = parts[1].Trim();
            return ch;
        }
    }

    class Reaction
    {
        public Chemical Output { get; set; }
        public List<Chemical> Inputs { get; set; }
        public Reaction()
        {
            this.Inputs = new List<Chemical>();
        }
    }

    class Stat
    {
        public string ChemicalName { get; set; }
        public int TotalProduced { get; set; }
        public int TotalConsumed { get; set; }
    }

    class NanoFactory
    {
        private List<Reaction> reactions;
        private List<Stat> inventory = new List<Stat>();
        public NanoFactory(List<Reaction> reactions)
        {
            this.reactions = reactions;
        }

        public int Run()
        {
            this.inventory = new List<Stat>();
            MakeReaction("FUEL", 1);
            return this.FindStatByName("ORE").TotalConsumed;
        }

        public void MakeReaction(string nameOfReaction, int unitsNeeded)
        {
            var reaction = FindReactionByName(nameOfReaction);

            if (reaction == null) return;

            while (true)
            {
                int freeUnitsInInventory = this.LeftChemical(reaction.Output.Name);
                if (freeUnitsInInventory < unitsNeeded)
                {
                    foreach (Chemical chemical in reaction.Inputs)
                    {
                        MakeReaction(chemical.Name, chemical.Units);
                        UpdateStats(chemical.Name, 0, chemical.Units);
                    }
                    UpdateStats(reaction.Output.Name, reaction.Output.Units, 0);
                }
                else 
                    break;
            }
        }

        private Reaction FindReactionByName(string name)
        {
            return this.reactions.Where(r => r.Output.Name.Equals(name)).FirstOrDefault();
        }

        private int LeftChemical(string name)
        {
            var stat = this.FindStatByName(name);
            if (stat == null)
                return 0;
            else
                return stat.TotalProduced - stat.TotalConsumed;
        }

        private Stat FindStatByName(string name)
        {
            return this.inventory.Find(s => s.ChemicalName.Equals(name));
        }

        private void UpdateStats(string name, int produced, int consumed)
        {
            var stat = this.FindStatByName(name);
            if (stat == null)
            {
                this.inventory.Add(new Stat()
                {
                    ChemicalName = name,
                    TotalConsumed = consumed,
                    TotalProduced = produced
                });
            }
            else
            {
                stat.TotalConsumed += consumed;
                stat.TotalProduced += produced;
            }
        }
    }
    class Program
    {

        static void Main(string[] args)
        {
            var reactions = ReadFile("input.txt");
            NanoFactory nf = new NanoFactory(reactions);
            int totalOREconsumed = nf.Run();

            Console.WriteLine($"The total number of ORE is {totalOREconsumed} consumed.");
        }

        private static List<Reaction> ReadFile(string fileName)
        {
            var stream = File.OpenRead(fileName);
            var sr = new System.IO.StreamReader(stream);
            List<Reaction> reactions = new List<Reaction>();
            while (!sr.EndOfStream)
            {
                var reaction = new Reaction();
                var line = sr.ReadLine();

                var inOut = line.Split("=>");
                reaction.Output = Chemical.Parse(inOut[1]);
                foreach (var seq in inOut[0].Split(','))
                {
                    reaction.Inputs.Add(Chemical.Parse(seq));
                }

                reactions.Add(reaction);
            }
            return reactions;
        }
    }
}