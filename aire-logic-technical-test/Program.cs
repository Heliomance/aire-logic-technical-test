using System;
using System.Threading.Tasks;

namespace aire_logic_technical_test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var controller = new Controller();
            await controller.Run();
        }
    }
}
