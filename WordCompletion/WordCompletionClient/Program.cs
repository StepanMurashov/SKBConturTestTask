using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordCompletion;

namespace WordCompletionClient
{
    class Program
    {
        static void Main(string[] args)
        {
            IWordCompletionDictionary Dictionary = DictionaryFactory.CreateFromStream(Console.In);

        }
    }
}
