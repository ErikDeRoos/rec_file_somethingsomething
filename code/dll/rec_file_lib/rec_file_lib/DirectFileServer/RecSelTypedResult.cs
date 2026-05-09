using System;
using System.Runtime.InteropServices;
using rec_file_lib.Model;

namespace rec_file_lib.DirectFileServer;

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_IRecSelTypedResult)]
[InterfaceType(ComInterfaceType.InterfaceIsDual)]
public interface IRecSelTypedResult
{
    int StatusCode { get; set; }

    string Message { get; set; }

    IRecSelTypedRecordSet[] RecordSets { get; set; }
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_RecSelTypedResult)]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IRecSelTypedResult))]
public class RecSelTypedResult : IRecSelTypedResult
{
    public int StatusCode { get; set; }

    public string Message { get; set; } = string.Empty;

    public IRecSelTypedRecordSet[] RecordSets { get; set; } = Array.Empty<IRecSelTypedRecordSet>();

    internal static RecSelTypedResult Success(RecRecordSet? recordSet)
    {
        if (recordSet is null)
        {
            return new RecSelTypedResult
            {
                StatusCode = 1,
                Message = "No matching record set found.",
                RecordSets = Array.Empty<IRecSelTypedRecordSet>()
            };
        }

        var typedRecordSet = RecSelTypedRecordSet.FromModel(recordSet);
        return new RecSelTypedResult
        {
            StatusCode = 0,
            Message = "OK",
            RecordSets = [typedRecordSet]
        };
    }

    internal static RecSelTypedResult Error(int statusCode, string message)
    {
        return new RecSelTypedResult
        {
            StatusCode = statusCode,
            Message = message,
            RecordSets = Array.Empty<IRecSelTypedRecordSet>()
        };
    }
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_IRecSelTypedRecordSet)]
[InterfaceType(ComInterfaceType.InterfaceIsDual)]
public interface IRecSelTypedRecordSet
{
    string RecordType { get; set; }

    IRecSelTypedRecord[] Records { get; set; }
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_RecSelTypedRecordSet)]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IRecSelTypedRecordSet))]
public class RecSelTypedRecordSet : IRecSelTypedRecordSet
{
    public string RecordType { get; set; } = string.Empty;

    public IRecSelTypedRecord[] Records { get; set; } = Array.Empty<IRecSelTypedRecord>();

    internal static RecSelTypedRecordSet FromModel(RecRecordSet recordSet)
    {
        return new RecSelTypedRecordSet
        {
            RecordType = recordSet.TypeName ?? string.Empty,
            Records = recordSet.Records.Select(RecSelTypedRecord.FromModel).Cast<IRecSelTypedRecord>().ToArray()
        };
    }
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_IRecSelTypedRecord)]
[InterfaceType(ComInterfaceType.InterfaceIsDual)]
public interface IRecSelTypedRecord
{
    IRecSelTypedField[] Fields { get; set; }
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_RecSelTypedRecord)]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IRecSelTypedRecord))]
public class RecSelTypedRecord : IRecSelTypedRecord
{
    public IRecSelTypedField[] Fields { get; set; } = Array.Empty<IRecSelTypedField>();

    internal static RecSelTypedRecord FromModel(RecRecord record)
    {
        return new RecSelTypedRecord
        {
            Fields = record.Fields.Select(RecSelTypedField.FromModel).Cast<IRecSelTypedField>().ToArray()
        };
    }
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_IRecSelTypedField)]
[InterfaceType(ComInterfaceType.InterfaceIsDual)]
public interface IRecSelTypedField
{
    string Name { get; set; }

    string Value { get; set; }
}

[ComVisible(true)]
[Guid(ContractGuids.DirectFileServer_RecSelTypedField)]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IRecSelTypedField))]
public class RecSelTypedField : IRecSelTypedField
{
    public string Name { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    internal static RecSelTypedField FromModel(RecField field)
    {
        return new RecSelTypedField
        {
            Name = field.Name,
            Value = field.Value
        };
    }
}
