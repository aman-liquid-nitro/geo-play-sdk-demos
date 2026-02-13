using System;
using UnityEngine;

namespace GeoPlaySample.InfiniteRunner
{
    public class StoreItem : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Image itemImage;
        [SerializeField] private UnityEngine.UI.Image selectedImage;

        private StoreItemSO data;

        public int ID => data.id;

        public static event Action<int> OnSelectItem;

        public void Init(StoreItemSO data)
        {
            this.data = data;
            SetItemSprite(data.sprite);
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