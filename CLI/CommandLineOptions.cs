﻿// Licensed to the Genometric organization (https://github.com/Genometric) under one or more agreements.
// The Genometric organization licenses this file to you under the GNU General Public License v3.0 (GPLv3).
// See the LICENSE file in the project root for more information.

using Genometric.MSPC.Model;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("Genometric.MSPC.CLI.Tests")]
namespace Genometric.MSPC.CLI
{
    internal class CommandLineOptions
    {
        private readonly CommandLineApplication _cla;

        private readonly CommandOption _cInput = new CommandOption("-i | --input <value>", CommandOptionType.MultipleValue)
        {
            Description = "Input samples to be processed in Browser Extensible Data (BED) Format."
        };

        private readonly CommandOption _cReplicate = new CommandOption("-r | --replicate <value>", CommandOptionType.SingleValue)
        {
            Description = "Sets the replicate type of samples. Possible values are: { Bio, Biological, Tec, Technical }"
        };

        private readonly CommandOption _cTauS = new CommandOption("-s | --tauS <value>", CommandOptionType.SingleValue)
        {
            Description = "Sets stringency threshold. All peaks with p-values lower than this value are considered as stringent peaks."
        };

        private readonly CommandOption _cTauW = new CommandOption("-w | --tauW <value>", CommandOptionType.SingleValue)
        {
            Description = "Sets weak threshold. All peaks with p-values higher than this value are considered as weak peaks."
        };

        private readonly CommandOption _cGamma = new CommandOption("-g | --gamma <value>", CommandOptionType.SingleValue)
        {
            Description = "Sets combined stringency threshold. The peaks with their combined p-values satisfying this threshold will be confirmed."
        };

        private readonly CommandOption _cAlpha = new CommandOption("-a | --alpha <value>", CommandOptionType.SingleValue)
        {
            Description = "Sets false discovery rate of Benjamini–Hochberg step-up procedure."
        };

        private readonly CommandOption _cC = new CommandOption("-c <value>", CommandOptionType.SingleValue)
        {
            Description = "Sets minimum number of overlapping peaks before combining p-values."
        };

        private readonly CommandOption _cM = new CommandOption("-m | --multipleIntersections <value>", CommandOptionType.SingleValue)
        {
            Description = "When multiple peaks from a sample overlap with a given peak, " +
                "this argument defines which of the peaks to be considered: the one with lowest p-value, or " +
                "the one with highest p-value? Possible values are: { Lowest, Highest }"
        };

        private ReplicateType _vreplicate;
        private double _vtauS = 1E-8;
        private double _vtauW = 1E-4;
        private double _vgamma = -1;
        private float _valpha = 0.05F;
        private byte _vc = 1;
        private MultipleIntersections _vm = MultipleIntersections.UseLowestPValue;


        public Config Options { private set; get; }

        public IReadOnlyList<string> Input { get { return _cInput.Values.AsReadOnly(); } }

        public CommandLineOptions()
        {
            _cla = new CommandLineApplication();
            _cla.Options.Add(_cInput);
            _cla.Options.Add(_cReplicate);
            _cla.Options.Add(_cTauS);
            _cla.Options.Add(_cTauW);
            _cla.Options.Add(_cGamma);
            _cla.Options.Add(_cAlpha);
            _cla.Options.Add(_cC);
            _cla.Options.Add(_cM);
            Func<int> assertArguments = AssertArguments;
            _cla.OnExecute(assertArguments);
        }

        private int AssertArguments()
        {
            var missingArgs = new List<string>();
            if (!_cInput.HasValue()) missingArgs.Add(_cInput.ShortName + "|" + _cInput.LongName);
            if (!_cReplicate.HasValue()) missingArgs.Add(_cReplicate.ShortName + "|" + _cReplicate.LongName);
            if (!_cTauS.HasValue()) missingArgs.Add(_cTauS.ShortName + "|" + _cTauS.LongName);
            if (!_cTauW.HasValue()) missingArgs.Add(_cTauW.ShortName + "|" + _cTauW.LongName);

            if (missingArgs.Count > 0)
            {
                var msgBuilder = new StringBuilder("The following required arguments are missing: ");
                foreach (var item in missingArgs)
                    msgBuilder.Append(item + "; ");
                msgBuilder.Append(".");
                throw new ArgumentException(msgBuilder.ToString());
            }

            switch (_cReplicate.Value().ToLower())
            {
                case "bio":
                case "biological":
                    _vreplicate = ReplicateType.Biological;
                    break;

                case "tec":
                case "technical":
                    _vreplicate = ReplicateType.Technical;
                    break;

                default:
                    ThrowInvalidException(_cReplicate.LongName);
                    break;
            }

            if (!double.TryParse(_cTauS.Value(), out _vtauS))
                ThrowInvalidException(_cTauS.LongName);

            if (!double.TryParse(_cTauW.Value(), out _vtauW))
                ThrowInvalidException(_cTauW.LongName);

            if (_cGamma.HasValue() && !double.TryParse(_cGamma.Value(), out _vgamma))
                ThrowInvalidException(_cGamma.LongName);

            if (_cAlpha.HasValue() && !float.TryParse(_cAlpha.Value(), out _valpha))
                ThrowInvalidException(_cAlpha.LongName);

            if (_cC.HasValue() && !byte.TryParse(_cC.Value(), out _vc))
                ThrowInvalidException(_cC.LongName);

            if(_cM.HasValue())
                switch (_cM.Value().ToLower())
                {
                    case "lowest":
                        _vm = MultipleIntersections.UseLowestPValue;
                        break;

                    case "highest":
                        _vm = MultipleIntersections.UseHighestPValue;
                        break;

                    default:
                        ThrowInvalidException(_cM.LongName);
                        break;
                }

            return 0;
        }

        private void ThrowInvalidException(string commandOption)
        {
            throw new ArgumentException("Invalid value given for the " + commandOption + " argument.");
        }

        public Config Parse(string[] args)
        {
            _cla.Execute(args);
            Options = new Config(_vreplicate, _vtauW, _vtauS, _vgamma, _vc, _valpha, _vm);
            return Options;
        }
    }
}