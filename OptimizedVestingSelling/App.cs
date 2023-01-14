using System.Threading.Tasks;
using System;

namespace ApplicationTemplate
{
    public class App
    {
        public App()
        {

        }

        // Async application starting point
        public async Task Run()
        {
            // Do somethings
            for (var counter = 0; counter < 30; counter++)
            {
                await Console.Out.WriteLineAsync(counter.ToString());
            }
        }
    }
}
