using System;
using System.Runtime.InteropServices;

namespace rec_file_lib.DirectFileServer;

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_RecSelOptions)]
[ClassInterface(ClassInterfaceType.AutoDual)]
public class RecSelOptions
{
    public RecSelTypeOptions Type { get; set; } = new();

    public RecSelSelectOptions Select { get; set; } = new();

    public RecSelProjectOptions Project { get; set; } = new();

    public RecSelGroupOptions Group { get; set; } = new();

    public RecSelAggregateOptions Aggregate { get; set; } = new();
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_RecSelTypeOptions)]
[ClassInterface(ClassInterfaceType.AutoDual)]
public class RecSelTypeOptions
{
    public string RecordType { get; set; } = string.Empty;
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_RecSelSelectOptions)]
[ClassInterface(ClassInterfaceType.AutoDual)]
public class RecSelSelectOptions
{
    public string Indexes { get; set; } = string.Empty;

    public string Quick { get; set; } = string.Empty;

    public string Expression { get; set; } = string.Empty;

    public string JoinField { get; set; } = string.Empty;
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_RecSelProjectOptions)]
[ClassInterface(ClassInterfaceType.AutoDual)]
public class RecSelProjectOptions
{
    public string[] FieldNames { get; set; } = Array.Empty<string>();
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_RecSelGroupOptions)]
[ClassInterface(ClassInterfaceType.AutoDual)]
public class RecSelGroupOptions
{
    public string[] FieldNames { get; set; } = Array.Empty<string>();
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_RecSelAggregateOptions)]
[ClassInterface(ClassInterfaceType.AutoDual)]
public class RecSelAggregateOptions
{
    public bool Count { get; set; }

    public string CountFieldName { get; set; } = "Count";
}
