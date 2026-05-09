using System.Runtime.InteropServices;

namespace rec_file_lib
{
    [ComVisible(true)]
    [Guid(ContractGuids.DirectFileServer_Interface)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDirectFileServer
    {
        string RecSel(string filePath);

        string RecSelType(string filePath, string recordType);

        string RecSelTypeSelect(string filePath, string recordType, string indexes, string fields);

        string RecInsType(string filePath, string recordType, string recordText);

        string RecDelType(string filePath, string recordType);
    }
}
