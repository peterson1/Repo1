using Repo1.Core.ns11.Drupal7Tools;

namespace Repo1.Core.ns11.R1Models
{
    [D7Type(Key = "issue")]
    public class R1Issue : D7NodeBase
    {
        [NodeTitle]                              public string           Title             { get; set; }
        [Value(Key = "field_description"     )]  public string           Description       { get; set; }
        [Term (Key = "field_issue_category"  )]  public IssueCategories  Category          { get; set; }
        [Term (Key = "field_issue_status"    )]  public IssueStates      Status            { get; set; }
        [Term (Key = "field_issue_priority"  )]  public IssuePriorities  Priority          { get; set; }
        [User (Key = "field_assigned_to"     )]  public object           AssignedTo        { get; set; }
        [Node (Key = "field_executable"      )]  public R1Executable     Executable        { get; set; }
        [Value(Key = "field_fileversion"     )]  public string           ExeVersion        { get; set; }
        [Value(Key = "field_publicip"        )]  public string           PublicIP          { get; set; }
        [Value(Key = "field_filepath"        )]  public string           ExePath           { get; set; }
        [Value(Key = "field_macandprivateips")]  public string           MacAndPrivateIPs  { get; set; }
        [Value(Key = "field_windowsaccount"  )]  public string           WindowsAccount    { get; set; }
        [Value(Key = "field_computername"    )]  public string           ComputerName      { get; set; }
        [Value(Key = "field_workgroup"       )]  public string           Workgroup         { get; set; }
        [Value(Key = "field_legacycfgjson"   )]  public string           LegacyCfgJson     { get; set; }
        [Value(Key = "field_repo1cfgjson"    )]  public string           Repo1CfgJson      { get; set; }
    }
}
