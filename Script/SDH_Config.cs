
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


        private int[] _sort_temp_list;
        public int[] sort_id_list;
        public int[] id_in_sorted_list;
        public int zhu_icon;
        public int zhuang_player;
        public int zhuang_jiaoscore;


        #region init code
        private bool _is_init = false;
        public void Init()
        {
            if (this._is_init)
                return;
            this._is_init = true;

            // user code init here


             //ConfigSortIdList(SDH_GameManager.CONST_ICON_JOKER);
            this.zhu_icon = SDH_GameManager.CONST_ICON_JOKER;
            this.zhuang_player = 0;
            this.zhuang_jiaoscore = 80;
        
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