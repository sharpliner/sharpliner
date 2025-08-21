// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Style", "IDE0130:Namespace does not match folder structure", Justification = "Test project code is used for samples in documentation so we need concise syntax", Scope = "namespace", Target = "~N:MyProject.Pipelines")]
[assembly: SuppressMessage("Style", "IDE0130:Namespace does not match folder structure", Justification = "Test project code is used for samples in documentation so we need concise syntax", Scope = "namespaceanddescendants", Target = "~N:Sharpliner.Tests")]
[assembly: SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Test project code is used for samples in documentation so we need concise syntax", Scope = "namespaceanddescendants", Target = "~N:Sharpliner.Tests")]
[assembly: SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Test project code is used for samples in documentation so we need concise syntax", Scope = "namespaceanddescendants", Target = "~N:Sharpliner.Tests")]
