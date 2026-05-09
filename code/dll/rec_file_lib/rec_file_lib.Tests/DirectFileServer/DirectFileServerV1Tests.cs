using rec_file_lib.DirectFileServer;
using rec_file_lib.Tests.ExampleData;

namespace rec_file_lib.Tests.DirectFileServer;

public sealed class DirectFileServerV1Tests
{
    [Fact]
    public void RecSel_SimpleSingleFile_ReturnsPackedRecutilsStyleOutput()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.SimpleSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(workingCopy.FilePath);

        Assert.Equal(
            NormalizeForComparison(
                """
                Name: Erik
                Notes:
                + Thinks 'Inventing a new wheel is the best thing to do ever'.
                + 
                + Some x% of the people agree with that...
                + But does it matter? No percentage how little of how big, can be used to (by percentage alone) decide on its usefullness.
                
                Name: Buddy
                Notes:
                + Loyal dog.
                + Also goes by the name of 'Cuty-pie'.
                """),
            NormalizeForComparison(output));
    }

    [Fact]
    public void RecSel_SimpleRecutilsBookExample_ReturnsPackedRecordOutputWithoutCommentsOrDescriptor()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.SimpleRecutilsBookExample);
        var server = new DirectFileServerV1();

        var output = server.RecSel(workingCopy.FilePath);

        Assert.Equal(
            NormalizeForComparison(
                """
                Title: GNU Emacs Manual
                Author: Richard M. Stallman
                Publisher: FSF
                Location: home
                
                Title: The Colour of Magic
                Author: Terry Pratchett
                Location: loaned
                
                Title: Mio Cid
                Author: Anonymous
                Location: home
                
                Title: chapters.gnu.org administration guide
                Author: Nacho Gonzalez
                Author: Jose E. Marchesi
                Location: unknown
                
                Title: Yeelong User Manual
                Location: home
                """),
            NormalizeForComparison(output));
    }

    [Fact]
    public void RecSel_MultipleRecordTypesSingleFile_ThrowsBecauseTypeMustBeSpecified()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var exception = Assert.Throws<InvalidOperationException>(() => server.RecSel(workingCopy.FilePath));

        Assert.Equal("several record types found. Please use -t to specify one.", exception.Message);
    }

    [Fact]
    public void RecSelType_SimpleRecutilsBookExample_WithBookType_ReturnsRecordOutput()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.SimpleRecutilsBookExample);
        var server = new DirectFileServerV1();

        var output = server.RecSelType(workingCopy.FilePath, "Book");

        Assert.Equal(
            NormalizeForComparison(
                """
                Title: GNU Emacs Manual
                Author: Richard M. Stallman
                Publisher: FSF
                Location: home
                
                Title: The Colour of Magic
                Author: Terry Pratchett
                Location: loaned
                
                Title: Mio Cid
                Author: Anonymous
                Location: home
                
                Title: chapters.gnu.org administration guide
                Author: Nacho Gonzalez
                Author: Jose E. Marchesi
                Location: unknown
                
                Title: Yeelong User Manual
                Location: home
                """),
            NormalizeForComparison(output));
    }

    [Fact]
    public void RecSelType_SimpleRecutilsBookExample_WithUnknownType_ReturnsEmptyOutput()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.SimpleRecutilsBookExample);
        var server = new DirectFileServerV1();

        var output = server.RecSelType(workingCopy.FilePath, "Unknown");

        Assert.Equal(string.Empty, output);
    }

    [Fact]
    public void RecSelType_MultipleRecordTypesSingleFile_WithPersonType_ReturnsOnlyPersonRecords()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSelType(workingCopy.FilePath, "Person");

        Assert.Equal(
            NormalizeForComparison(
                """
                Name: Alfred Nebel
                Email: alf@example.com
                Abode: 42AbbeterWay
                
                Name: Mandy Nebel
                Email: mandy@example.com
                Abode: 42AbbeterWay
                
                Name: Ernest Wright
                Abode: ChezGrampa
                """),
            NormalizeForComparison(output));
    }

    [Fact]
    public void RecSelType_MultipleRecordTypesSingleFile_WithResidenceType_ReturnsOnlyResidenceRecords()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSelType(workingCopy.FilePath, "Residence");

        Assert.Equal(
            NormalizeForComparison(
                """
                Id: 42AbbeterWay
                Address: 42 Abbeter Way, Inprooving, WORCS
                Telephone: 01234 5676789
                
                Id: ChezGrampa
                Address: 1 Wanter Rise, Greater Inncombe, BUCKS
                """),
            NormalizeForComparison(output));
    }

    [Fact]
    public void RecInsType_SimpleRecutilsBookExample_AppendsBookRecordAndPersistsIt()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.SimpleRecutilsBookExample);
        var server = new DirectFileServerV1();

        var output = server.RecInsType(
            workingCopy.FilePath,
            "Book",
            """
            Title: Smalltalk Best Practice Patterns
            Author: Kent Beck
            Location: home
            """);

        Assert.Contains("Title: Smalltalk Best Practice Patterns", output, StringComparison.Ordinal);
        Assert.Contains("Author: Kent Beck", output, StringComparison.Ordinal);
        Assert.Contains("Location: home", output, StringComparison.Ordinal);

        var selected = server.RecSelType(workingCopy.FilePath, "Book");
        Assert.Contains("Title: Smalltalk Best Practice Patterns", selected, StringComparison.Ordinal);
    }

    [Fact]
    public void RecInsType_SimpleRecutilsBookExample_PersistsInsertionAcrossServerInstances()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.SimpleRecutilsBookExample);
        var insertingServer = new DirectFileServerV1();

        insertingServer.RecInsType(
            workingCopy.FilePath,
            "Book",
            """
            Title: Domain-Driven Design
            Author: Eric Evans
            Location: home
            """);

        var reloadedServer = new DirectFileServerV1();
        var output = reloadedServer.RecSelType(workingCopy.FilePath, "Book");

        Assert.Contains("Title: Domain-Driven Design", output, StringComparison.Ordinal);
        Assert.Contains("Author: Eric Evans", output, StringComparison.Ordinal);
    }

    [Fact]
    public void RecInsType_MultipleRecordTypesSingleFile_AppendsOnlyTheRequestedRecordType()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecInsType(
            workingCopy.FilePath,
            "Residence",
            """
            Id: NewPlace
            Address: 99 New Street, Test City
            Telephone: 0000 111222
            """);

        Assert.Contains("Id: NewPlace", output, StringComparison.Ordinal);
        Assert.Contains("Address: 99 New Street, Test City", output, StringComparison.Ordinal);

        var residences = server.RecSelType(workingCopy.FilePath, "Residence");
        var people = server.RecSelType(workingCopy.FilePath, "Person");

        Assert.Contains("Id: NewPlace", residences, StringComparison.Ordinal);
        Assert.DoesNotContain("Id: NewPlace", people, StringComparison.Ordinal);
    }

    [Fact]
    public void RecDelType_SimpleRecutilsBookExample_RemovesAllBookRecordsButKeepsDescriptor()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.SimpleRecutilsBookExample);
        var server = new DirectFileServerV1();

        var output = server.RecDelType(workingCopy.FilePath, "Book");

        Assert.Equal(string.Empty, output);
        Assert.Equal(string.Empty, server.RecSelType(workingCopy.FilePath, "Book"));
        var fileContents = File.ReadAllText(workingCopy.FilePath);
        Assert.Contains("%rec: Book", fileContents, StringComparison.Ordinal);
    }

    [Fact]
    public void RecDelType_SimpleRecutilsBookExample_PersistsDeletionAcrossServerInstances()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.SimpleRecutilsBookExample);
        var deletingServer = new DirectFileServerV1();

        deletingServer.RecDelType(workingCopy.FilePath, "Book");

        var reloadedServer = new DirectFileServerV1();
        var output = reloadedServer.RecSelType(workingCopy.FilePath, "Book");

        Assert.Equal(string.Empty, output);
        var fileContents = File.ReadAllText(workingCopy.FilePath);
        Assert.Contains("%rec: Book", fileContents, StringComparison.Ordinal);
    }

    [Fact]
    public void RecDelType_MultipleRecordTypesSingleFile_WithPerson_RemovesOnlyPersonRecords()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecDelType(workingCopy.FilePath, "Person");

        Assert.Equal(string.Empty, output);
        Assert.Equal(string.Empty, server.RecSelType(workingCopy.FilePath, "Person"));
        var residences = server.RecSelType(workingCopy.FilePath, "Residence");
        Assert.Contains("Id: 42AbbeterWay", residences, StringComparison.Ordinal);
    }

    [Fact]
    public void RecDelType_MultipleRecordTypesSingleFile_WithResidence_ThrowsValidationErrorBecausePersonsReferenceIt()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var exception = Assert.Throws<InvalidOperationException>(() => server.RecDelType(workingCopy.FilePath, "Residence"));

        Assert.Contains("references unknown key '42AbbeterWay'", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void RecDelType_WrongMissingMandatoryField_RemovesOffendingRecordsAndCanRecoverTheFile()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.WrongMissingMandatoryField);
        var server = new DirectFileServerV1();

        var output = server.RecDelType(workingCopy.FilePath, "User");

        Assert.Equal(string.Empty, output);
        Assert.Equal(string.Empty, server.RecSelType(workingCopy.FilePath, "User"));
    }

    [Fact]
    public void RecDelType_WrongDuplicateKeyValue_RemovesOffendingRecordsAndCanRecoverTheFile()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.WrongDuplicateKeyValue);
        var server = new DirectFileServerV1();

        var output = server.RecDelType(workingCopy.FilePath, "Item");

        Assert.Equal(string.Empty, output);
        Assert.Equal(string.Empty, server.RecSelType(workingCopy.FilePath, "Item"));
    }

    [Fact]
    public void RecDelType_WrongInvalidFieldType_RemovesOffendingRecordsAndCanRecoverTheFile()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.WrongInvalidFieldType);
        var server = new DirectFileServerV1();

        var output = server.RecDelType(workingCopy.FilePath, "Task");

        Assert.Equal(string.Empty, output);
        Assert.Equal(string.Empty, server.RecSelType(workingCopy.FilePath, "Task"));
    }

    [Fact]
    public void RecInsType_WrongMissingMandatoryField_ThrowsValidationError()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.WrongMissingMandatoryField);
        var server = new DirectFileServerV1();

        var exception = Assert.Throws<InvalidOperationException>(() => server.RecInsType(
            workingCopy.FilePath,
            "User",
            """
            Name: Added user
            Notes: Valid inserted record.
            """));

        Assert.Contains("missing mandatory field 'Name'", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void RecInsType_WrongDuplicateKeyValue_ThrowsValidationError()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.WrongDuplicateKeyValue);
        var server = new DirectFileServerV1();

        var exception = Assert.Throws<InvalidOperationException>(() => server.RecInsType(
            workingCopy.FilePath,
            "Item",
            """
            Id: 2
            Title: Added item
            """));

        Assert.Contains("duplicate key value '1'", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void RecInsType_WrongInvalidFieldType_ThrowsValidationError()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.WrongInvalidFieldType);
        var server = new DirectFileServerV1();

        var exception = Assert.Throws<InvalidOperationException>(() => server.RecInsType(
            workingCopy.FilePath,
            "Task",
            """
            Title: Added task
            Status: open
            """));

        Assert.Contains("invalid enum value 'pending'", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void RecInsType_WrongBadMultilineContinuation_ThrowsDuringInitialLoad()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.WrongBadMultilineContinuation);
        var server = new DirectFileServerV1();

        var exception = Assert.Throws<FormatException>(() => server.RecInsType(
            workingCopy.FilePath,
            "Note",
            """
            Title: Added note
            """));

        Assert.Equal("Continuation line found without a current field.", exception.Message);
    }

    [Fact]
    public void RecInsType_WrongMissingFieldSeparator_ThrowsDuringInitialLoad()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.WrongMissingFieldSeparator);
        var server = new DirectFileServerV1();

        var exception = Assert.Throws<FormatException>(() => server.RecInsType(
            workingCopy.FilePath,
            "Article",
            """
            Name: Added thing
            Id: 1
            """));

        Assert.Equal("Invalid field line: %key Id", exception.Message);
    }

    [Fact]
    public void RecDelType_WrongBadMultilineContinuation_ThrowsDuringInitialLoad()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.WrongBadMultilineContinuation);
        var server = new DirectFileServerV1();

        var exception = Assert.Throws<FormatException>(() => server.RecDelType(workingCopy.FilePath, "Note"));

        Assert.Equal("Continuation line found without a current field.", exception.Message);
    }

    [Fact]
    public void RecDelType_WrongMissingFieldSeparator_ThrowsDuringInitialLoad()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.WrongMissingFieldSeparator);
        var server = new DirectFileServerV1();

        var exception = Assert.Throws<FormatException>(() => server.RecDelType(workingCopy.FilePath, "Article"));

        Assert.Equal("Invalid field line: %key Id", exception.Message);
    }

    [Fact]
    public void RecSelTypeSelect_MultipleRecordTypesSingleFile_WithIndexes_ReturnsOnlyRequestedRecords()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSelTypeSelect(workingCopy.FilePath, "Person", "1-2", null);

        Assert.Equal(
            NormalizeForComparison(
                """
                Name: Mandy Nebel
                Email: mandy@example.com
                Abode: 42AbbeterWay

                Name: Ernest Wright
                Abode: ChezGrampa
                """),
            NormalizeForComparison(output));
    }

    [Fact]
    public void RecSelTypeSelect_MultipleRecordTypesSingleFile_WithProjection_ReturnsOnlyProjectedFields()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSelTypeSelect(workingCopy.FilePath, "Person", null, "Name,Email");

        Assert.Equal(
            NormalizeForComparison(
                """
                Name: Alfred Nebel
                Email: alf@example.com

                Name: Mandy Nebel
                Email: mandy@example.com

                Name: Ernest Wright
                """),
            NormalizeForComparison(output));
    }

    [Fact]
    public void RecSelTypeSelect_MultipleRecordTypesSingleFile_WithIndexesAndProjection_AppliesBoth()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSelTypeSelect(workingCopy.FilePath, "Person", "0,2", "Name");

        Assert.Equal(
            NormalizeForComparison(
                """
                Name: Alfred Nebel

                Name: Ernest Wright
                """),
            NormalizeForComparison(output));
    }

    [Fact]
    public void RecSelTypeSelect_WithInvalidIndexRange_ThrowsFormatException()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var exception = Assert.Throws<FormatException>(() =>
            server.RecSelTypeSelect(workingCopy.FilePath, "Person", "2-1", "Name"));

        Assert.Equal("Invalid record index range: '2-1'.", exception.Message);
    }

    private static string NormalizeForComparison(string text)
    {
        return text.Replace("\r\n", "\n", StringComparison.Ordinal).TrimEnd('\n');
    }
}
