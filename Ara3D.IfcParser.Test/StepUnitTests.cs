using Ara3D.Parakeet;
using Ara3D.Parakeet.Grammars;
using Ara3D.Utils;

namespace Ara3D.IfcParser.Test
{
    public static class StepUnitTests
    {
        [Test, Explicit]
        public static void OutputFilesInFolder()
        {
            var folder = new DirectoryPath(@"C:\Users\cdigg\dev\impraria");
            var files = folder.GetAllFilesRecursively().Where(f => f.HasExtension("ifc")).OrderBy(f => f.GetFileSize());

            foreach (var f in files)
            {
                Console.WriteLine($@"// {f.FileSizeAsString()}");
                Console.WriteLine($"@\"{f.Value}\",");
            }
        }


        public static FilePath HugeFile =
            @"C:\Users\cdigg\dev\impraria\02 - Gulf of Aqaba\4800000194 - GOA Romantic Bay\Stage 3A\IFC\KEQ\02-211211-4800000194-WBP-KEQ-MDL-000001.ifc";

        public static FilePath BigFile =
            @"C:\Users\cdigg\dev\impraria\02 - Gulf of Aqaba\4800000194 - GOA Romantic Bay\Stage 3A\IFC\ARC\02-211211-4800000194-WBP-ARC-MDL-000001.ifc";

        public static FilePath MedFile =
            @"C:\Users\cdigg\dev\impraria\02 - Gulf of Aqaba\4800000194 - GOA Romantic Bay\Stage 3A\IFC\ARC\02-211211-4800000194-WBP-ARC-MDL-000003.ifc";

        public static FilePath TinyFile =
            @"C:\Users\cdigg\dev\impraria\02 - Gulf of Aqaba\4800000194 - GOA Romantic Bay\Stage 3A\IFC\ARC\02-211211-4800000194-WBP-ARC-MDL-000000.ifc";

        public static void OutputTokenAnalysis(StepTokenAnalysis sta)
        {
            Console.WriteLine($"There are {sta.Indices.Count} unique token values");

            var tokenGroups = sta
                .Indices
                .Where(kv => kv.Value.Count > 2)
                .OrderByDescending(kv => kv.Value.Count)
                .Take(50);

            foreach (var g in tokenGroups)
                Console.WriteLine($"{g.Key} = {g.Value.Count}");
        }

        [Test]
        [TestCaseSource(nameof(TestFiles))]
        public static void TestTokenizer(string filePath)
        {
            var testFile = new FilePath(filePath);
            var bytes = Array.Empty<byte>();
            var tokens = Array.Empty<StepToken>();

            IReadOnlyList<StepEntity> entities = null;

            TimingUtils.TimeIt(() => { bytes = testFile.ReadAllBytes(); },
                "Loading Bytes");

            /*
            TimingUtils.TimeIt(() =>
                {
                    seps = StepTokenizer.GetSeparatorIndices(bytes);
                    Console.WriteLine($"Found {seps.Count} separators");
                },
                "Computing Separators");
            */

            TimingUtils.TimeIt(() =>
                {
                    tokens = StepTokenizer.CreateTokens(bytes);
                    Console.WriteLine($"Found {tokens.Length} tokens");
                },
                "Tokenizing");

            var size = 0;
            foreach (var token in tokens)
            {
                size += token.GetByteSize();
                //Console.WriteLine(token.GetString(bytes));
            }

            Console.WriteLine($"File size = {testFile.GetFileSize()}, current size = {size}");

            TimingUtils.TimeIt(() =>
                {
                    var sta = new StepTokenAnalysis(bytes, tokens, TokenType.Id);
                    OutputTokenAnalysis(sta);
                },
                "Analyzing ID tokens");

            TimingUtils.TimeIt(() =>
                {
                    var sta = new StepTokenAnalysis(bytes, tokens, TokenType.String);
                    OutputTokenAnalysis(sta);
                },
                "Analyzing String tokens");

            TimingUtils.TimeIt(() =>
                {
                    var sta = new StepTokenAnalysis(bytes, tokens, TokenType.Ident);
                    OutputTokenAnalysis(sta);
                },
                "Analyzing Identifier tokens");
        }

        [Test]
        [TestCaseSource(nameof(TestFiles))]
        public static void Test(string filePath)
        {
            var testFile = new FilePath(filePath);
            var lines = Array.Empty<string>();
            var bytes = Array.Empty<byte>();

            IReadOnlyList<StepEntity> entities = null;

            TimingUtils.TimeIt(() => { bytes = testFile.ReadAllBytes(); }, "Loading Bytes");


            TimingUtils.TimeIt(() => { lines = testFile.ReadAllLines(); }, "Loading Lines");

            TimingUtils.TimeIt(() => { entities = ParseLines(lines); }, "Creating entities");

            TimingUtils.TimeIt(() =>
            {
                foreach (var e in entities)
                    ParseRawData(e);
            }, "Parsing values");

            var errors = entities.Where(e => e.Error != EntityError.None).ToList();
            Console.WriteLine($"Number of error entities {errors.Count}");

            var nameGroups = entities.GroupBy(e => e.Name).OrderBy(e => e.Key).ToList();
            foreach (var grp in nameGroups)
            {
                Console.WriteLine($"{grp.Key} = {grp.Count()}");
            }
        }

