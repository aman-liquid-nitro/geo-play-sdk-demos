using UnityEngine;

namespace GeoPlaySample.InfiniteRunner
{
	[CreateAssetMenu(fileName = "NewHeadMount", menuName = "SO/Store/Head Mounts")]
	public class StoreItemSO : ScriptableObject
	{
		public string name;
		public int id;
		public Sprite sprite;

		[Header("Transform")]
		public Vector3 localPosition;
		public float zRotation;
		public float localScale = 1;
	}
}