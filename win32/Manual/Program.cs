﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Manual
{
    /// <summary>
    /// Recommendation ITU-R P.528 Results Data Structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Result
    {
        /// <summary>
        /// Mode of propagation
        /// </summary>
        public int propagation_mode;

        public int LOSIterations;

        /// <summary>
        /// Path distance used in calculations, in km
        /// </summary>
        public double d__km;

        /// <summary>
        /// Basic transmission loss, in dB
        /// </summary>
        public double A__db;

        /// <summary>
        /// Free space basic transmission loss, in dB
        /// </summary>
        public double A_fs__db;
    }

    enum Polarization : int
    {
        Horizontal = 0,
        Vertical = 1
    }

    class Program
    {
        [DllImport("p528.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, EntryPoint = "P528")]
        private static extern int P528(double d__km, double h_1__meter, double h_2__meter, double f__mhz,
            int T_pol, double time_percentage, ref Result result);

        const string NEW_DATA_TABLES_DIR = "DataTables-v5";
        const string OLD_DATA_TABLES_DIR = "DataTables-v4";
        const string DIFF_DATA_TABLES_DIR = "DiffDataTables";

        static void Main(string[] args)
        {
            //double d__km = 1000;
            //double h_1__meter = 10;
            //double h_2__meter = 200e3;
            //double f__mhz = 15500;
            //double time_percentage = 0.5;
            //Result result = new Result();

            //int rtn = P528(d__km, h_1__meter, h_2__meter, f__mhz, time_percentage, ref result);

            //Console.WriteLine(result.A__db);
            //Console.ReadKey();

            GenerateDataFiles();
            DiffDataTables();

            Console.WriteLine();
            Console.WriteLine("Done.");
            Console.ReadKey();
        }

        /// <summary>
        /// Generate a new set of CSV data table files
        /// </summary>
        static void GenerateDataFiles()
        {
            // Generate data file format
            var freqs = new List<double> { 125 };//, 300, 600, 1200, 2400, 5100, 9400, 15500 };
            var qs = new List<double> { 1 };//, 5, 10, 50, 95 };

            string dir = Path.Combine(Environment.CurrentDirectory, NEW_DATA_TABLES_DIR);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            foreach (var f__mhz in freqs)
            {
                foreach (var q in qs)
                {
                    var filename = $"{f__mhz.ToString("0,0")} MHz - Lb({(q / 100).ToString("0.00")})_P528.csv";
                    Console.Write($"Generating {filename}...");

                    using (var fs = new StreamWriter(Path.Combine(dir, filename)) { AutoFlush = true })
                    {
                        fs.WriteLine($"{f__mhz}MHz / Lb({(q / 100).ToString("0.00")}) dB");
                        fs.WriteLine(",h2(m),1000,1000,1000,1000,1000,10000,10000,10000,10000,10000,10000,20000,20000,20000,20000,20000,20000,20000");
                        fs.WriteLine(",h1(m),1.5,15,30,60,1000,1.5,15,30,60,1000,10000,1.5,15,30,60,1000,10000,20000");
                        fs.WriteLine("D (km),FSL");

                        for (int d__km = 0; d__km <= 1000; d__km++)
                        {
                            fs.Write(d__km);

                            fs.Write($",{GetFreeSpaceLoss(d__km, 1.5, 1000, f__mhz, q)}"); // FSL
                            fs.Write($",{GetLoss(d__km, 1.5, 1000, f__mhz, q)}");
                            fs.Write($",{GetLoss(d__km, 15, 1000, f__mhz, q)}");
                            fs.Write($",{GetLoss(d__km, 30, 1000, f__mhz, q)}");
                            fs.Write($",{GetLoss(d__km, 60, 1000, f__mhz, q)}");
                            fs.Write($",{GetLoss(d__km, 1000, 1000, f__mhz, q)}");
                            fs.Write($",{GetLoss(d__km, 1.5, 10000, f__mhz, q)}");
                            fs.Write($",{GetLoss(d__km, 15, 10000, f__mhz, q)}");
                            fs.Write($",{GetLoss(d__km, 30, 10000, f__mhz, q)}");
                            fs.Write($",{GetLoss(d__km, 60, 10000, f__mhz, q)}");
                            fs.Write($",{GetLoss(d__km, 1000, 10000, f__mhz, q)}");
                            fs.Write($",{GetLoss(d__km, 10000, 10000, f__mhz, q)}");
                            fs.Write($",{GetLoss(d__km, 1.5, 20000, f__mhz, q)}");
                            fs.Write($",{GetLoss(d__km, 15, 20000, f__mhz, q)}");
                            fs.Write($",{GetLoss(d__km, 30, 20000, f__mhz, q)}");
                            fs.Write($",{GetLoss(d__km, 60, 20000, f__mhz, q)}");
                            fs.Write($",{GetLoss(d__km, 1000, 20000, f__mhz, q)}");
                            fs.Write($",{GetLoss(d__km, 10000, 20000, f__mhz, q)}");
                            fs.Write($",{GetLoss(d__km, 20000, 20000, f__mhz, q)}");

                            fs.WriteLine();
                        }
                    }

                    Console.WriteLine("done.");
                }
            }
        }

        /// <summary>
        /// Perform a diff with the P.528-4 data table files
        /// </summary>
        static void DiffDataTables()
        {
            string v4Dir = Path.Combine(Environment.CurrentDirectory, OLD_DATA_TABLES_DIR);
            string v5Dir = Path.Combine(Environment.CurrentDirectory, NEW_DATA_TABLES_DIR);

            string dir = Path.Combine(Environment.CurrentDirectory, DIFF_DATA_TABLES_DIR);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            foreach (var file in Directory.GetFiles(v5Dir))
            {
                var fi = new FileInfo(file);

                using (var fs = new StreamWriter(Path.Combine(dir, fi.Name.Replace(".csv", ".diff.csv"))) { AutoFlush = true })
                {
                    // load v4 and v5 data table files
                    var lines_v4 = File.ReadAllLines(Path.Combine(v4Dir, fi.Name));
                    var lines_v5 = File.ReadAllLines(Path.Combine(v5Dir, fi.Name));

                    // first 4 lines are static
                    for (int i = 0; i < 4; i++)
                        fs.WriteLine(lines_v4[i]);

                    // now compute diff (v4-v5) and write to file
                    for (int i = 4; i < lines_v4.Length; i++)
                    {
                        var parts_v4 = lines_v4[i].Split(',');
                        var parts_v5 = lines_v5[i].Split(',');

                        fs.Write(i - 4);
                        for (int j = 1; j < parts_v4.Length; j++)
                        {
                            double diff = Convert.ToDouble(parts_v4[j]) - Convert.ToDouble(parts_v5[j]);
                            fs.Write($",{diff}");

                            if (Math.Abs(diff) > 0.5)
                                Console.WriteLine("DIFF!!!");
                        }
                        fs.WriteLine();
                    }
                }
            }
        }

        /// <summary>
        /// Get the basic transmission loss from P.528
        /// </summary>
        static double GetLoss(double d__km, double h1, double h2, double f__mhz, double q)
        {
            var result = new Result();
            P528(d__km, h1, h2, f__mhz, (int)Polarization.Horizontal, q, ref result);

            return -Math.Round(result.A__db, 1);
        }

        /// <summary>
        /// Get the free space loss value from P.528
        /// </summary>
        static double GetFreeSpaceLoss(double d__km, double h1, double h2, double f__mhz, double q)
        {
            var result = new Result();
            P528(d__km, h1, h2, f__mhz, (int)Polarization.Horizontal, q, ref result);

            return -Math.Round(result.A_fs__db, 1);
        }
    }
}
