namespace PirateX.Config
{
    public class ConfigIndex
    {
        public string[] Names { get; private set; }

        public ConfigIndex(params string[] names)
        {
            Names = names; 
        }
    }
}
