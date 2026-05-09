using System.Runtime.InteropServices;
using rec_file_lib.DirectFileServer;

namespace rec_file_lib
{
    [ComVisible(true)]
    [Guid(ContractGuids.DirectFileServer_Interface)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDirectFileServer
    {
        string RecSel(string filePath, IRecSelOptions options);

        string RecInsType(string filePath, string recordType, string recordText);

        string RecDelType(string filePath, string recordType);
    }
}
