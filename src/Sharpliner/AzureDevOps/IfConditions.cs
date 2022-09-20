// using System.Collections.Generic;
// using Sharpliner.AzureDevOps.ConditionedExpressions;
//
// namespace Sharpliner.AzureDevOps;
//
// public class IfVariableParameterCondition : IfStringCondition
// {
//     internal IfVariableParameterCondition(string keyword, StaticVariableReference variable, ParameterReference parameter)
//         : base(keyword, Serialize(variable), Serialize(parameter))
//     {
//     }
// }
//
// public class IfVariableVariableCondition : IfStringCondition
// {
//     internal IfVariableVariableCondition(string keyword, StaticVariableReference variable, StaticVariableReference variable2)
//         : base(keyword, Serialize(variable), Serialize(variable2))
//     {
//     }
// }
//
// public class IfVariableStringCondition : IfStringCondition
// {
//     internal IfVariableStringCondition(string keyword, StaticVariableReference variable, string str)
//         : base(keyword, Serialize(variable), str)
//     {
//     }
// }
//
// public class IfVariableArrayCondition : IfStringCondition
// {
//     internal IfVariableArrayCondition(string keyword, StaticVariableReference variable, IEnumerable<object> array)
//         : base(keyword, Serialize(variable), Serialize(array))
//     {
//     }
// }
//
// public class IfStringParameterCondition : IfStringCondition
// {
//     internal IfStringParameterCondition(string keyword, string str, ParameterReference parameter)
//         : base(keyword, str, Serialize(parameter))
//     {
//     }
// }
//
// public class IfStringVariableCondition : IfStringCondition
// {
//     internal IfStringVariableCondition(string keyword, string str, StaticVariableReference variable)
//         : base(keyword, str, Serialize(variable))
//     {
//     }
// }
//
// public class IfStringArrayCondition : IfStringCondition
// {
//     internal IfStringArrayCondition(string keyword, string str, IEnumerable<object> array)
//         : base(keyword, str, Serialize(array))
//     {
//     }
// }
//
// public class IfParameterVariableCondition : IfStringCondition
// {
//     internal IfParameterVariableCondition(string keyword, ParameterReference parameter, StaticVariableReference variable)
//         : base(keyword, Serialize(parameter), Serialize(variable))
//     {
//     }
// }
//
// public class IfParameterParameterCondition : IfStringCondition
// {
//     internal IfParameterParameterCondition(string keyword, ParameterReference parameter, ParameterReference parameter2)
//         : base(keyword, Serialize(parameter), Serialize(parameter2))
//     {
//     }
// }
//
// public class IfParameterStringCondition : IfStringCondition
// {
//     internal IfParameterStringCondition(string keyword, ParameterReference parameter, string str)
//         : base(keyword, Serialize(parameter), str)
//     {
//     }
// }
//
// public class IfParameterArrayCondition : IfStringCondition
// {
//     internal IfParameterArrayCondition(string keyword, ParameterReference parameter, IEnumerable<object> array)
//         : base(keyword, Serialize(parameter), Serialize(array))
//     {
//     }
// }
//
//
//
// public class InlineVariableParameterCondition : InlineStringCondition
// {
//     internal InlineVariableParameterCondition(string keyword, VariableReference variable, ParameterReference parameter)
//         : base(keyword, Serialize(variable), Serialize(parameter))
//     {
//     }
// }
//
// public class InlineVariableVariableCondition : InlineStringCondition
// {
//     internal InlineVariableVariableCondition(string keyword, VariableReference variable, VariableReference variable2)
//         : base(keyword, Serialize(variable), Serialize(variable2))
//     {
//     }
// }
//
// public class InlineVariableStringCondition : InlineStringCondition
// {
//     internal InlineVariableStringCondition(string keyword, VariableReference variable, string str)
//         : base(keyword, Serialize(variable), str)
//     {
//     }
// }
//
// public class InlineVariableArrayCondition : InlineStringCondition
// {
//     internal InlineVariableArrayCondition(string keyword, VariableReference variable, IEnumerable<object> array)
//         : base(keyword, Serialize(variable), Serialize(array))
//     {
//     }
// }
//
// public class InlineStringParameterCondition : InlineStringCondition
// {
//     internal InlineStringParameterCondition(string keyword, string str, ParameterReference parameter)
//         : base(keyword, str, Serialize(parameter))
//     {
//     }
// }
//
// public class InlineStringVariableCondition : InlineStringCondition
// {
//     internal InlineStringVariableCondition(string keyword, string str, VariableReference variable)
//         : base(keyword, str, Serialize(variable))
//     {
//     }
// }
//
// public class InlineStringArrayCondition : InlineStringCondition
// {
//     internal InlineStringArrayCondition(string keyword, string str, IEnumerable<object> array)
//         : base(keyword, str, Serialize(array))
//     {
//     }
// }
//
// public class InlineParameterVariableCondition : InlineStringCondition
// {
//     internal InlineParameterVariableCondition(string keyword, ParameterReference parameter, VariableReference variable)
//         : base(keyword, Serialize(parameter), Serialize(variable))
//     {
//     }
// }
//
// public class InlineParameterParameterCondition : InlineStringCondition
// {
//     internal InlineParameterParameterCondition(string keyword, ParameterReference parameter, ParameterReference parameter2)
//         : base(keyword, Serialize(parameter), Serialize(parameter2))
//     {
//     }
// }
//
// public class InlineParameterStringCondition : InlineStringCondition
// {
//     internal InlineParameterStringCondition(string keyword, ParameterReference parameter, string str)
//         : base(keyword, Serialize(parameter), str)
//     {
//     }
// }
//
// public class InlineParameterArrayCondition : InlineStringCondition
// {
//     internal InlineParameterArrayCondition(string keyword, ParameterReference parameter, IEnumerable<object> array)
//         : base(keyword, Serialize(parameter), Serialize(array))
//     {
//     }
// }
