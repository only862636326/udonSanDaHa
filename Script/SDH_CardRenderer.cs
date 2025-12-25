
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using static Unity.Collections.AllocatorManager;


namespace HopeSDH
{

    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SDH_CardRenderer : UdonSharpBehaviour
    {
        void Start()
        {
            for (int i = 0; i < 52; i++)
            {
                SetNullShow(Color.white, i, false);
            }
        }

        private Renderer _renderer;
        private MaterialPropertyBlock _block;
        private bool _is_block_init = false;
        public void SetNullShow(Color c, int card_id, bool is_null)
        {
            // if(!this._is_block_init)
            {
                _renderer = this.transform.GetChild(card_id).GetComponent<Renderer>();
                this._block = new MaterialPropertyBlock();
                _renderer.GetPropertyBlock(_block);
            }

            if (!is_null)
            {
                var y = card_id % 13;
                var x = 4 - card_id / 13;
                // Debug.Log($"{idx_1}, { x},{y}");
                this._block.SetVector("_MainParams", new Vector4(x, y, 5, 13));
                this._renderer.SetPropertyBlock(this._block);
            }
            else
            {
                this._block.SetVector("_MainParams", new Vector4(5, 0, 5, 3));
                this._renderer.SetPropertyBlock(this._block);
            }
        }
    }
}