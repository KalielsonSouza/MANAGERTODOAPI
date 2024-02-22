using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Text.RegularExpressions;


namespace MANAGERTODOAPI.DiagnosticRules
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PadraoVar : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "PadraoVar";
        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            "Nome de variável não segue o padrão",
            "O nome da variável '{0}' não segue o padrão 'Cobertura + Contexto'.",
            "Naming Conventions",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.VariableDeclaration);
        }

        private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var variableDeclaration = (VariableDeclarationSyntax)context.Node;

            foreach (var variable in variableDeclaration.Variables)
            {
                var variableName = variable.Identifier.Text;

                
                if (!Regex.IsMatch(variableName, @"^Cobertura\w*"))
                {
                    var diagnostic = Diagnostic.Create(Rule, variable.Identifier.GetLocation(), variableName);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
