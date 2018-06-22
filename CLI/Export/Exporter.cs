﻿// Licensed to the Genometric organization (https://github.com/Genometric) under one or more agreements.
// The Genometric organization licenses this file to you under the GNU General Public License v3.0 (GPLv3).
// See the LICENSE file in the project root for more information.


using Genometric.GeUtilities.IGenomics;
using Genometric.MSPC.Core.Model;
using Genometric.MSPC.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Genometric.MSPC.CLI.Exporter
{
    public class Exporter<P>
        where P : IChIPSeqPeak, new()
    {
        private readonly string _header = "chr\tstart\tstop\tname\t-1xlog10(p-value)\txSqrd\t-1xlog10(Right-Tail Probability)";
        private Options _options;

        public void Export(
            Dictionary<uint, string> fileNames,
            ReadOnlyDictionary<uint, Result<P>> results,
            ReadOnlyDictionary<string, SortedList<P, P>> consensusPeaks,
            Options options)
        {
            _options = options;

            if (!Directory.Exists(_options.Path))
                Directory.CreateDirectory(_options.Path);

            string date =
                DateTime.Now.Date.ToString("dd'_'MM'_'yyyy", CultureInfo.InvariantCulture) +
                "_h" + DateTime.Now.TimeOfDay.Hours.ToString() +
                "_m" + DateTime.Now.TimeOfDay.Minutes.ToString() +
                "_s" + DateTime.Now.TimeOfDay.Seconds.ToString() + "__";

            ExportConsensusPeaks(consensusPeaks);

            foreach (var result in results)
            {
                int duplicationExtension = 0;

                string samplePath = _options.Path + Path.DirectorySeparatorChar + date + Path.GetFileNameWithoutExtension(fileNames[result.Key]);
                while (Directory.Exists(samplePath))
                    samplePath = _options.Path + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(fileNames[result.Key]) + "_" + (duplicationExtension++).ToString();
                Directory.CreateDirectory(samplePath);

                foreach(var attribute in options.AttributesToExport)
                    WriteToFile(samplePath, result.Value, attribute);
            }
        }

        private void WriteToFile(string samplePath, Result<P> data, Attributes attribute)
        {
            string fileName = samplePath + Path.DirectorySeparatorChar + attribute.ToString() + ".bed";
            File.Create(fileName).Dispose();
            using (StreamWriter writter = new StreamWriter(fileName))
            {
                if (_options.IncludeHeader)
                    writter.WriteLine(_header);

                foreach (var chr in data.Chromosomes)
                {
                    var sortedDictionary = from entry in chr.Value.Get(attribute) orderby entry ascending select entry;

                    foreach (var item in sortedDictionary)
                    {
                        writter.WriteLine(
                            chr.Key + "\t" +
                            item.Source.Left.ToString() + "\t" +
                            item.Source.Right.ToString() + "\t" +
                            item.Source.Name + "\t" +
                            ConvertPValue(item.Source.Value) + "\t" +
                            Math.Round(item.XSquared, 3) + "\t" +
                            ConvertPValue(item.RTP));
                    }
                }
            }
        }

        private void ExportConsensusPeaks(ReadOnlyDictionary<string, SortedList<P, P>> peaks)
        {
            File.Create(_options.Path + Path.DirectorySeparatorChar + "ConsensusPeaks.bed").Dispose();
            using (StreamWriter writter = new StreamWriter(_options.Path + Path.DirectorySeparatorChar + "ConsensusPeaks.bed"))
            {
                if (_options.IncludeHeader)
                    writter.WriteLine("chr\tstart\tstop\tname\tX-squared");

                foreach (var chr in peaks)
                {
                    foreach (var item in chr.Value)
                    {
                        writter.WriteLine(
                            chr.Key + "\t" +
                            item.Value.Left.ToString() + "\t" +
                            item.Value.Right.ToString() + "\t" +
                            item.Value.Name + "\t" +
                            Math.Round(item.Value.Value, 3));
                    }
                }
            }
        }

        private string ConvertPValue(double pValue)
        {
            if (pValue != 0)
                return (Math.Round((-1) * Math.Log10(pValue), 3)).ToString();
            return "0";
        }
    }
}
