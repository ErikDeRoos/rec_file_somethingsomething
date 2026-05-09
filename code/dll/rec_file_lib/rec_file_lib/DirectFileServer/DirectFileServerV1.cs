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
        private readonly DirectFileServerDocumentStore _documentStore = new();
        private readonly DirectRecSelFormatter _recSelFormatter = new();

        public string RecSel(string filePath)
        {
            _documentStore.LoadFromFile(filePath);
            return _recSelFormatter.FormatSelection(_documentStore.GetDocument());
        }
    }
}