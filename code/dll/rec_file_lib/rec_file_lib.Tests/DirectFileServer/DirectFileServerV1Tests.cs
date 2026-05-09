using rec_file_lib.DirectFileServer;
using rec_file_lib.Tests.ExampleData;

namespace rec_file_lib.Tests.DirectFileServer;

public sealed class DirectFileServerV1Tests
{
    [Fact]
    public void RecSel_SimpleSingleFile_ReturnsPackedRecutilsStyleOutput()
    {
        var server = new DirectFileServerV1();
        var filePath = RecExampleData.GetExampleFilePath(RecExampleScenario.SimpleSingleFile);

        var output = server.RecSel(filePath);

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
        var server = new DirectFileServerV1();
        var filePath = RecExampleData.GetExampleFilePath(RecExampleScenario.SimpleRecutilsBookExample);

        var output = server.RecSel(filePath);

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

    private static string NormalizeForComparison(string text)
    {
        return text.Replace("\r\n", "\n", StringComparison.Ordinal).TrimEnd('\n');
    }
}
