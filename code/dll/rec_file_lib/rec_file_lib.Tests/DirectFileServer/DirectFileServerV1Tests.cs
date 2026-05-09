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

        var output = server.RecSel(workingCopy.FilePath, options: null);

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

        var output = server.RecSel(workingCopy.FilePath, options: null);

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

        var exception = Assert.Throws<InvalidOperationException>(() => server.RecSel(workingCopy.FilePath, options: null));

        Assert.Equal("several record types found. Please use -t to specify one.", exception.Message);
    }

    [Fact]
    public void RecSel_WithType_SimpleRecutilsBookExample_WithBookType_ReturnsRecordOutput()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.SimpleRecutilsBookExample);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Book" }
            });

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
    public void RecSel_WithType_SimpleRecutilsBookExample_WithUnknownType_ReturnsEmptyOutput()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.SimpleRecutilsBookExample);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Unknown" }
            });

        Assert.Equal(string.Empty, output);
    }

    [Fact]
    public void RecSel_WithType_MultipleRecordTypesSingleFile_WithPersonType_ReturnsOnlyPersonRecords()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" }
            });

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
    public void RecSel_WithType_MultipleRecordTypesSingleFile_WithResidenceType_ReturnsOnlyResidenceRecords()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Residence" }
            });

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

        var selected = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Book" }
            });
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
        var output = reloadedServer.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Book" }
            });

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

        var residences = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Residence" }
            });
        var people = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" }
            });

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
        Assert.Equal(
            string.Empty,
            server.RecSel(
                workingCopy.FilePath,
                new RecSelOptions
                {
                    Type = new RecSelTypeOptions { RecordType = "Book" }
                }));
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
        var output = reloadedServer.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Book" }
            });

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
        Assert.Equal(
            string.Empty,
            server.RecSel(
                workingCopy.FilePath,
                new RecSelOptions
                {
                    Type = new RecSelTypeOptions { RecordType = "Person" }
                }));
        var residences = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Residence" }
            });
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
        Assert.Equal(
            string.Empty,
            server.RecSel(
                workingCopy.FilePath,
                new RecSelOptions
                {
                    Type = new RecSelTypeOptions { RecordType = "User" }
                }));
    }

    [Fact]
    public void RecDelType_WrongDuplicateKeyValue_RemovesOffendingRecordsAndCanRecoverTheFile()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.WrongDuplicateKeyValue);
        var server = new DirectFileServerV1();

        var output = server.RecDelType(workingCopy.FilePath, "Item");

        Assert.Equal(string.Empty, output);
        Assert.Equal(
            string.Empty,
            server.RecSel(
                workingCopy.FilePath,
                new RecSelOptions
                {
                    Type = new RecSelTypeOptions { RecordType = "Item" }
                }));
    }

    [Fact]
    public void RecDelType_WrongInvalidFieldType_RemovesOffendingRecordsAndCanRecoverTheFile()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.WrongInvalidFieldType);
        var server = new DirectFileServerV1();

        var output = server.RecDelType(workingCopy.FilePath, "Task");

        Assert.Equal(string.Empty, output);
        Assert.Equal(
            string.Empty,
            server.RecSel(
                workingCopy.FilePath,
                new RecSelOptions
                {
                    Type = new RecSelTypeOptions { RecordType = "Task" }
                }));
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
    public void RecSel_WithTypeAndSelect_MultipleRecordTypesSingleFile_WithIndexes_ReturnsOnlyRequestedRecords()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { Indexes = "1-2" }
            });

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
    public void RecSel_WithTypeAndProject_MultipleRecordTypesSingleFile_WithProjection_ReturnsOnlyProjectedFields()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Project = new RecSelProjectOptions { FieldNames = ["Name", "Email"] }
            });

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
    public void RecSel_WithTypeSelectAndProject_MultipleRecordTypesSingleFile_AppliesBoth()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { Indexes = "0,2" },
                Project = new RecSelProjectOptions { FieldNames = ["Name"] }
            });

        Assert.Equal(
            NormalizeForComparison(
                """
                Name: Alfred Nebel

                Name: Ernest Wright
                """),
            NormalizeForComparison(output));
    }

    [Fact]
    public void RecSel_WithInvalidIndexRange_ThrowsFormatException()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var exception = Assert.Throws<FormatException>(() =>
            server.RecSel(
                workingCopy.FilePath,
                new RecSelOptions
                {
                    Type = new RecSelTypeOptions { RecordType = "Person" },
                    Select = new RecSelSelectOptions { Indexes = "2-1" },
                    Project = new RecSelProjectOptions { FieldNames = ["Name"] }
                }));

        Assert.Equal("Invalid record index range: '2-1'.", exception.Message);
    }

    [Fact]
    public void RecSel_WithTypeAndQuick_MultipleRecordTypesSingleFile_ReturnsOnlyQuickMatchedRecords()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { Quick = "Chez" }
            });

        Assert.Equal(
            NormalizeForComparison(
                """
                Name: Ernest Wright
                Abode: ChezGrampa
                """),
            NormalizeForComparison(output));
    }

    [Fact]
    public void RecSel_WithTypeQuickAndProject_MultipleRecordTypesSingleFile_AppliesQuickThenProjection()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { Quick = "example.com" },
                Project = new RecSelProjectOptions { FieldNames = ["Name"] }
            });

        Assert.Equal(
            NormalizeForComparison(
                """
                Name: Alfred Nebel

                Name: Mandy Nebel
                """),
            NormalizeForComparison(output));
    }

    [Fact]
    public void RecSel_WithTypeQuickAndIndexes_MultipleRecordTypesSingleFile_AppliesBothFilters()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { Indexes = "1-2", Quick = "Chez" }
            });

        Assert.Equal(
            NormalizeForComparison(
                """
                Name: Ernest Wright
                Abode: ChezGrampa
                """),
            NormalizeForComparison(output));
    }

    [Fact]
    public void RecSel_WithTypeAndQuick_NoMatches_ReturnsEmptyOutput()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { Quick = "does-not-exist" }
            });

        Assert.Equal(string.Empty, output);
    }

    [Fact]
    public void RecSel_WithTypeAndExpressionEquals_ReturnsOnlyMatchingRecords()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { Expression = "Name = \"Mandy Nebel\"" }
            });

        Assert.Equal(
            NormalizeForComparison(
                """
                Name: Mandy Nebel
                Email: mandy@example.com
                Abode: 42AbbeterWay
                """),
            NormalizeForComparison(output));
    }

    [Fact]
    public void RecSel_WithTypeAndExpressionContains_ReturnsOnlyMatchingRecords()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { Expression = "Email ~ \"example.com\"" }
            });

        Assert.Equal(
            NormalizeForComparison(
                """
                Name: Alfred Nebel
                Email: alf@example.com
                Abode: 42AbbeterWay

                Name: Mandy Nebel
                Email: mandy@example.com
                Abode: 42AbbeterWay
                """),
            NormalizeForComparison(output));
    }

    [Fact]
    public void RecSel_WithTypeAndExpressionNotEquals_ReturnsOnlyNonMatchingRecords()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { Expression = "Name != \"Mandy Nebel\"" }
            });

        Assert.Equal(
            NormalizeForComparison(
                """
                Name: Alfred Nebel
                Email: alf@example.com
                Abode: 42AbbeterWay

                Name: Ernest Wright
                Abode: ChezGrampa
                """),
            NormalizeForComparison(output));
    }

    [Fact]
    public void RecSel_WithTypeAndInvalidExpression_ThrowsFormatException()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var exception = Assert.Throws<FormatException>(() =>
            server.RecSel(
                workingCopy.FilePath,
                new RecSelOptions
                {
                    Type = new RecSelTypeOptions { RecordType = "Person" },
                    Select = new RecSelSelectOptions { Expression = "Name ~~ Mandy" }
                }));

        Assert.Equal("Invalid selection expression: 'Name ~~ Mandy'.", exception.Message);
    }

    [Fact]
    public void RecSel_WithTypeAndJoinField_PerformsInnerJoinAndIncludesJoinedFields()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { JoinField = "Abode" }
            });

        Assert.Equal(
            NormalizeForComparison(
                """
                Name: Alfred Nebel
                Email: alf@example.com
                Abode: 42AbbeterWay
                Residence.Id: 42AbbeterWay
                Residence.Address: 42 Abbeter Way, Inprooving, WORCS
                Residence.Telephone: 01234 5676789

                Name: Mandy Nebel
                Email: mandy@example.com
                Abode: 42AbbeterWay
                Residence.Id: 42AbbeterWay
                Residence.Address: 42 Abbeter Way, Inprooving, WORCS
                Residence.Telephone: 01234 5676789

                Name: Ernest Wright
                Abode: ChezGrampa
                Residence.Id: ChezGrampa
                Residence.Address: 1 Wanter Rise, Greater Inncombe, BUCKS
                """),
            NormalizeForComparison(output));
    }

    [Fact]
    public void RecSel_WithTypeJoinFieldAndProjection_CanProjectJoinedFields()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { JoinField = "Abode" },
                Project = new RecSelProjectOptions { FieldNames = ["Name", "Residence.Address"] }
            });

        Assert.Equal(
            NormalizeForComparison(
                """
                Name: Alfred Nebel
                Residence.Address: 42 Abbeter Way, Inprooving, WORCS

                Name: Mandy Nebel
                Residence.Address: 42 Abbeter Way, Inprooving, WORCS

                Name: Ernest Wright
                Residence.Address: 1 Wanter Rise, Greater Inncombe, BUCKS
                """),
            NormalizeForComparison(output));
    }

    [Fact]
    public void RecSel_WithJoinFieldNotDeclaredAsRecType_ThrowsInvalidOperationException()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var exception = Assert.Throws<InvalidOperationException>(() =>
            server.RecSel(
                workingCopy.FilePath,
                new RecSelOptions
                {
                    Type = new RecSelTypeOptions { RecordType = "Person" },
                    Select = new RecSelSelectOptions { JoinField = "Name" }
                }));

        Assert.Equal("join field 'Name' is not declared with a rec type.", exception.Message);
    }

    [Fact]
    public void RecSel_WithTypeGroupByAndCount_ReturnsGroupedCounts()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Group = new RecSelGroupOptions { FieldNames = ["Abode"] },
                Aggregate = new RecSelAggregateOptions { Count = true, CountFieldName = "Total" }
            });

        Assert.Equal(
            NormalizeForComparison(
                """
                Abode: 42AbbeterWay
                Total: 2

                Abode: ChezGrampa
                Total: 1
                """),
            NormalizeForComparison(output));
    }

    [Fact]
    public void RecSel_WithTypeAndCountOnly_ReturnsSingleCountRecord()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Aggregate = new RecSelAggregateOptions { Count = true, CountFieldName = "Count" }
            });

        Assert.Equal(
            NormalizeForComparison(
                """
                Count: 3
                """),
            NormalizeForComparison(output));
    }

    [Fact]
    public void RecSel_WithJoinGroupByAndCount_CanGroupOnJoinedFields()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { JoinField = "Abode" },
                Group = new RecSelGroupOptions { FieldNames = ["Residence.Address"] },
                Aggregate = new RecSelAggregateOptions { Count = true, CountFieldName = "Total" }
            });

        Assert.Equal(
            NormalizeForComparison(
                """
                Residence.Address: 42 Abbeter Way, Inprooving, WORCS
                Total: 2

                Residence.Address: 1 Wanter Rise, Greater Inncombe, BUCKS
                Total: 1
                """),
            NormalizeForComparison(output));
    }

    [Fact]
    public void RecSel_WithTypeAndSortBySingleField_SortsOutputAscending()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Sort = new RecSelSortOptions { FieldNames = ["Name"] }
            });

        Assert.Equal(
            NormalizeForComparison(
                """
                Name: Alfred Nebel
                Email: alf@example.com
                Abode: 42AbbeterWay

                Name: Ernest Wright
                Abode: ChezGrampa

                Name: Mandy Nebel
                Email: mandy@example.com
                Abode: 42AbbeterWay
                """),
            NormalizeForComparison(output));
    }

    [Fact]
    public void RecSel_WithTypeGroupByCountAndSort_SortsGroupedOutput()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Group = new RecSelGroupOptions { FieldNames = ["Abode"] },
                Aggregate = new RecSelAggregateOptions { Count = true, CountFieldName = "Total" },
                Sort = new RecSelSortOptions { FieldNames = ["Abode"] }
            });

        Assert.Equal(
            NormalizeForComparison(
                """
                Abode: 42AbbeterWay
                Total: 2

                Abode: ChezGrampa
                Total: 1
                """),
            NormalizeForComparison(output));
    }

    [Fact]
    public void RecSel_SimpleRecutilsBookExample_WithCollapse_EmitsRecordsWithoutBlankSeparators()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.SimpleRecutilsBookExample);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Book" },
                Select = new RecSelSelectOptions { Collapse = true }
            });

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
    public void RecSel_WithTypeAndSortBySingleField_WithCollapse_SortsAndCollapsesOutput()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { Collapse = true },
                Sort = new RecSelSortOptions { FieldNames = ["Name"] }
            });

        Assert.Equal(
            NormalizeForComparison(
                """
                Name: Alfred Nebel
                Email: alf@example.com
                Abode: 42AbbeterWay
                Name: Ernest Wright
                Abode: ChezGrampa
                Name: Mandy Nebel
                Email: mandy@example.com
                Abode: 42AbbeterWay
                """),
            NormalizeForComparison(output));
    }

    [Fact]
    public void RecSel_WithTypeAndUniq_DoesNotRemoveNonDuplicateRepeatedFields()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.SimpleRecutilsBookExample);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Book" },
                Select = new RecSelSelectOptions { Uniq = true }
            });

        Assert.Contains("Author: Nacho Gonzalez", output, StringComparison.Ordinal);
        Assert.Contains("Author: Jose E. Marchesi", output, StringComparison.Ordinal);
    }

    [Fact]
    public void RecSel_WithJoinProjectionAndUniq_RemovesDuplicateJoinedFieldsPerRecord()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { JoinField = "Abode", Uniq = true },
                Project = new RecSelProjectOptions { FieldNames = ["Residence.Address", "Residence.Address", "Name"] }
            });

        Assert.Equal(
            NormalizeForComparison(
                """
                Name: Alfred Nebel
                Residence.Address: 42 Abbeter Way, Inprooving, WORCS

                Name: Mandy Nebel
                Residence.Address: 42 Abbeter Way, Inprooving, WORCS

                Name: Ernest Wright
                Residence.Address: 1 Wanter Rise, Greater Inncombe, BUCKS
                """),
            NormalizeForComparison(output));
    }

    [Fact]
    public void RecSel_WithTypeIndexesSortAndProjection_AppliesSelectionThenSortThenProjection()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { Indexes = "0-1" },
                Sort = new RecSelSortOptions { FieldNames = ["Email"] },
                Project = new RecSelProjectOptions { FieldNames = ["Name"] }
            });

        Assert.Equal(
            NormalizeForComparison(
                """
                Name: Alfred Nebel

                Name: Mandy Nebel
                """),
            NormalizeForComparison(output));
    }

    [Fact]
    public void RecSel_WithTypeGroupByCountSortAndProjection_AppliesGroupingThenSortingThenProjection()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Group = new RecSelGroupOptions { FieldNames = ["Abode"] },
                Aggregate = new RecSelAggregateOptions { Count = true, CountFieldName = "Total" },
                Sort = new RecSelSortOptions { FieldNames = ["Abode"] },
                Project = new RecSelProjectOptions { FieldNames = ["Total"] }
            });

        Assert.Equal(
            NormalizeForComparison(
                """
                Total: 2

                Total: 1
                """),
            NormalizeForComparison(output));
    }

    [Fact]
    public void RecSel_WithTypeAndIncludeDescriptors_IncludesDescriptorBeforeRecords()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { IncludeDescriptors = true }
            });

        Assert.Equal(
            NormalizeForComparison(
                """
                %rec: Person
                %type: Abode rec Residence

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
    public void RecSel_WithTypeIncludeDescriptorsAndCollapse_CombinesBothBehaviors()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { IncludeDescriptors = true, Collapse = true }
            });

        Assert.Equal(
            NormalizeForComparison(
                """
                %rec: Person
                %type: Abode rec Residence

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
    public void RecSel_WithTypeIncludeDescriptorsAndNoMatchedRecords_StillIncludesDescriptor()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var output = server.RecSel(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { IncludeDescriptors = true, Quick = "__no_match__" }
            });

        Assert.Equal(
            NormalizeForComparison(
                """
                %rec: Person
                %type: Abode rec Residence
                """),
            NormalizeForComparison(output));
    }

    private static string NormalizeForComparison(string text)
    {
        return text.Replace("\r\n", "\n", StringComparison.Ordinal).TrimEnd('\n');
    }
}
