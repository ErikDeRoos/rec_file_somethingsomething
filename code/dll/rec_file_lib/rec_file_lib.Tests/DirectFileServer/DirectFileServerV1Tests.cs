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

    private static string NormalizeForComparison(string text)
    {
        return text.Replace("\r\n", "\n", StringComparison.Ordinal).TrimEnd('\n');
    }
}
