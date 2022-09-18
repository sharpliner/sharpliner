using System.CodeDom.Compiler;

namespace Sharpliner.SourceGenerator;

public class CodeGenerationTextWriter : IndentedTextWriter
{
    public CodeGenerationTextWriter()
 : base(new StringWriter())
    {
    }

    public override void WriteLine(string s)
    {
        var trimmed = s.Trim();

        if (trimmed.StartsWith("}"))
        {
            Indent--;
        }

        base.WriteLine(s);

        if (trimmed.StartsWith("{"))
        {
            Indent++;
        }
    }

    public override string ToString()
    {
        Flush();
        return InnerWriter.ToString();
    }
}
