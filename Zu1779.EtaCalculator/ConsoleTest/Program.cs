namespace ConsoleTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Zu1779.EtaCalculator;

    class Program
    {
        static void Main()
        {
            new Program().Run();

            "\r\n\r\nPress any key".Dump();
            //Console.ReadKey();
        }

        //const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const string chars = "ABCDEF";
        private readonly Random rng = new Random();
        private readonly int nr = 50_000;
        private readonly int charNr = 40;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        public void Run()
        {
            // Console.LargestWindowWidth.Dump(nameof(Console.LargestWindowWidth));
            // Console.WindowWidth = (int)Math.Floor(Console.LargestWindowWidth * 0.9);

            var collection = new List<string>(nr);

            for (int ciclo = 0; ciclo < nr; ciclo++)
            {
                var person = new Person { Id = Guid.NewGuid(), Name = rndString() };
                collection.Add($"{person.Id}_{person.Name}");
            }

            var ec = (new EtaCalculator() as IEtaCalculator)
                //.SetCountThreshold(1_000)
                .SetTimeThreashold(TimeSpan.FromSeconds(1));
            ec.ToString().Dump();
            ec.Progress += (s, e) => s.Dump();
            var name = rndString();
            var id = Guid.NewGuid();
            foreach (var _ in collection.TrackProgress(ec)) collection.Contains($"{id}_{name}");
            ec.Complete().ToString().Dump();
        }

        private string rndString() => new string(Enumerable.Range(1, charNr).Select(c => chars[rng.Next(chars.Length)]).ToArray());
    }

    public class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public static class GenericExtension
    {
        public static void Dump<T>(this T obj, string header = null) => Console.WriteLine($"{(header == null ? string.Empty : $"{header}\r\n" )}{obj}");
    }
}
