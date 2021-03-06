﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harvester.Properties;

namespace Harvester
{
    class Program
    {
        static void Main(string[] args)
        {
            // Start the performance collection
            var type = "analyze";
            if(args.Length > 0)
                type = args[0];

            if (type != "analyze")
            {
                Collect(type);
            }
            else
            {
                Analyze("Matmul", "philosophers45", "data/Philosophers45");

                /*Analyze("MatmulKJI", "data/MatmulKJI");
                Analyze("MatmulIJK", "data/MatmulIJK");
                Analyze("Matmul", "data/AccountB");
                Analyze("MatmulIJK", "data/MatmulIJK");
                Analyze("MatmulKIJ", "data/MatmulKIJ");
                Analyze("MatmulKJI", "data/MatmulKJI");
                Analyze("ComputePi", "data/ComputePi");
                Analyze("Mandelbrot", "data/Mandelbrot");
                Analyze("NQueens", "data/NQueens");
                Analyze("Matmul", "data/ParticleS");
                Analyze("RayTracer", "data/RayTracer");
                Analyze("Matmul", "data/AccountA");
                Analyze("Matmul", "data/MergeSortP");
                Analyze("Matmul", "data/MergeSortS");
                Analyze("Matmul", "data/ParticleP");
                Analyze("Matmul", "data/NodeJS");     
                Analyze("Matmul", "data/Spike");      
                */
            }

            Console.WriteLine("Analysis: Completed");
            if (type != "analyze")
                Environment.Exit(0);
            
        }

        /// <summary>
        /// Performs the data collection and the subsequent analysis.
        /// </summary>
        static void Collect(string processName)
        {
            var pcm = HarvestProcess.FromBinary("pcm-win", "pcm.exe", Resources.pcm_win);
            var os = HarvestProcess.FromBinary("os-win", "PerfMonitor.exe", Resources.os_win);

            pcm.Run("5");
            os.Run(" -KernelEvents:ContextSwitch,MemoryPageFaults /providers:FE2625C1-C10D-452C-B813-A8703EA9D2BA start");

            Console.WriteLine("Press any key to stop data collection...");
            Console.ReadKey();

            pcm.Stop();
            os.Run("stop");

            Console.WriteLine("Waiting for data collection to stop...");
            os.WaitForExit();
            pcm.WaitForExit();

            

            // Analyze
            var experiment = new HarvestExperimentWin();
            Console.WriteLine("Copying and merging to the experiment folder...");
            experiment.Merge(processName, processName, pcm, os);

        }

        /// <summary>
        /// Analyzes the data.
        /// </summary>
        /// <param name="processName">The process to analyze.</param>
        /// <param name="folder">The folder containing the data</param>
        static void Analyze(string processName, string name, string folder)
        {
            var experiment = new HarvestExperimentWin(folder);
            experiment.Merge(processName, name, null, null);
        }


    }
}
