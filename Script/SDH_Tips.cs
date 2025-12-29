
using HopeSDH;
using HopeTools;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;



namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SDH_Tips : UdonSharpBehaviour
    {
        #region init code
        private bool _is_init = false;

        private GameObject []_obj_tips_list;
        public void Init()
        {
            if (this._is_init)
                return;
            this._is_init = true;

            // user code init here

            foreach (Transform tf in this.transform)
            {
                var _low = tf.name.ToLower();

                if (_low.Contains("active") && _low.Contains("prt"))
                {
                    _obj_tips_list = new GameObject[tf.childCount];
                    for (int i = 0; i < tf.childCount; i++)
                    {
                        _obj_tips_list[i] = tf.GetChild(i).gameObject;
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
            hugf.udonEvn.RegisterListener(nameof(this.SetActivePlayerCall), this);
        }


        public void HufgIocGet()
        {
            //var p = (Transform[])hugf.udonIoc.GetServiceObj(nameof(SDH_FaPaiJi.card_tf_list));
        }

        public void SetActivePlayerCall()
        {
            int x = (int)eventData;
            for (int i = 0; i < _obj_tips_list.Length; i++)
            {
                _obj_tips_list[i].SetActive(i == x);
            }
        }

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