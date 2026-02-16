using System;
using UnityEngine;

namespace GeoPlaySample.InfiniteRunner
{
    public class StoreItem : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Image itemImage;
        [SerializeField] private UnityEngine.UI.Image selectedImage;

        private StoreItemSO data;
        private bool isInit = false;

        public int ID => data.id;

        public static event Action<int> OnSelectItem;

        public void Init(StoreItemSO data)
        {
            if (isInit) return;

            this.data = data;
            SetItemSprite(data.sprite);
            isInit = true;
        }

        public void SetItemSprite(Sprite spr)
        {
            itemImage.sprite = spr;
        }

        public void OnClickItem()
        {
            OnSelectItem?.Invoke(data.id);
        }

        public void OnItemSelected(bool isSelected)
        {
            selectedImage.gameObject.SetActive(isSelected);
        }
    }
}