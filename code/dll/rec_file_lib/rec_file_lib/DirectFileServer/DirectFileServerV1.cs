using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace rec_file_lib.DirectFileServer
{
    [ComVisible(true)]
    [Guid(ContractGuids.DirectFileServer_Implementation_V1)]
    public class DirectFileServerV1 : IDirectFileServer
    {
    }
}