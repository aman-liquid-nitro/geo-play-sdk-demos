using System.Collections.Generic;

namespace GeoPlaySample.InfiniteRunner
{
    [System.Serializable]
    public class GameConfig
    {
        public StoreConfig storeConfig;
        public FeaturesConfig featuresConfig;

        public GameConfig() 
        {
            storeConfig = new StoreConfig();
            featuresConfig = new FeaturesConfig();
        }
    }

    [System.Serializable]
    public class StoreConfig
    {
        public List<int> availableItems;

        public StoreConfig()
        {
            availableItems = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
        }
    }

    [System.Serializable]
    public class FeaturesConfig
    {
        public List<Feature> features;

        public FeaturesConfig() 
        {
            features = new List<Feature>() 
            {
                new Feature("store", true),
                new Feature("leaderboard", true)
            };
        }
    }

    [System.Serializable]
    public class Feature
    {
        public string id;
        public bool enabled;

        public Feature() { }

        public Feature(string featureId, bool isEnabled)
        {
            id = featureId;
            enabled = isEnabled;
        }
    }
}
