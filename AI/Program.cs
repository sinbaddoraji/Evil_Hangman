using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI
{
    class Program
    {
        static Dictionary GameDictionary;

        static void Main(string[] args)
        {
            GameDictionary = new Dictionary();

            foreach (var item in GameDictionary.WordFamilies["oom"].GetWords(4))
            {
                Console.WriteLine(item);
            } 

            Console.ReadKey();
        }
    }
}
