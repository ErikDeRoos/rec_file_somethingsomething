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
        private readonly DirectRecMutationParser _mutationParser = new();

        public string RecSel(string filePath)
        {
            _documentStore.LoadFromFile(filePath);
            return _recSelFormatter.FormatSelection(_documentStore.GetDocument());
        }

        public string RecSelType(string filePath, string recordType)
        {
            ArgumentNullException.ThrowIfNull(recordType);

            _documentStore.LoadFromFile(filePath);
            return _recSelFormatter.FormatRecordSet(_documentStore.FindRecordSet(recordType));
        }

        public string RecInsType(string filePath, string recordType, string recordText)
        {
            ArgumentNullException.ThrowIfNull(recordType);

            _documentStore.LoadFromFile(filePath);
            var record = _mutationParser.ParseRecord(recordText);
            var updatedRecordSet = _documentStore.InsertRecord(recordType, record);
            _documentStore.SaveToFile(filePath);
            return _recSelFormatter.FormatRecordSet(updatedRecordSet);
        }
    }
}