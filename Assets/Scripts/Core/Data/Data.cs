
namespace HollowForest.Data
{
    public class Data
    {
        public Configuration Config { get; }
        public GameData GameData { get; }

        public Data(Configuration configuration)
        {
            Config = configuration;
            GameData = new GameData();
            
            Config.InitAwake(this);
        }
    }
}