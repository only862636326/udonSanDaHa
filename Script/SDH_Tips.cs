
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;



namespace HopeSDH
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SDH_Tips : UdonSharpBehaviour
    {
        #region init code
        private bool _is_init = false;

        private GameObject []_obj_tips_list;
        private Transform [] _tf_game_info;

        private GameObject start_but;

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
                if (_low.Contains("info") && _low.Contains("prt"))
                {
                    _tf_game_info = new Transform[tf.childCount];
                    for (int i = 0; i < tf.childCount; i++)
                    {
                        _tf_game_info[i] = tf.GetChild(i);
                    }
                }
                if (_low.Contains("toggle") && _low.Contains("start"))
                {
                    start_but = tf.gameObject;
                }
            }
        }

        public HopeTools.HopeUdonFramework hugf;
        public object eventData;

        public void HugfInitAfter()
        {
            // user code after hugf init here
            hugf.udonEvn.RegisterListener(nameof(this.SetActivePlayerCall), this);
            hugf.udonEvn.RegisterListener(nameof(this.SetZhuangPlayerCall), this);
            hugf.udonEvn.RegisterListener(nameof(this.SetZhuIconShowCall), this);
            hugf.udonEvn.RegisterListener(nameof(this.SetJiaoFenShowCall), this);
            hugf.udonEvn.RegisterListener(nameof(this.SetDeFenShowCall), this);

            hugf.udonEvn.RegisterListener(nameof(SDH_JiaoZhuang.StartJiaoCall), this);

            hugf.udonEvn.RegisterListener(nameof(this.SDH_GameResetCall), this);

            StartJiaoCall();
        }

        public void SDH_GameResetCall()
        {
            if (this.start_but != null)
                this.start_but.SetActive(true);
            StartJiaoCall();
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

        public void SetZhuIconShowCall()
        {
            var x = (int)eventData;
            for (int i = 0; i < _tf_game_info.Length; i++)
            {
                var prt = _tf_game_info[i].GetChild(0);
                for (int j = 0; j < prt.childCount; j++)
                {
                    prt.GetChild(j).gameObject.SetActive(j == x);
                }
            }
        }

        public void StartJiaoCall()
        {
            for (int i = 0; i < _tf_game_info.Length; i++)
            {
                _tf_game_info[i].GetComponent<Image>().color = Color.white * 0.7f;
                _tf_game_info[i].Find("Text_JiaoFen").GetComponent<Text>().text = "";
                _tf_game_info[i].Find("Text_DeFen").GetComponent<Text>().text = "";
                _obj_tips_list[i].SetActive(false);

                var prt = _tf_game_info[i].GetChild(0);
                for (int j = 0; j < prt.childCount; j++)
                {
                    prt.GetChild(j).gameObject.SetActive(false);
                }
            }
        }

        public void SetZhuangPlayerCall()
        {
            int x = (int)eventData;
            for (int i = 0; i < _tf_game_info.Length; i++)
            {
                if (i == x)
                {
                    _tf_game_info[i].GetComponent<Image>().color = Color.green * 0.7f;
                }
                else
                {
                    _tf_game_info[i].GetComponent<Image>().color = Color.white * 0.7f;
                }
            }
        }

        public void SetJiaoFenShowCall()
        {
            var x = (int)eventData;
            for (int i = 0; i < _tf_game_info.Length; i++)
            {
                _tf_game_info[i].Find("Text_JiaoFen").GetComponent<Text>().text = "叫分 "+ x;
            }
        }


        public void SetDeFenShowCall()
        {
            var x = (int)eventData;
            for (int i = 0; i < _tf_game_info.Length; i++)
            {
                _tf_game_info[i].Find("Text_DeFen").GetComponent<Text>().text = "得分 "+ x;
            }
        }



        #endregion end init code




        // start method

        // end method
    }
}