using System.ComponentModel.DataAnnotations.Schema;
using LeSi.Admin.Domain.Attributes;

namespace LeSi.Admin.Domain.Entities.SystemManage;

[Table("tbl_dictionary")]
[Database("dictionary_db")]
public class DictionaryEntity : BaseEntity
{
    public string DictionaryCode { get; set; }

    public string DictionaryName { get; set; }
}