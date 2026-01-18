
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeSDH
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SDH_Input : UdonSharpBehaviour
    {
        [HideInInspector] public HopeTools.HopeUdonFramework hugf;
        public object eventData;
        public object eventData1;
        public object eventData2;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                hugf.TriggerReEvent(nameof(SDH_OutCartFsm.StartChuPaiCall));
            }
            this.eventData = 10;
            if (Input.GetKeyDown(KeyCode.P))
            {
                hugf.TriggerReEventWithData(nameof(SDH_FaPaiJi.StartShuffleCall), this.eventData);
            }
        }


        public void HugfInitAfter()
        {
            // user code after hugf init here
            //hugf.udonEvn.RegisterListener(nameof(this.DemeFunCall), this);
            hugf.udonIoc.RegisterSingleton(nameof(SDH_Input), this, this);
        }

        public void ToggleEvn_Score(int score_idx, int idx)
        {
            hugf.TriggerReEventWith2Data(nameof(SDH_JiaoZhuang.ToggleEvn_ScoreCall), idx, score_idx);
        }

        public void ToggleEvn_JiaoZhuang(int idx)
        {
            hugf.TriggerReEventWithData(nameof(SDH_JiaoZhuang.ToggleEvn_JiaoZhuangCall), idx);
        }
        public void ToggleEvn_BuJiao(int idx)
        {
            hugf.TriggerReEventWithData(nameof(SDH_JiaoZhuang.ToggleEvn_BuJiaoCall), idx);
        }

        // start method
        public void ToggleEvn_Score0_0() { ToggleEvn_Score(0, 0); }
        public void ToggleEvn_Score1_0() { ToggleEvn_Score(1, 0); }
        public void ToggleEvn_Score2_0() { ToggleEvn_Score(2, 0); }
        public void ToggleEvn_Score3_0() { ToggleEvn_Score(3, 0); }
        public void ToggleEvn_Score4_0() { ToggleEvn_Score(4, 0); }
        public void ToggleEvn_Score5_0() { ToggleEvn_Score(5, 0); }
        public void ToggleEvn_Score6_0() { ToggleEvn_Score(6, 0); }
        public void ToggleEvn_Score7_0() { ToggleEvn_Score(7, 0); }
        public void ToggleEvn_Score8_0() { ToggleEvn_Score(8, 0); }
        public void ToggleEvn_Score9_0() { ToggleEvn_Score(9, 0); }
        public void ToggleEvn_Score10_0() { ToggleEvn_Score(10, 0); }
        public void ToggleEvn_Score11_0() { ToggleEvn_Score(11, 0); }
        public void ToggleEvn_Score12_0() { ToggleEvn_Score(12, 0); }
        public void ToggleEvn_Score13_0() { ToggleEvn_Score(13, 0); }
        public void ToggleEvn_Score14_0() { ToggleEvn_Score(14, 0); }
        public void ToggleEvn_Score15_0() { ToggleEvn_Score(15, 0); }
        public void ToggleEvn_JiaoZhuang_0() { ToggleEvn_JiaoZhuang(0); }
        public void ToggleEvn_BuJiao_0() { ToggleEvn_BuJiao(0); }
        public void ToggleEvn_Score0_1() { ToggleEvn_Score(0, 1); }
        public void ToggleEvn_Score1_1() { ToggleEvn_Score(1, 1); }
        public void ToggleEvn_Score2_1() { ToggleEvn_Score(2, 1); }
        public void ToggleEvn_Score3_1() { ToggleEvn_Score(3, 1); }
        public void ToggleEvn_Score4_1() { ToggleEvn_Score(4, 1); }
        public void ToggleEvn_Score5_1() { ToggleEvn_Score(5, 1); }
        public void ToggleEvn_Score6_1() { ToggleEvn_Score(6, 1); }
        public void ToggleEvn_Score7_1() { ToggleEvn_Score(7, 1); }
        public void ToggleEvn_Score8_1() { ToggleEvn_Score(8, 1); }
        public void ToggleEvn_Score9_1() { ToggleEvn_Score(9, 1); }
        public void ToggleEvn_Score10_1() { ToggleEvn_Score(10, 1); }
        public void ToggleEvn_Score11_1() { ToggleEvn_Score(11, 1); }
        public void ToggleEvn_Score12_1() { ToggleEvn_Score(12, 1); }
        public void ToggleEvn_Score13_1() { ToggleEvn_Score(13, 1); }
        public void ToggleEvn_Score14_1() { ToggleEvn_Score(14, 1); }
        public void ToggleEvn_Score15_1() { ToggleEvn_Score(15, 1); }
        public void ToggleEvn_JiaoZhuang_1() { ToggleEvn_JiaoZhuang(1); }
        public void ToggleEvn_BuJiao_1() { ToggleEvn_BuJiao(1); }
        public void ToggleEvn_Score0_2() { ToggleEvn_Score(0, 2); }
        public void ToggleEvn_Score1_2() { ToggleEvn_Score(1, 2); }
        public void ToggleEvn_Score2_2() { ToggleEvn_Score(2, 2); }
        public void ToggleEvn_Score3_2() { ToggleEvn_Score(3, 2); }
        public void ToggleEvn_Score4_2() { ToggleEvn_Score(4, 2); }
        public void ToggleEvn_Score5_2() { ToggleEvn_Score(5, 2); }
        public void ToggleEvn_Score6_2() { ToggleEvn_Score(6, 2); }
        public void ToggleEvn_Score7_2() { ToggleEvn_Score(7, 2); }
        public void ToggleEvn_Score8_2() { ToggleEvn_Score(8, 2); }
        public void ToggleEvn_Score9_2() { ToggleEvn_Score(9, 2); }
        public void ToggleEvn_Score10_2() { ToggleEvn_Score(10, 2); }
        public void ToggleEvn_Score11_2() { ToggleEvn_Score(11, 2); }
        public void ToggleEvn_Score12_2() { ToggleEvn_Score(12, 2); }
        public void ToggleEvn_Score13_2() { ToggleEvn_Score(13, 2); }
        public void ToggleEvn_Score14_2() { ToggleEvn_Score(14, 2); }
        public void ToggleEvn_Score15_2() { ToggleEvn_Score(15, 2); }
        public void ToggleEvn_JiaoZhuang_2() { ToggleEvn_JiaoZhuang(2); }
        public void ToggleEvn_BuJiao_2() { ToggleEvn_BuJiao(2); }
        public void ToggleEvn_Score0_3() { ToggleEvn_Score(0, 3); }
        public void ToggleEvn_Score1_3() { ToggleEvn_Score(1, 3); }
        public void ToggleEvn_Score2_3() { ToggleEvn_Score(2, 3); }
        public void ToggleEvn_Score3_3() { ToggleEvn_Score(3, 3); }
        public void ToggleEvn_Score4_3() { ToggleEvn_Score(4, 3); }
        public void ToggleEvn_Score5_3() { ToggleEvn_Score(5, 3); }
        public void ToggleEvn_Score6_3() { ToggleEvn_Score(6, 3); }
        public void ToggleEvn_Score7_3() { ToggleEvn_Score(7, 3); }
        public void ToggleEvn_Score8_3() { ToggleEvn_Score(8, 3); }
        public void ToggleEvn_Score9_3() { ToggleEvn_Score(9, 3); }
        public void ToggleEvn_Score10_3() { ToggleEvn_Score(10, 3); }
        public void ToggleEvn_Score11_3() { ToggleEvn_Score(11, 3); }
        public void ToggleEvn_Score12_3() { ToggleEvn_Score(12, 3); }
        public void ToggleEvn_Score13_3() { ToggleEvn_Score(13, 3); }
        public void ToggleEvn_Score14_3() { ToggleEvn_Score(14, 3); }
        public void ToggleEvn_Score15_3() { ToggleEvn_Score(15, 3); }
        public void ToggleEvn_JiaoZhuang_3() { ToggleEvn_JiaoZhuang(3); }
        public void ToggleEvn_BuJiao_3() { ToggleEvn_BuJiao(3); }

        public void ToggleEvn_JoinBut(int idx)
        {
            hugf.TriggerReEventWithData(nameof(SDH_JoinExit.ToggleEvn_JoinButCall), idx);
        }

        public void ToggleEvn_ExitBut(int idx)
        {
            hugf.TriggerReEventWithData(nameof(SDH_JoinExit.ToggleEvn_ExitButCall), idx);
        }

        public void ToggleEvn_JoinBut_0() { ToggleEvn_JoinBut(0); }
        public void ToggleEvn_ExitBut_0() { ToggleEvn_ExitBut(0); }
        public void ToggleEvn_JoinBut_1() { ToggleEvn_JoinBut(1); }
        public void ToggleEvn_ExitBut_1() { ToggleEvn_ExitBut(1); }
        public void ToggleEvn_JoinBut_2() { ToggleEvn_JoinBut(2); }
        public void ToggleEvn_ExitBut_2() { ToggleEvn_ExitBut(2); }
        public void ToggleEvn_JoinBut_3() { ToggleEvn_JoinBut(3); }
        public void ToggleEvn_ExitBut_3() { ToggleEvn_ExitBut(3); }


        public void ToggleEvn_OutBut(int idx)
        {
            hugf.TriggerReEventWithData(nameof(SDH_OutCartFsm.ToggleEvn_OutButCall), idx);
        }

        public void ToggleEvn_MaiDi(int idx)
        {
            hugf.TriggerReEventWithData(nameof(SDH_OutCartFsm.ToggleEvn_MaiDiCall), idx);
        }

        public void ToggleEvn_TipsBut(int idx)
        {
            hugf.TriggerReEventWithData(nameof(SDH_OutCartFsm.ToggleEvn_TipsButCall), idx);
        }

        public void ToggleEvn_OutBut_0() { ToggleEvn_OutBut(0); }
        public void ToggleEvn_OutBut_1() { ToggleEvn_OutBut(1); }
        public void ToggleEvn_OutBut_2() { ToggleEvn_OutBut(2); }
        public void ToggleEvn_OutBut_3() { ToggleEvn_OutBut(3); }

        public void ToggleEvn_MaiDi_0() { ToggleEvn_MaiDi(0); }
        public void ToggleEvn_MaiDi_1() { ToggleEvn_MaiDi(1); }
        public void ToggleEvn_MaiDi_2() { ToggleEvn_MaiDi(2); }
        public void ToggleEvn_MaiDi_3() { ToggleEvn_MaiDi(3); }

        public void ToggleEvn_TipsBut_0() { ToggleEvn_TipsBut(0); }
        public void ToggleEvn_TipsBut_1() { ToggleEvn_TipsBut(1); }
        public void ToggleEvn_TipsBut_2() { ToggleEvn_TipsBut(2); }
        public void ToggleEvn_TipsBut_3() { ToggleEvn_TipsBut(3); }

        public void ToggleEvn_UnselecCard(int card_id)
        {
            hugf.TriggerReEventWithData(nameof(SDH_OutCartFsm.UnselecCardCall), card_id);
        }
        public void ToggleEvn_SelecCard(int card_id)
        {
            hugf.TriggerReEventWithData(nameof(SDH_OutCartFsm.SelecCardCall), card_id);
        }
        public void ToggleEvn_ClickCard(int card_id)
        {
            //hugf.TriggerReEventWithData(nameof(SDH_OutCartFsm.ClickCardCall), card_id);
        }

        // end method
    }
}



