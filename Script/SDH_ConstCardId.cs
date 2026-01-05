
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeSDH
{
    public class SDH_ConstCardId : UdonSharpBehaviour
    {
        public const int CARD_HEI_BASE = 0x00;
        public const int CARD_HEI_A_0 = CARD_HEI_BASE + 0;
        public const int CARD_HEI_A_1 = CARD_HEI_BASE + 1;
        public const int CARD_HEI_2_0 = CARD_HEI_BASE + 2;
        public const int CARD_HEI_2_1 = CARD_HEI_BASE + 3;
        public const int CARD_HEI_3_0 = CARD_HEI_BASE + 4;
        public const int CARD_HEI_3_1 = CARD_HEI_BASE + 5;
        public const int CARD_HEI_4_0 = CARD_HEI_BASE + 6;
        public const int CARD_HEI_4_1 = CARD_HEI_BASE + 7;
        public const int CARD_HEI_5_0 = CARD_HEI_BASE + 8;
        public const int CARD_HEI_5_1 = CARD_HEI_BASE + 9;
        public const int CARD_HEI_6_0 = CARD_HEI_BASE + 10;
        public const int CARD_HEI_6_1 = CARD_HEI_BASE + 11;
        public const int CARD_HEI_7_0 = CARD_HEI_BASE + 12;
        public const int CARD_HEI_7_1 = CARD_HEI_BASE + 13;
        public const int CARD_HEI_8_0 = CARD_HEI_BASE + 14;
        public const int CARD_HEI_8_1 = CARD_HEI_BASE + 15;
        public const int CARD_HEI_9_0 = CARD_HEI_BASE + 16;
        public const int CARD_HEI_9_1 = CARD_HEI_BASE + 17;
        public const int CARD_HEI_10_0 = CARD_HEI_BASE + 18;
        public const int CARD_HEI_10_1 = CARD_HEI_BASE + 19;
        public const int CARD_HEI_J_0 = CARD_HEI_BASE + 20;
        public const int CARD_HEI_J_1 = CARD_HEI_BASE + 21;
        public const int CARD_HEI_Q_0 = CARD_HEI_BASE + 22;
        public const int CARD_HEI_Q_1 = CARD_HEI_BASE + 23;
        public const int CARD_HEI_K_0 = CARD_HEI_BASE + 24;
        public const int CARD_HEI_K_1 = CARD_HEI_BASE + 25;


        public const int CARD_HONG_BASE = 0x20;
        public const int CARD_HONG_A_0 = CARD_HONG_BASE + 0;
        public const int CARD_HONG_A_1 = CARD_HONG_BASE + 1;
        public const int CARD_HONG_2_0 = CARD_HONG_BASE + 2;
        public const int CARD_HONG_2_1 = CARD_HONG_BASE + 3;
        public const int CARD_HONG_3_0 = CARD_HONG_BASE + 4;
        public const int CARD_HONG_3_1 = CARD_HONG_BASE + 5;
        public const int CARD_HONG_4_0 = CARD_HONG_BASE + 6;
        public const int CARD_HONG_4_1 = CARD_HONG_BASE + 7;
        public const int CARD_HONG_5_0 = CARD_HONG_BASE + 8;
        public const int CARD_HONG_5_1 = CARD_HONG_BASE + 9;
        public const int CARD_HONG_6_0 = CARD_HONG_BASE + 10;
        public const int CARD_HONG_6_1 = CARD_HONG_BASE + 11;
        public const int CARD_HONG_7_0 = CARD_HONG_BASE + 12;
        public const int CARD_HONG_7_1 = CARD_HONG_BASE + 13;
        public const int CARD_HONG_8_0 = CARD_HONG_BASE + 14;
        public const int CARD_HONG_8_1 = CARD_HONG_BASE + 15;
        public const int CARD_HONG_9_0 = CARD_HONG_BASE + 16;
        public const int CARD_HONG_9_1 = CARD_HONG_BASE + 17;
        public const int CARD_HONG_10_0 = CARD_HONG_BASE + 18;
        public const int CARD_HONG_10_1 = CARD_HONG_BASE + 19;
        public const int CARD_HONG_J_0 = CARD_HONG_BASE + 20;
        public const int CARD_HONG_J_1 = CARD_HONG_BASE + 21;
        public const int CARD_HONG_Q_0 = CARD_HONG_BASE + 22;
        public const int CARD_HONG_Q_1 = CARD_HONG_BASE + 23;
        public const int CARD_HONG_K_0 = CARD_HONG_BASE + 24;
        public const int CARD_HONG_K_1 = CARD_HONG_BASE + 25;

        public const int CARD_MEI_BASE = 0x40;
        public const int CARD_MEI_A_0 = CARD_MEI_BASE + 0;
        public const int CARD_MEI_A_1 = CARD_MEI_BASE + 1;
        public const int CARD_MEI_2_0 = CARD_MEI_BASE + 2;
        public const int CARD_MEI_2_1 = CARD_MEI_BASE + 3;
        public const int CARD_MEI_3_0 = CARD_MEI_BASE + 4;
        public const int CARD_MEI_3_1 = CARD_MEI_BASE + 5;
        public const int CARD_MEI_4_0 = CARD_MEI_BASE + 6;
        public const int CARD_MEI_4_1 = CARD_MEI_BASE + 7;
        public const int CARD_MEI_5_0 = CARD_MEI_BASE + 8;
        public const int CARD_MEI_5_1 = CARD_MEI_BASE + 9;
        public const int CARD_MEI_6_0 = CARD_MEI_BASE + 10;
        public const int CARD_MEI_6_1 = CARD_MEI_BASE + 11;
        public const int CARD_MEI_7_0 = CARD_MEI_BASE + 12;
        public const int CARD_MEI_7_1 = CARD_MEI_BASE + 13;
        public const int CARD_MEI_8_0 = CARD_MEI_BASE + 14;
        public const int CARD_MEI_8_1 = CARD_MEI_BASE + 15;
        public const int CARD_MEI_9_0 = CARD_MEI_BASE + 16;
        public const int CARD_MEI_9_1 = CARD_MEI_BASE + 17;
        public const int CARD_MEI_10_0 = CARD_MEI_BASE + 18;
        public const int CARD_MEI_10_1 = CARD_MEI_BASE + 19;
        public const int CARD_MEI_J_0 = CARD_MEI_BASE + 20;
        public const int CARD_MEI_J_1 = CARD_MEI_BASE + 21;
        public const int CARD_MEI_Q_0 = CARD_MEI_BASE + 22;
        public const int CARD_MEI_Q_1 = CARD_MEI_BASE + 23;
        public const int CARD_MEI_K_0 = CARD_MEI_BASE + 24;
        public const int CARD_MEI_K_1 = CARD_MEI_BASE + 25;

        public const int CARD_FANG_BASE = 0x60;
        public const int CARD_FANG_A_0 = CARD_FANG_BASE + 0;
        public const int CARD_FANG_A_1 = CARD_FANG_BASE + 1;
        public const int CARD_FANG_2_0 = CARD_FANG_BASE + 2;
        public const int CARD_FANG_2_1 = CARD_FANG_BASE + 3;
        public const int CARD_FANG_3_0 = CARD_FANG_BASE + 4;
        public const int CARD_FANG_3_1 = CARD_FANG_BASE + 5;
        public const int CARD_FANG_4_0 = CARD_FANG_BASE + 6;
        public const int CARD_FANG_4_1 = CARD_FANG_BASE + 7;
        public const int CARD_FANG_5_0 = CARD_FANG_BASE + 8;
        public const int CARD_FANG_5_1 = CARD_FANG_BASE + 9;
        public const int CARD_FANG_6_0 = CARD_FANG_BASE + 10;
        public const int CARD_FANG_6_1 = CARD_FANG_BASE + 11;
        public const int CARD_FANG_7_0 = CARD_FANG_BASE + 12;
        public const int CARD_FANG_7_1 = CARD_FANG_BASE + 13;
        public const int CARD_FANG_8_0 = CARD_FANG_BASE + 14;
        public const int CARD_FANG_8_1 = CARD_FANG_BASE + 15;
        public const int CARD_FANG_9_0 = CARD_FANG_BASE + 16;
        public const int CARD_FANG_9_1 = CARD_FANG_BASE + 17;
        public const int CARD_FANG_10_0 = CARD_FANG_BASE + 18;
        public const int CARD_FANG_10_1 = CARD_FANG_BASE + 19;
        public const int CARD_FANG_J_0 = CARD_FANG_BASE + 20;
        public const int CARD_FANG_J_1 = CARD_FANG_BASE + 21;
        public const int CARD_FANG_Q_0 = CARD_FANG_BASE + 22;
        public const int CARD_FANG_Q_1 = CARD_FANG_BASE + 23;
        public const int CARD_FANG_K_0 = CARD_FANG_BASE + 24;
        public const int CARD_FANG_K_1 = CARD_FANG_BASE + 25;

        public const int CARD_FANG_SMALL_JOKER_0 = 104;
        public const int CARD_FANG_SMALL_JOKER_1 = 105;
        public const int CARD_FANG_BIG_JOKER_0 = 106;
        public const int CARD_FANG_BIG_JOKER_1 = 107;

    }
}