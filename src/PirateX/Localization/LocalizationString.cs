using System.Resources;

namespace PirateX.Localization
{
    public class LocalizationString
    {
        private ResourceManager rm = null;

        public LocalizationString(ResourceManager resourceManager)
        {
            rm = resourceManager; 
        }

    }
}