        public static StepGrammar Grammar = StepGrammar.Instance;

        public static void ParseRawData(StepEntity entity)
        {
            if (string.IsNullOrEmpty(entity.RawData))
                return;
            /*
            var (value, index) = StepValueParser.ParseAggregate(entity.RawData);
            if (index != entity.RawData.Length)
                throw new Exception("Did not reach end of data during parse");
            entity.Data = value;
            */
            var rule = Grammar.EntityData;
            entity.State = rule.Parse(entity.RawData);
        }

        public static StepEntity ParseLine(string entry, int index)
        {
            var r = new StepEntity();
            r.Index = index;
            r.Length = entry.Length;

            if (!entry.StartsWith('#'))
            {
                r.Error = EntityError.NoHash;
                return r;
            }

            if (!entry.EndsWith(';'))
            {
                r.Error = EntityError.NoSemicolon;
                return r;
            }

            var n = entry.IndexOf('=');
            if (n < 0)
            {
                r.Error = EntityError.NoEquals;
                return r;
            }

            var idStr = entry.Substring(1, n - 1);
            if (!int.TryParse(idStr, out r.Id))
            {
                r.Error = EntityError.NoId;
                return r;
            }

            var n1 = entry.IndexOf('(');
            if (n1 < 0)
            {
                r.Error = EntityError.NoParen;
                return r;
            }

            r.Name = entry.Substring(n + 1, n1 - n - 1);
            r.RawData = entry.Substring(n1, entry.Length - n1 - 1);
            return r;
        }

        public static IReadOnlyList<StepEntity> ParseLines(IReadOnlyList<string> lines)
            => lines.Select(ParseLine).ToList();

        public static void OutputParseErrors(ParserState state)
        {
            for (var e = state.LastError; e != null; e = e.Previous)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.State.CurrentLine);
                Console.WriteLine(e.State.Indicator);
            }
        }

        public static int ParseTest(ParserInput input, Rule rule, bool outputInput = false)
        {
            if (outputInput)
            {
                Console.WriteLine($"Testing Rule {rule} with input {input}");
            }
            else
            {
                Console.WriteLine($"Testing Rule {rule}");
            }

            ParserState ps = null;
            try
            {
                ps = rule.Parse(input);
            }
            catch (ParserException pe)
            {
                Console.WriteLine($"Parsing exception {pe.Message} occurred at {pe.LastValidState} ");
            }

            if (ps != null)
            {
                OutputParseErrors(ps);

                if (ps.LastError != null)
                {
                    return 0;
                }
            }

            if (ps == null)
            {
                Console.WriteLine($"FAILED");
            }
            else if (ps.AtEnd())
            {
                Console.WriteLine($"PASSED");
            }
            else
            {
                Console.WriteLine($"PARTIAL PASSED: {ps.Position}/{ps.Input.Length}");
            }

            // Check that optimized rules produce the same output 
            var optimizedRule = rule.Optimize();
            var ps2 = optimizedRule.Parse(input);

            Assert.IsTrue(ps == null ? ps2 == null : ps2 != null);

            if (ps == null || ps2 == null)
                return 0;

            Assert.AreEqual(ps.Position, ps2.Position);

            if (ps == null)
                return 0;

            if (rule is NodeRule)
            {
                if (ps.Node == null)
                {
                    Console.WriteLine($"No parse node created");
                    return 0;
                }

                Console.WriteLine($"Node {ps.Node}");

                var treeAndNode = ps.Node.ToParseTreeAndNode();
                var tree = treeAndNode.Item1;
                if (tree == null)
                {
                    Console.WriteLine($"No parse tree created");
                    return 0;
                }
                //Console.WriteLine($"Tree {tree}");
            }

            return ps.AtEnd() ? 1 : 0;
        }

        public static IEnumerable<string> TestFiles()
        {
            yield return HugeFile;
            yield return BigFile;
            yield return MedFile;
            yield return TinyFile;
        }

        public static IEnumerable<string> SmallTestFiles()
        {
            yield return MedFile;
            yield return TinyFile;
        }

        [Test]
        [TestCaseSource(nameof(SmallTestFiles))]
        public static void TestGrammar(string filePath)
        {
            var g = StepGrammar.Instance;
            var file = new FilePath(filePath);
            var text = file.ReadAllText();
            var input = new ParserInput(text, filePath);
            TimingUtils.TimeIt(() =>
            {
                var n = ParseTest(input, g.StartRule);
                Console.WriteLine($"Parse test result {n == 1}");
            });
        }
    }
}