using CloneClownAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloneClownDaemon2
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            CloneClown cloneClown = new CloneClown();
            await cloneClown.Start();
        }
    }
}
