#include "..\include\p528.h"

/*=============================================================================
 |
 |       Author:  William Kozma Jr
 |                wkozma@ntia.gov
 |                US Dept of Commerce, NTIA/ITS
 |                June 2021 : Geneva Study Group 3 Meetings
 |
 *===========================================================================*/

// Data table with K-values and corresponding Nakagami-Rice distributions
const double data::NR_Data[17][18] =
{
    {
      -40.0000,   -0.1417,   -0.1252,   -0.1004,   -0.0784,   -0.0634,
       -0.0515,   -0.0321,   -0.0155,    0.0000,    0.0156,    0.0323,
        0.0518,    0.0639,    0.0791,    0.1016,    0.1271,    0.1441
    },
    {
      -25.0000,   -0.7676,   -0.6811,   -0.5497,   -0.4312,   -0.3504,
       -0.2856,   -0.1790,   -0.0870,    0.0000,    0.0878,    0.1828,
        0.2953,    0.3651,    0.4537,    0.5868,    0.7390,    0.8420
    },
    {
      -20.0000,   -1.3183,   -1.1738,   -0.9524,   -0.7508,   -0.6121,
       -0.5003,   -0.3151,   -0.1537,    0.0000,    0.1564,    0.3269,
        0.5308,    0.6585,    0.8218,    1.0696,    1.3572,    1.5544
    },
    {
      -18.0000,   -1.6263,   -1.4507,   -1.1805,   -0.9332,   -0.7623,
       -0.6240,   -0.3940,   -0.1926,    0.0000,    0.1969,    0.4127,
        0.6722,    0.8355,    1.0453,    1.3660,    1.7417,    2.0014
    },
    {
      -16.0000,   -1.9963,   -1.7847,   -1.4573,   -1.1557,   -0.9462,
       -0.7760,   -0.4916,   -0.2410,    0.0000,    0.2478,    0.5209,
        0.8519,    1.0615,    1.3326,    1.7506,    2.2463,    2.5931
    },
    {
      -14.0000,   -2.4355,   -2.1829,   -1.7896,   -1.4247,   -1.1695,
       -0.9613,   -0.6113,   -0.3007,    0.0000,    0.3114,    0.6573,
        1.0802,    1.3505,    1.7028,    2.2526,    2.9156,    3.3872
    },
    {
      -12.0000,   -2.9491,   -2.6507,   -2.1831,   -1.7455,   -1.4375,
       -1.1846,   -0.7567,   -0.3737,    0.0000,    0.3903,    0.8281,
        1.3698,    1.7198,    2.1808,    2.9119,    3.8143,    4.4714
    },
    {
      -10.0000,   -3.5384,   -3.1902,   -2.6407,   -2.1218,   -1.7535,
       -1.4495,   -0.9307,   -0.4619,    0.0000,    0.4874,    1.0404,
        1.7348,    2.1898,    2.7975,    3.7820,    5.0373,    5.9833
    },
    {
       -8.0000,   -4.1980,   -3.7974,   -3.1602,   -2.5528,   -2.1180,
       -1.7565,   -1.1345,   -0.5662,    0.0000,    0.6045,    1.2999,
        2.1887,    2.7814,    3.5868,    4.9288,    6.7171,    8.1319
    },
    {
       -6.0000,   -4.9132,   -4.4591,   -3.7313,   -3.0306,   -2.5247,
       -2.1011,   -1.3655,   -0.6855,    0.0000,    0.7415,    1.6078,
        2.7374,    3.5059,    4.5714,    6.4060,    8.9732,   11.0973
    },
    {
       -4.0000,   -5.6559,   -5.1494,   -4.3315,   -3.5366,   -2.9578,
       -2.4699,   -1.6150,   -0.8154,    0.0000,    0.8935,    1.9530,
        3.3611,    4.3363,    5.7101,    8.1216,   11.5185,   14.2546
    },
    {
       -2.0000,   -6.3810,   -5.8252,   -4.9219,   -4.0366,   -3.3871,
       -2.8364,   -1.8638,   -0.9455,    0.0000,    1.0458,    2.2979,
        3.9771,    5.1450,    6.7874,    9.6276,   13.4690,   16.4251
    },
    {
        0.0000,   -7.0247,   -6.4249,   -5.4449,   -4.4782,   -3.7652,
       -3.1580,   -2.0804,   -1.0574,    0.0000,    1.1723,    2.5755,
        4.4471,    5.7363,    7.5266,   10.5553,   14.5401,   17.5511
    },
    {
        2.0000,   -7.5229,   -6.8862,   -5.8424,   -4.8090,   -4.0446,
       -3.3927,   -2.2344,   -1.1347,    0.0000,    1.2535,    2.7446,
        4.7144,    6.0581,    7.9073,   11.0003,   15.0270,   18.0526
    },
    {
        4.0000,   -7.8532,   -7.1880,   -6.0963,   -5.0145,   -4.2145,
       -3.5325,   -2.3227,   -1.1774,    0.0000,    1.2948,    2.8268,
        4.8377,    6.2021,    8.0724,   11.1869,   15.2265,   18.2566
    },
    {
        6.0000,   -8.0435,   -7.3588,   -6.2354,   -5.1234,   -4.3022,
       -3.6032,   -2.3656,   -1.1975,    0.0000,    1.3130,    2.8619,
        4.8888,    6.2610,    8.1388,   11.2607,   15.3047,   18.3361
    },
    {
       20.0000,   -8.2238,   -7.5154,   -6.3565,   -5.2137,   -4.3726,
       -3.6584,   -2.3979,   -1.2121,    0.0000,    1.3255,    2.8855,
        4.9224,    6.2992,    8.1814,   11.3076,   15.3541,   18.3864
    }
};

// Probabilities for interpolation and data tables
const double data::P[17] = { 1, 2, 5, 10, 15, 20, 30, 40, 50,
    60, 70, 80, 85, 90, 95, 98, 99 };