
using HopeSDH;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SDH_Config : UdonSharpBehaviour
    {
        public const string SDH_CONFIG_Singleton_String = "SDH_CONFIG_Singleton_String";
        #region init code
        private bool _is_init = false;
        public void Init()
        {
            if (this._is_init)
                return;
            this._is_init = true;

            // user code init here
            
        }

        private HopeTools.HopeUdonFramework hugf;
        public object eventData;
        public object eventData1; // eventData1 is the same as eventData (eventData1 = eventData)
        public object eventData2;
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
            hugf.udonIoc.RegisterSingleton(SDH_CONFIG_Singleton_String, this, this);
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

        private int[] _sort_temp_list;
        public  int[] sort_id_list;
        public int[] id_in_sorted_list;
        public int zhu_icon;
        public int zhuang_player;
        public int zhuang_jiaoscore;


        public void ConfigSortIdList(int _icon)
        {
            if (sort_id_list == null)
            {
                this.sort_id_list = new int[SDH_GameManager.CONST_SHOW_CARD_NUM];
                this.id_in_sorted_list = new int[SDH_GameManager.CONST_SHOW_CARD_NUM];
            }

            var _all_num = sort_id_list.Length;

            var _sort_idx = 0;
            sort_id_list[_sort_idx++] = _all_num - 1;
            sort_id_list[_sort_idx++] = _all_num - 2;
            sort_id_list[_sort_idx++] = _all_num - 3;
            sort_id_list[_sort_idx++] = _all_num - 4;

            int _num;
            _num = 7;
            if (_icon >= 0 && _icon < 4)
            {
                sort_id_list[_sort_idx++] = _icon * 26 + (_num * 2 - 1);
                sort_id_list[_sort_idx++] = _icon * 26 + (_num * 2 - 2);
            }

            for (var _t = 0; _t < 4; _t++)
            {
                if (_t == _icon)
                    continue;
                sort_id_list[_sort_idx++] = _t * 26 + (_num * 2 - 1);
                sort_id_list[_sort_idx++] = _t * 26 + (_num * 2 - 2);
            }

            _num = 2;
            if (_icon >= 0 && _icon < 4)
            {
                sort_id_list[_sort_idx++] = _icon * 26 + (_num * 2 - 1);
                sort_id_list[_sort_idx++] = _icon * 26 + (_num * 2 - 2);
            }

            for (var _t = 0; _t < 4; _t++)
            {
                if (_t == _icon)
                    continue;
                sort_id_list[_sort_idx++] = _t * 26 + (_num * 2 - 1);
                sort_id_list[_sort_idx++] = _t * 26 + (_num * 2 - 2);
            }

            // 
            if (_icon >= 0 && _icon < 4)
            {
                _num = 1;
                sort_id_list[_sort_idx++] = _icon * 26 + (_num * 2 - 1);
                sort_id_list[_sort_idx++] = _icon * 26 + (_num * 2 - 2);

                for (int i = 13; i >= 3; i--)
                {
                    if (i == 7)
                        continue;
                    _num = i;
                    sort_id_list[_sort_idx++] = _icon * 26 + (_num * 2 - 1);
                    sort_id_list[_sort_idx++] = _icon * 26 + (_num * 2 - 2);
                }
            }

            for (var _t = 0; _t < 4; _t++)
            {
                if (_t == _icon)
                    continue;

                _num = 1;

                sort_id_list[_sort_idx++] = _t * 26 + (_num * 2 - 1);
                sort_id_list[_sort_idx++] = _t * 26 + (_num * 2 - 2);

                for (int i = 13; i >= 3; i--)
                {
                    if (i == 7)
                        continue;
                    _num = i;
                    sort_id_list[_sort_idx++] = _t * 26 + (_num * 2 - 1);
                    sort_id_list[_sort_idx++] = _t * 26 + (_num * 2 - 2);
                }
            }

            for (int i = 0; i < _all_num; i++)
            {
                // 记录每个牌的排序索引
                id_in_sorted_list[sort_id_list[i]] = i;
            }
        }

        


        #region syn

        void RequestSyn()
        {
#if !UNITY_EDITOR
            if(!Networking.IsOwner(this.gameObject))
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


        // start method

        // end method
    }
}