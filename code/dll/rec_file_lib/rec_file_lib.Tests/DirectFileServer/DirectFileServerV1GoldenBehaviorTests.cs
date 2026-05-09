using rec_file_lib.DirectFileServer;
using rec_file_lib.Tests.ExampleData;

namespace rec_file_lib.Tests.DirectFileServer;

public sealed class DirectFileServerV1GoldenBehaviorTests
{
    [Theory]
    [MemberData(nameof(GetGoldenCases))]
    public void RecSel_GoldenSubset_MatchesExpectedOutput(
        string caseName,
        int scenario,
        RecSelOptions options,
        string expectedOutput)
    {
        using var workingCopy = RecExampleWorkingCopy.Create((RecExampleScenario)scenario);
        var server = new DirectFileServerV1();

        var output = server.RecSel(workingCopy.FilePath, options);

        Assert.Equal(NormalizeForComparison(expectedOutput), NormalizeForComparison(output));
    }

    public static IEnumerable<object[]> GetGoldenCases()
    {
        yield return
        [
            "type_only_person",
            (int)RecExampleScenario.MultipleRecordTypesSingleFile,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" }
            },
            """
            Name: Alfred Nebel
            Email: alf@example.com
            Abode: 42AbbeterWay

            Name: Mandy Nebel
            Email: mandy@example.com
            Abode: 42AbbeterWay

            Name: Ernest Wright
            Abode: ChezGrampa
            """
        ];

        yield return
        [
            "type_indexes",
            (int)RecExampleScenario.MultipleRecordTypesSingleFile,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { Indexes = "1-2" }
            },
            """
            Name: Mandy Nebel
            Email: mandy@example.com
            Abode: 42AbbeterWay

            Name: Ernest Wright
            Abode: ChezGrampa
            """
        ];

        yield return
        [
            "type_quick",
            (int)RecExampleScenario.MultipleRecordTypesSingleFile,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { Quick = "Chez" }
            },
            """
            Name: Ernest Wright
            Abode: ChezGrampa
            """
        ];

        yield return
        [
            "type_expression",
            (int)RecExampleScenario.MultipleRecordTypesSingleFile,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { Expression = "Email ~ \"example.com\"" }
            },
            """
            Name: Alfred Nebel
            Email: alf@example.com
            Abode: 42AbbeterWay

            Name: Mandy Nebel
            Email: mandy@example.com
            Abode: 42AbbeterWay
            """
        ];

        yield return
        [
            "type_join_projection",
            (int)RecExampleScenario.MultipleRecordTypesSingleFile,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { JoinField = "Abode" },
                Project = new RecSelProjectOptions { FieldNames = ["Name", "Residence.Address"] }
            },
            """
            Name: Alfred Nebel
            Residence.Address: 42 Abbeter Way, Inprooving, WORCS

            Name: Mandy Nebel
            Residence.Address: 42 Abbeter Way, Inprooving, WORCS

            Name: Ernest Wright
            Residence.Address: 1 Wanter Rise, Greater Inncombe, BUCKS
            """
        ];

        yield return
        [
            "type_group_count_sort",
            (int)RecExampleScenario.MultipleRecordTypesSingleFile,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Group = new RecSelGroupOptions { FieldNames = ["Abode"] },
                Aggregate = new RecSelAggregateOptions { Count = true, CountFieldName = "Total" },
                Sort = new RecSelSortOptions { FieldNames = ["Abode"] }
            },
            """
            Abode: 42AbbeterWay
            Total: 2

            Abode: ChezGrampa
            Total: 1
            """
        ];

        yield return
        [
            "type_sort_collapse",
            (int)RecExampleScenario.MultipleRecordTypesSingleFile,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { Collapse = true },
                Sort = new RecSelSortOptions { FieldNames = ["Name"] }
            },
            """
            Name: Alfred Nebel
            Email: alf@example.com
            Abode: 42AbbeterWay
            Name: Ernest Wright
            Abode: ChezGrampa
            Name: Mandy Nebel
            Email: mandy@example.com
            Abode: 42AbbeterWay
            """
        ];

        yield return
        [
            "type_join_projection_uniq",
            (int)RecExampleScenario.MultipleRecordTypesSingleFile,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { JoinField = "Abode", Uniq = true },
                Project = new RecSelProjectOptions { FieldNames = ["Residence.Address", "Residence.Address", "Name"] }
            },
            """
            Name: Alfred Nebel
            Residence.Address: 42 Abbeter Way, Inprooving, WORCS

            Name: Mandy Nebel
            Residence.Address: 42 Abbeter Way, Inprooving, WORCS

            Name: Ernest Wright
            Residence.Address: 1 Wanter Rise, Greater Inncombe, BUCKS
            """
        ];

        yield return
        [
            "type_include_descriptors",
            (int)RecExampleScenario.MultipleRecordTypesSingleFile,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { IncludeDescriptors = true }
            },
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
            """
        ];

        yield return
        [
            "type_include_descriptors_collapse",
            (int)RecExampleScenario.MultipleRecordTypesSingleFile,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { IncludeDescriptors = true, Collapse = true }
            },
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
            """
        ];
    }

    private static string NormalizeForComparison(string text)
    {
        return text.Replace("\r\n", "\n", StringComparison.Ordinal).TrimEnd('\n');
    }
}
