namespace PirateX.GMSDK.Mapping
{
    /// <summary>
    /// 表示控件对应了一个实体
    /// </summary>
    public class GMUIMapPropertyMap: GMUIPropertyMap<GMUIMapPropertyMap>
    {
        public IGMUIItemMap Map { get; private set; }

        public GMUIMapPropertyMap ToPropertyMap(IGMUIItemMap propertyMap)
        {
            ToGroupName(this.DisplayName);
            Map = propertyMap;
            return this ;
        }
    }

}
