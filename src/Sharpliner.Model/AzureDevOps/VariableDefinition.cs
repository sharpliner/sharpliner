using System;
using System.Collections.Generic;

namespace Sharpliner.Model.AzureDevOps
{
    // https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#variables
    public abstract record VariableDefinition
    {
        internal ConditionedVariableDefinition? Parent { get; set; }
    }

    public record VariableGroup(string Name) : VariableDefinition;

    public record Variable : VariableDefinition
    {
        public string Name { get; }

        public object Value { get; }

        public bool Readonly { get; init; }

        private Variable(string name, object value)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public Variable(string name, string value)
            : this(name, (object)value)
        {
        }

        public Variable(string name, int value)
            : this(name, (object)value)
        {
        }

        public Variable(string name, bool value)
            : this(name, (object)value)
        {
        }

        public Variable ReadOnly()
        {
            return this with
            {
                Readonly = true
            };
        }
    }

    public record ConditionedVariableDefinition : VariableDefinition
    {
        private readonly List<VariableDefinition> _definitions = new();

        public string Condition { get; }

        internal ConditionedVariableDefinition(string condition, VariableDefinition variableDefinition)
        {
            Condition = condition;
            _definitions.Add(variableDefinition);
        }

        public ConditionedVariableDefinition Variable(string name, string value)
        {
            _definitions.Add(new Variable(name, value) { Parent = this });
            return this;
        }

        public ConditionedVariableDefinition Variable(string name, int value)
        {
            _definitions.Add(new Variable(name, value) { Parent = this });
            return this;
        }

        public ConditionedVariableDefinition Variable(string name, bool value)
        {
            _definitions.Add(new Variable(name, value) { Parent = this });
            return this;
        }

        public ConditionedVariableDefinition Group(string name)
        {
            _definitions.Add(new VariableGroup(name) { Parent = this });
            return this;
        }

        public VariableDefinitionConditionBuilder If => new(this);

        public ConditionedVariableDefinition EndIf => Parent
            ?? throw new InvalidOperationException("You have called EndIf on a top level statement. EndIf should be only used to return from a nested definition.");
    }

    public abstract class VariableDefinitionCondition
    {
        protected readonly string _condition;
        private readonly ConditionedVariableDefinition? _parent;

        protected VariableDefinitionCondition(string condition, ConditionedVariableDefinition? parent = null)
        {
            _condition = condition ?? throw new ArgumentNullException(nameof(condition));
            _parent = parent;
        }

        public ConditionedVariableDefinition Variable(string name, string value) => new(_condition, new Variable(name, value) { Parent = _parent });
        public ConditionedVariableDefinition Variable(string name, int value) => new(_condition, new Variable(name, value) { Parent = _parent });
        public ConditionedVariableDefinition Variable(string name, bool value) => new(_condition, new Variable(name, value) { Parent = _parent });
        public ConditionedVariableDefinition Group(string name) => new(_condition, new VariableGroup(name) { Parent = _parent });

        public override string ToString() => _condition;
    }

    public class EqualityVariableDefinitionCondition : VariableDefinitionCondition
    {
        internal EqualityVariableDefinitionCondition(string expression1, string expression2, bool equal)
            : base((equal ? "eq" : "ne") + $"({expression1}, {expression2})")
        {
        }
    }

    public class AndVariableDefinitionCondition : VariableDefinitionCondition
    {
        internal AndVariableDefinitionCondition(VariableDefinitionCondition expression1, VariableDefinitionCondition expression2)
            : base($"and({expression1}, {expression2})")
        {
        }
    }

    public class OrVariableDefinitionCondition : VariableDefinitionCondition
    {
        internal OrVariableDefinitionCondition(VariableDefinitionCondition expression1, VariableDefinitionCondition expression2)
            : base($"or({expression1}, {expression2})")
        {
        }
    }

    public class VariableDefinitionConditionBuilder
    {
        internal ConditionedVariableDefinition? Parent { get; }

        internal VariableDefinitionConditionBuilder(ConditionedVariableDefinition? parent = null)
        {
            Parent = parent;
        }

        public VariableDefinitionCondition Equal(string expression1, string expression2) => new EqualityVariableDefinitionCondition(expression1, expression2, true);

        public VariableDefinitionCondition Equal(VariableDefinitionCondition condition) => condition;

        public VariableDefinitionCondition NotEqual(string expression1, string expression2)
            => new EqualityVariableDefinitionCondition(expression1, expression2, false);

        public VariableDefinitionCondition NotEqual(VariableDefinitionCondition condition) => condition;

        public VariableDefinitionCondition And(VariableDefinitionCondition condition1, VariableDefinitionCondition condition2)
            => new AndVariableDefinitionCondition(condition1, condition2);

        public VariableDefinitionCondition Or(VariableDefinitionCondition condition1, VariableDefinitionCondition condition2)
            => new OrVariableDefinitionCondition(condition1, condition2);
    }
}
