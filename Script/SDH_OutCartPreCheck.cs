
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


namespace HopeSDH
{
    public class SDH_OutCartPreCheck : UdonSharpBehaviour
    {
        #region init code
        private bool _is_init = false;
        public void Init()
        {
            if (this._is_init)
                return;
            this._is_init = true;

            // user code init here
            var n = 0;
            for (int i = 0; i < n; i++)
            {
                var tf = this.transform.GetChild(i);

                foreach (Transform child in tf)
                {
                    var _low = child.name.ToLower();
                    if (_low.Contains("tips") && _low.Contains("text"))
                    {
                        ;
                    }
                }
            }
        }

        [HideInInspector] public HopeTools.HopeUdonFramework hugf;
        public object eventData;
        public object eventData1; // eventData1 is the same as eventData (eventData1 = eventData)
        public object eventData2;

        public void HugfInitAfter()
        {
            // user code after hugf init here
            //hugf.udonEvn.RegisterListener(nameof(this.DemeFunCall), this);
            //hugf.udonIoc.RegisterSingleton(nameof(this.card_tf_list), this, this.card_tf_list);
        }


        public void HufgIocGet()
        {
            //var p = (Transform[])hugf.udonIoc.GetServiceObj(nameof(SDH_FaPaiJi.card_tf_list));
        }



        [SerializeField] private int[] _select_card_id_list;
        [SerializeField] private int _select_card_num;

        public void SelecCardCall()
        {
            if (_select_card_num >= _select_card_id_list.Length) return;
            var _id = (int)this.eventData;
            for (int i = 0; i < this._select_card_num; i++)
            {
                if (this._select_card_id_list[i] == _id)
                    return;
            }
            this._select_card_id_list[this._select_card_num++] = _id;
        }

        public void UnselecCardCall()
        {
            if (_select_card_num <= 0) return;

            var _id = (int)this.eventData;

            var _has = false;
            for (int i = 0; i < this._select_card_num; i++)
            {
                if (this._select_card_id_list[i] == _id)
                {
                    _has = true;
                }
                if (_has)
                {
                    this._select_card_id_list[i] = this._select_card_id_list[this._select_card_num - 1];
                }
            }
            if (this._select_card_num >= this._select_card_id_list.Length)
            {
                hugf.udondebug.LogWarning($"UnselecCardCall: {_id}, select_card_num: {this._select_card_num}");
                return;
            }
            this._select_card_num--;
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