using System.Collections.Generic;
using UnityEngine;

namespace GeoPlaySample.InfiniteRunner
{
    public class StoreManager : MonoBehaviour
    {
        public const string PLAYER_PREF_SELECTED_STORE_ITEM = "Pref_SelectedStoreItem";

        [SerializeField] private StoreItemsDB _itemsDB;
        [SerializeField] private GameObject storeCanvasObject;
        [SerializeField] private List<int> availableItems = new List<int>{ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

        [Header("UIs")]
        [SerializeField] private StoreItem storeItemPrefab;
        [SerializeField] private StoreItem defaultStoreItem;

        private List<StoreItem> populatedItems = new List<StoreItem>();

        private void OnEnable()
        {
            StoreItem.OnSelectItem += OnSelectStoreItem;
            GameManager.OnSetGameConfig += ApplyConfig;
        }

        private void OnDisable()
        {
            StoreItem.OnSelectItem -= OnSelectStoreItem;
            GameManager.OnSetGameConfig -= ApplyConfig;
        }

        private void InitDefaultItem()
        {
            if (defaultStoreItem)
                defaultStoreItem.Init(_itemsDB.defaultItem);

            if (!populatedItems.Contains(defaultStoreItem))
                populatedItems.Add(defaultStoreItem);
        }

        public void PopulateAvailableItems()
        {
            ClearPopulatedItems();
            InitDefaultItem();

            foreach (var itemId in availableItems)
            {
                StoreItemSO itemSO = _itemsDB.GetItem(itemId);
                if (itemSO != null)
                {
                    var item = Instantiate(storeItemPrefab, defaultStoreItem.transform.parent);
                    item.Init(itemSO);

                    if (!populatedItems.Contains(item))
                        populatedItems.Add(item);
                }
            }
        }

        private void ClearPopulatedItems()
        {
            foreach (var item in populatedItems)
            {
                if (item.ID != 0)
                    Destroy(item.gameObject);
            }
            populatedItems.Clear();
        }

        void OnAppInitialized()
        {
            PopulateAvailableItems();
            
            // Auto select item in prefs
            OnSelectStoreItem(GetSelectedItemId());
        }

        public void EnterStore()
        {
            storeCanvasObject.SetActive(true);
            AnalyticsHandler.Log("enter_store");
        }

        public void ExitStore()
        {
            storeCanvasObject.SetActive(false);
            AnalyticsHandler.Log("exit_store");
        }

        private void OnSelectStoreItem(int id)
        {
            PlayerPrefs.SetInt(PLAYER_PREF_SELECTED_STORE_ITEM, id);
            PlayerPrefs.Save();

            foreach (var item in populatedItems)
            {
                item.OnItemSelected(item.ID == id);
            }
        }

        public static int GetSelectedItemId()
        {
            return PlayerPrefs.GetInt(PLAYER_PREF_SELECTED_STORE_ITEM, 0);
        }

        private void ApplyConfig(GameConfig config)
        {
            availableItems = config.storeConfig.availableItems;
            OnAppInitialized();
        }
    }
}