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

    private static string NormalizeForComparison(string text)
    {
        return text.Replace("\r\n", "\n", StringComparison.Ordinal).TrimEnd('\n');
    }
}
