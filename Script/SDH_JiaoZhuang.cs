
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeSDH
{
    public class SDH_JiaoZhuang : UdonSharpBehaviour
    {
        #region init code
        private bool _is_init = false;

        [SerializeField] private Text[] _text_tips;
        public void Init()
        {
            if (this._is_init)
                return;
            this._is_init = true;

            // user code init here

            var _n = this.transform.childCount;
            _text_tips = new Text[_n];
            for (int i = 0; i < _n; i++)
            {
                var tf = this.transform.GetChild(i);

                foreach (Transform child in tf)
                {
                    var _low = child.name.ToLower();
                    if (_low.Contains("tips") && _low.Contains("text"))
                    {
                        _text_tips[i] = child.GetComponent<Text>();
                        break;
                    }
                }
            }
        }

        private HopeTools.HopeUdonFramework hugf;
        public object eventData;
        public void HugfInit()
        {
            if (hugf == null)
            {
                hugf = GameObject.Find(SDH_GameManager.CONST_SDH_HUGF_STRING).GetComponent<HopeTools.HopeUdonFramework>();
                if (hugf == null)
                {
                    Debug.LogError("HugfInit failed, hugf is null!");
                    return;
                }

                hugf.Init();
                return;
            }
        }

        public void HugfInitAfter()
        {
            // user code after hugf init here
            //hugf.udonEvn.RegisterListener(nameof(this.DemeFunCall), this);
        }


        public void HufgIocGet()
        {
            //var p = (Transform[])hugf.udonIoc.GetServiceObj(nameof(SDH_FaPaiJi.card_tf_list));
        }

        //public void DemeFunCall()
        //{
        //    this.eventData = data;
        //}
        #endregion end init code

        #region syn

        void RequestSyn()
        {
#if !UNITY_EDITOR
            if (!Networking.IsOwner(this.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            }
            RequestSerialization();
#else
            OnPreSerialization();
#endif
            ;
        }

        public override void OnPreSerialization()
        {

            //DebugSynData();
        }

        public override void OnDeserialization()
        {

            //DebugSynData();
        }

        public void DebugSynData()
        {

        }
        #endregion end syn


        public void ToggleEvn_Score(int score_idx, int idx)
        {
            //Debug.Log($"ToggleEvn_Score called with score: {score}, idx: {idx}");
            // implement your logic here
            var srore = 80 - score_idx * 5;
            _text_tips[idx].text = $"{srore}分";
        }

		public void ToggleEvn_JiaoZhuang(int idx)
        {

            //Debug.Log($"ToggleEvn_JiaoZhuang called with idx: {idx}");
           
        }

        public void ToggleEvn_BuJiao(int idx)
        {
            //Debug.Log($"ToggleEvn_BuJiao called with idx: {idx}");
        }

        #region start method
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
        // end method
        #endregion end method
    }
}
