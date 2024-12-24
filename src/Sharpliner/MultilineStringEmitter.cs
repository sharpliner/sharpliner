using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;

namespace Sharpliner;

/// <summary>
/// This makes sure that multiline strings get serialized properly.
/// That means using `|` and not `>` (which glues lines together).
/// </summary>
internal class MultilineStringEmitter(IEventEmitter nextEmitter) : ChainedEventEmitter(nextEmitter)
{
    public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
    {

        if (typeof(string).IsAssignableFrom(eventInfo.Source.Type))
        {
            string? value = eventInfo.Source.Value as string;
            if (!string.IsNullOrEmpty(value))
            {
                bool isMultiLine = value.IndexOfAny(['\r', '\n']) >= 0;
                if (isMultiLine)
                {
                    eventInfo = new ScalarEventInfo(eventInfo.Source)
                    {
                        Style = ScalarStyle.Literal
                    };
                }
            }
        }

        nextEmitter.Emit(eventInfo, emitter);
    }
}
