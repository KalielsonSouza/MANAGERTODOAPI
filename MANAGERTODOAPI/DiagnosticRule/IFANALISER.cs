using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MANAGERTODOAPI.DiagnosticRule
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IFANALISER: DiagnosticAnalyzer
    {
        public const string DiagnosticId = "IFANALISER";

        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            "Aninhamento excessivo de blocos 'if-else'",
            "Evite blocos 'if-else' excessivos com outroas estruturas (Acima do máximo permitido: {0}).",
            "Clean Code",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            // Use a lambda expression to define the action directly
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.ReportDiagnostics);

            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.IfStatement);
        }

        private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = (IfStatementSyntax)context.Node;

            int depth = CountIfElseDepth(ifStatement);
            if (depth >= 3)
            {
                var diagnostic = Diagnostic.Create(Rule, ifStatement.GetLocation(), depth);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static int CountIfElseDepth(IfStatementSyntax ifStatement)
        {
            int depth = 0;
            while (ifStatement != null)
            {
                depth++;
                ifStatement = ifStatement.Else?.Statement as IfStatementSyntax;
            }
            return depth;
        }
    }
}
