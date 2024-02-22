using System;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MANAGERTODOAPI.DiagnosticRule
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SwitchCaseCountAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SwitchCaseCountAnalyzer";
        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            "Switch muito complexo",
            "O switch statement contém mais de 8 casos. Considere refatorar utlizando outra estrutura.",
            "Code Quality",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.SwitchStatement);
        }

        private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var switchStatement = (SwitchStatementSyntax)context.Node;
            var caseCount = switchStatement.Sections.Sum(section => section.Labels.Count);

            if (caseCount > 7)
            {
                var diagnostic = Diagnostic.Create(Rule, switchStatement.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
