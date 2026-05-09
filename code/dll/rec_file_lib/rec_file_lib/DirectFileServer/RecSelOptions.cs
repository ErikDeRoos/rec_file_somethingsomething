using System;
using System.Runtime.InteropServices;

namespace rec_file_lib.DirectFileServer;

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_IRecSelOptions)]
[InterfaceType(ComInterfaceType.InterfaceIsDual)]
public interface IRecSelOptions
{
    IRecSelTypeOptions Type { get; set; }

    IRecSelSelectOptions Select { get; set; }

    IRecSelProjectOptions Project { get; set; }

    IRecSelGroupOptions Group { get; set; }

    IRecSelAggregateOptions Aggregate { get; set; }

    IRecSelSortOptions Sort { get; set; }
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_RecSelOptions)]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IRecSelOptions))]
public class RecSelOptions : IRecSelOptions
{
    public IRecSelTypeOptions Type { get; set; } = new RecSelTypeOptions();

    public IRecSelSelectOptions Select { get; set; } = new RecSelSelectOptions();

    public IRecSelProjectOptions Project { get; set; } = new RecSelProjectOptions();

    public IRecSelGroupOptions Group { get; set; } = new RecSelGroupOptions();

    public IRecSelAggregateOptions Aggregate { get; set; } = new RecSelAggregateOptions();

    public IRecSelSortOptions Sort { get; set; } = new RecSelSortOptions();
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_IRecSelTypeOptions)]
[InterfaceType(ComInterfaceType.InterfaceIsDual)]
public interface IRecSelTypeOptions
{
    string RecordType { get; set; }
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_RecSelTypeOptions)]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IRecSelTypeOptions))]
public class RecSelTypeOptions : IRecSelTypeOptions
{
    public string RecordType { get; set; } = string.Empty;
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_IRecSelSelectOptions)]
[InterfaceType(ComInterfaceType.InterfaceIsDual)]
public interface IRecSelSelectOptions
{
    string Indexes { get; set; }

    string Quick { get; set; }

    string Expression { get; set; }

    string JoinField { get; set; }

    bool Collapse { get; set; }

    bool Uniq { get; set; }

    bool IncludeDescriptors { get; set; }
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_RecSelSelectOptions)]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IRecSelSelectOptions))]
public class RecSelSelectOptions : IRecSelSelectOptions
{
    public string Indexes { get; set; } = string.Empty;

    public string Quick { get; set; } = string.Empty;

    public string Expression { get; set; } = string.Empty;

    public string JoinField { get; set; } = string.Empty;

    public bool Collapse { get; set; }

    public bool Uniq { get; set; }

    public bool IncludeDescriptors { get; set; }
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_IRecSelProjectOptions)]
[InterfaceType(ComInterfaceType.InterfaceIsDual)]
public interface IRecSelProjectOptions
{
    string[] FieldNames { get; set; }
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_RecSelProjectOptions)]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IRecSelProjectOptions))]
public class RecSelProjectOptions : IRecSelProjectOptions
{
    public string[] FieldNames { get; set; } = Array.Empty<string>();
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_IRecSelGroupOptions)]
[InterfaceType(ComInterfaceType.InterfaceIsDual)]
public interface IRecSelGroupOptions
{
    string[] FieldNames { get; set; }
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_RecSelGroupOptions)]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IRecSelGroupOptions))]
public class RecSelGroupOptions : IRecSelGroupOptions
{
    public string[] FieldNames { get; set; } = Array.Empty<string>();
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_IRecSelAggregateOptions)]
[InterfaceType(ComInterfaceType.InterfaceIsDual)]
public interface IRecSelAggregateOptions
{
    bool Count { get; set; }

    string CountFieldName { get; set; }
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_RecSelAggregateOptions)]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IRecSelAggregateOptions))]
public class RecSelAggregateOptions : IRecSelAggregateOptions
{
    public bool Count { get; set; }

    public string CountFieldName { get; set; } = "Count";
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_IRecSelSortOptions)]
[InterfaceType(ComInterfaceType.InterfaceIsDual)]
public interface IRecSelSortOptions
{
    string[] FieldNames { get; set; }
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_RecSelSortOptions)]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IRecSelSortOptions))]
public class RecSelSortOptions : IRecSelSortOptions
{
    public string[] FieldNames { get; set; } = Array.Empty<string>();
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_IRecInsOptions)]
[InterfaceType(ComInterfaceType.InterfaceIsDual)]
public interface IRecInsOptions
{
    string RecordType { get; set; }

    string RecordText { get; set; }
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_RecInsOptions)]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IRecInsOptions))]
public class RecInsOptions : IRecInsOptions
{
    public string RecordType { get; set; } = string.Empty;

    public string RecordText { get; set; } = string.Empty;
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_IRecDelOptions)]
[InterfaceType(ComInterfaceType.InterfaceIsDual)]
public interface IRecDelOptions
{
    string RecordType { get; set; }
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_RecDelOptions)]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IRecDelOptions))]
public class RecDelOptions : IRecDelOptions
{
    public string RecordType { get; set; } = string.Empty;
}
