using Blazor.Core.Enums;

namespace Blazor.FamilyTreeJS.Components.Interop.Modules.FamilyTreeStatic;

/// <summary>
/// Available icons from FamilyTreeJS.
/// See https://balkan.app/FamilyTreeJS/API/classes/FamilyTree#icon.
/// </summary>
public sealed class Icon : StringEnum
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  public Icon(string value) : base(value) {}

  public static readonly Icon Png = new("png");

  public static readonly Icon Pdf = new("pdf");

  public static readonly Icon Svg = new("svg");

  public static readonly Icon Csv = new("csv");

  public static readonly Icon Excel = new("excel");

  public static readonly Icon Edit = new("edit");

  public static readonly Icon Details = new("details");

  public static readonly Icon Remove = new("remove");

  public static readonly Icon Add = new("add");

  public static readonly Icon Xml = new("xml");

  public static readonly Icon Link = new("link");

  public static readonly Icon Happy = new("happy");

  public static readonly Icon Sad = new("ad");

  public static readonly Icon Share = new("share");

  public static readonly Icon User = new("user");

  public static readonly Icon AddUser = new("addUser");

  public static readonly Icon Close = new("close");

  public static readonly Icon FamilyTree = new("ft");

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
