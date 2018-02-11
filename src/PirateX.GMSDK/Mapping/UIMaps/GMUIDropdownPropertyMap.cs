namespace PirateX.GMSDK.Mapping
{
    /// <summary>
    /// 下拉框
    /// </summary>
    public class GMUIDropdownPropertyMap:GMUIPropertyMap<GMUIDropdownPropertyMap>
    {
        public IGMUIListDataProvider ListDataProvider { get; private set; }

        public GMUIDropdownPropertyMap ToListDataProvider(IGMUIListDataProvider listDataProvider)
        {
            this.ListDataProvider = listDataProvider;
            return this as GMUIDropdownPropertyMap;
        }
    }
}
