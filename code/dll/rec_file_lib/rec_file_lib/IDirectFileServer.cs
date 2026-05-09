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

        IRecSelTypedResult RecSel_Typed(string filePath, IRecSelOptions options);

        string RecIns(string filePath, IRecInsOptions options);

        IRecSelTypedResult RecIns_Typed(string filePath, IRecInsOptions options);

        string RecDel(string filePath, IRecDelOptions options);

        IRecSelTypedResult RecDel_Typed(string filePath, IRecDelOptions options);
    }
}
