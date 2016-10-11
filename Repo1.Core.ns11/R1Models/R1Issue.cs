using Repo1.Core.ns11.Drupal7Tools;

namespace Repo1.Core.ns11.R1Models
{
    [D7Type(Key = "issue")]
    public class R1Issue : R1MachineSpecsBase
    {
        [NodeTitle]                            public string           Title         { get; set; }
        [Value(Key = "field_description"   )]  public string           Description   { get; set; }
        [Term (Key = "field_issue_category")]  public IssueCategories  Category      { get; set; }
        [Term (Key = "field_issue_status"  )]  public IssueStates      Status        { get; set; }
        [Term (Key = "field_issue_priority")]  public IssuePriorities  Priority      { get; set; }
        [User (Key = "field_assigned_to"   )]  public object           AssignedTo    { get; set; }
        //[Node (Key = "field_executable"    )]  public R1Executable     Executable    { get; set; }
        [Value(Key = "field_uniquekey"     )]  public string           RecordHash    { get; set; }
    }
}
