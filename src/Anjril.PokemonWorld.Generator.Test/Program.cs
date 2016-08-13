using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Anjril.PokemonWorld.Generator.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-----------------------");
            Console.WriteLine("-----------------------");
            Console.WriteLine("Pokemon World Generator");
            Console.WriteLine();

            Console.Write("Generating map");

            var generator = new WorldGen(400, 400, "Generated", true, true);

            var generationResult = Task.Run(() =>
            {
                var chrono = Stopwatch.StartNew();

                generator.GenerateMap();

                chrono.Stop();

                return chrono.Elapsed;
            });

            int nbLoop = 14;
            while (!generationResult.IsCompleted)
            {
                Console.Write(".");
                nbLoop++;

                if (nbLoop == 23)
                {
                    nbLoop = 0;
                    Console.WriteLine();
                }

                Thread.Sleep(5000);
            }

            Console.WriteLine("Map generated!");

            Console.WriteLine();
            Console.WriteLine("Generation took: {0:#.##}s", generationResult.Result.TotalSeconds);
            Console.WriteLine();

            Console.WriteLine("Pokemon World Generator");
            Console.WriteLine("-----------------------");
            Console.WriteLine("-----------------------");

            Console.WriteLine();
            Console.WriteLine("Press a key to finish");
            Console.ReadKey();
        }
    }
}
