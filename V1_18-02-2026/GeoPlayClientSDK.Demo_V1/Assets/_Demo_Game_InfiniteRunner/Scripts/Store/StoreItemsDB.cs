using System.Linq;
using UnityEngine;

namespace GeoPlaySample.InfiniteRunner
{
	[CreateAssetMenu(fileName = "StoreItemsDB", menuName = "SO/Store/Store Items DB")]
	public class StoreItemsDB : ScriptableObject
	{
		public StoreItemSO defaultItem;
		public StoreItemSO[] items;

		public StoreItemSO GetItem(int id)
		{
			return items.FirstOrDefault(x => x.id == id);
		}
    }
}