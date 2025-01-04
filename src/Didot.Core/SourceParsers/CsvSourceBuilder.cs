using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using PocketCsvReader.Configuration;

namespace Didot.Core.SourceParsers;

[Extension(".csv")]
internal class CsvSourceBuilder : BaseSourceBuilder<CsvSource>
{
    protected Dictionary<string, Action<string>> Actions = [];

    private DialectDescriptorBuilder _dialect = new();

    public CsvSourceBuilder()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        Actions.Add("delimiter", (delimiter) => _dialect.WithDelimiter(MapEnumToChar<Delimiter>(delimiter)));
        Actions.Add("lineTerminator", (lineTerminator) => _dialect.WithLineTerminator(MapLineTerminator(lineTerminator)));
        Actions.Add("quoteChar", (quoteChar) => _dialect.WithQuoteChar(MapEnumToChar<QuoteChar>(quoteChar)));
        Actions.Add("doubleQuote", (doubleQuote) => _dialect.WithDoubleQuote(doubleQuote.ToBoolean()));
        Actions.Add("escapeChar", (escapeChar) => _dialect.WithEscapeChar(MapEnumToChar<EscapeChar>(escapeChar)));
        Actions.Add("nullSequence", (nullSequence) => _dialect.WithNullSequence(nullSequence));
        Actions.Add("skipInitialSpace", (skipInitialSpace) => _dialect.WithSkipInitialSpace(skipInitialSpace.ToBoolean()));
        Actions.Add("commentChar", (commentChar) => _dialect.WithCommentChar(MapEnumToChar<CommentChar>(commentChar)));
        Actions.Add("commentRows", (commentRows) => _dialect.WithCommentRows(commentRows.ToArrayInt()));
        Actions.Add("header", (header) => _dialect.WithHeader(header.ToBoolean()));
        Actions.Add("headerRows", (headerRows) => _dialect.WithHeaderRows(headerRows.ToArrayInt()));
        Actions.Add("headerJoin", (headerJoin) => _dialect.WithHeaderJoin(headerJoin));
    }

    public override ISourceParser Build(IDictionary<string, string> parameters, string extension)
    {
        extension = $"{extension.NormalizeExtension()}@".Substring(1);
        foreach (var kv in parameters.Where(x => x.Key.StartsWith(extension)))
            if (Actions.TryGetValue(kv.Key.Split('@')[1], out var action))
                action(kv.Value);

        var builder = new CsvReaderBuilder();
        var csvReader = builder.WithDialect((_) => _dialect).Build();
        return new CsvSource(csvReader);
    }

    private char MapEnumToChar<T>(string value) where T : Enum
    {
        if (value.Length == 1)
            return value[0];

        return (char)(int)Enum.Parse(typeof(T),
            Enum.GetNames(typeof(T)).FirstOrDefault(x => x.Equals(value, StringComparison.InvariantCultureIgnoreCase))
            ?? throw new NotSupportedException(value)
        );
    }

    private string MapLineTerminator(string value)
    {
        if (value.Length >= 1 && value.Length <= 2)
            return value;

        var name = Enum.Parse<LineTerminator>(
            Enum.GetNames<LineTerminator>().FirstOrDefault(x => x.Equals(value, StringComparison.InvariantCultureIgnoreCase))
            ?? throw new NotSupportedException(value));

        return name switch
        {
            LineTerminator.CarriageReturn => "\r",
            LineTerminator.LineFeed => "\n",
            LineTerminator.CarriageReturnLineFeed => "\r\n",
            _ => throw new NotSupportedException(value),
        };
    }
}
