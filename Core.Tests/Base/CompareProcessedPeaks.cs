﻿// Licensed to the Genometric organization (https://github.com/Genometric) under one or more agreements.
// The Genometric organization licenses this file to you under the GNU General Public License v3.0 (GPLv3).
// See the LICENSE file in the project root for more information.

using Genometric.GeUtilities.IntervalParsers.Model.Defaults;
using Genometric.MSPC.Comparers;
using Genometric.MSPC.Core.Model;
using System.Collections.Generic;
using Xunit;

namespace Genometric.MSPC.Core.Tests.Base
{
    public class CompareProcessedPeaks
    {
        [Fact]
        public void BothAreNull()
        {
            // Arrange
            var comparer = new CompareProcessedPeaksByValue<ChIPSeqPeak>();

            // Act
            var result = comparer.Compare(null, null);

            // Assert
            Assert.True(result == 0);
        }

        [Fact]
        public void XIsNull()
        {
            // Arrange
            var comparer = new CompareProcessedPeaksByValue<ChIPSeqPeak>();

            // Act
            var result = comparer.Compare(
                null,
                new ProcessedPeak<ChIPSeqPeak>(new ChIPSeqPeak(), 10, new List<SupportingPeak<ChIPSeqPeak>>()));

            // Assert
            Assert.True(result == -1);
        }

        [Fact]
        public void YIsNull()
        {
            // Arrange
            var comparer = new CompareProcessedPeaksByValue<ChIPSeqPeak>();

            // Act
            var result = comparer.Compare(
                new ProcessedPeak<ChIPSeqPeak>(new ChIPSeqPeak(), 10, new List<SupportingPeak<ChIPSeqPeak>>()),
                null);

            // Assert
            Assert.True(result == 1);
        }
    }
}
