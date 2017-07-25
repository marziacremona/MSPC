### Call Example:
{{
// minimum
MSPC -i rep1.bed -i rep2.bed -r bio -s 1E-8 -w 1E-4

// complete
MSPC -i rep1.bed -i rep2.bed -r bio -s 1E-8 -w 1E-4 -g 1E-9 -c 2 -a 0.005
}}
Note that examples are for Microsoft Windows, on Linux/Mac invoke **[mono](http://www.mono-project.com)** before **MSPC**, for instance: 
{{
mono MSPC -i rep1.bed -i rep2.bed -r bio -s 1E-8 -w 1E-4
}}

# Arguments
In the following we explain arguments in details. 
## Input: (Required)
Method A: Sample files are listed with the character '&' as separator between them.
Method B: Sample files are listed after the '-i' argument.
Argument: _Short:_ **-i** _Long:_ **-input**
Type: **Required**
Valid values: **any BED file in the format above specified**
Default value: **none**
Example:
{{
MSPC -i rep1.bed&rep2.bed&rep3.bed
MSPC -i rep1.bed -i rep2.bed -i rep3.bed
}}

## Replicate Type: (Required)
Samples could be biological or technical replicates. The algorithm differentiates between the two replicate types based on the fact that less variations between technical replicates is expected compared to biological replicates. Replicate type can be specified using the following argument:

Argument: _Short_: **-r** _Long:_ **-replicate**
Type: **Required**
Valid values: **Bio, Biological, Tec, Technical**
Default value: **none**
Example:
{{
MSPC -i rep1.bed -i rep2.bed  -r tec
MSPC -i rep1.bed -i rep2.bed -i rep3.bed -r biological
}}

## Stringency Threshold: (Required)
It specifies the threshold for stringent peaks. Any peak with p-value lower than this threshold is set as stringent peak.

Argument: _Short:_ **-s** _Long:_ **-tauS**
Type: **Required**
Valid values: **Double**
Default value: **none**
Example:
{{
MSPC -i rep1.bed -i rep2.bed -s 1E-8
}}

## Weak Threshold: (Required)
It specifies the threshold for weak peaks. Any peak with p-value lower than this threshold and higher or equal to the Stringency Threshold is set as weak peak; any peak with p-value higher than this threshold is discarded.

Argument: _Short:_ **-w** _Long:_ **-tauW**
Type: **Required**
Valid values: **Double**
Default value: **none**
Example:
{{
MSPC -i rep1.bed -i rep2.bed -w 1E-4
}}

## Gamma: (Optional)
It sets the combined stringency threshold. Peaks with combined p-value below this threshold are confirmed.

Argument: _Short:_ **-g** _Long:_ **-gamma**
Type: **Optional**
Valid values: **Double**
Default value: **Equal to Stringency Threshold**
Example:
{{
MSPC -i rep1.bed -i rep2.bed -g 1E-8
}}

## C: (Optional)
It specifies the minimum number of samples where overlapping peaks must be called to combine their p-value. For example, given three replicates (rep1, rep2 and rep3), if C = 3, a peak on rep1 must intersect with at least one peak from both rep2 and rep3 to combine their p-values, otherwise the peak is discarded; if C = 2, a peak on rep1 must intersect with at least one peak from either rep2 or rep3 to combine their p-values, otherwise the peak is discarded.

Argument: **-c**
Type: **Optional**
Valid values: **Integer**
Default value: **1**
Example:
{{
MSPC -i rep1.bed -i rep2.bed -c 2
}}

## Alpha: (Optional)
Threshold for Benjamini-Hochberg multiple testing correction.

Argument: _Short:_ **-a** _Long:_ **-alpha**
Type: **Optional**
Valid values: **Double**
Default value: **0.05**
Example:
{{
MSPC -i rep1.bed -i rep2.bed -a 0.05
}}

# Output
For each sample, the following bed files are created in a folder named as the sample:
# AOutputSet.bed: Stringent confirmed and weak confirmed peaks, passing the Benjamini-Hochberg multiple testing correction.
# AOutputSetFalsePositives.bed: Stringent confirmed and weak confirmed peaks which do not pass the Benjamini-Hochberg multiple testing correction.
# AOutputSetstringentConfirmedpeaks.bed_: Stringent confirmed peaks, passing the Benjamini-Hochberg multiple testing correction.
# AOutputSetweakConfirmedpeaks.bed_: Weak Confirmed peaks, passing the Benjamini-Hochberg multiple testing correction.
# BStringentPeaks.bed: Peaks with p-value below the stringency threshold.
# CWeakPeaks.bed: Peaks with p-value above or equal to the stringency threshold and below the weak threshold.
# DConfirmedPeaks.bed: Stringent and weak confirmed peaks.
# DStringentConfirmedPeaks.bed_: Stringent confirmed peaks.
# DWeakConfirmedPeaks.bed_: Weak confirmed peaks.
# EDiscardedPeaks.bed: Stringent and weak discarded peaks.
# EStringentDiscardedPeaks.bed_: Stringent discarded peaks.
# EWeakDiscardedPeaks.bed_: Weak discarded peaks.