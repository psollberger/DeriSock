// --------------------------------------------------------------------------
// <auto-generated>
//      This code was generated by a tool.
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
// </auto-generated>
// --------------------------------------------------------------------------
#pragma warning disable CS1591
#nullable enable
namespace DeriSock.Model
{
  
  /// <summary>
  /// <para>Time resolution given in full seconds or keyword <c>1D</c> (only some specific resolutions are supported)</para>
  /// </summary>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
  public partial class VolatilityIndexResolution : DeriSock.Model.EnumValue
  {
    public static VolatilityIndexResolution _1 = new VolatilityIndexResolution("1");
    public static VolatilityIndexResolution _60 = new VolatilityIndexResolution("60");
    public static VolatilityIndexResolution _3600 = new VolatilityIndexResolution("3600");
    public static VolatilityIndexResolution _43200 = new VolatilityIndexResolution("43200");
    public static VolatilityIndexResolution _1D = new VolatilityIndexResolution("1D");
    private VolatilityIndexResolution(string value) : 
        base(value)
    {
    }
  }
}
