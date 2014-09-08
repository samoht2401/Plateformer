using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plateformer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (PlatformerGame game = new PlatformerGame())
            {
                game.Run();
            }
        }
    }
}
