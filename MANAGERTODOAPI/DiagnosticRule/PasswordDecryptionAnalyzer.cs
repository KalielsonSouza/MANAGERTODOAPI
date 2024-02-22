using System;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MANAGERTODOAPI.DiagnosticRule
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PasswordDecryptionAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "PasswordDecryptionAnalyzer";

        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            "Uso da função 'PasswordEncryptor'",
            "A função 'PasswordEncryptor' está sendo usada para criptografar a senha. Fusnção legado, por favor utilizado nova função.",
            "Security",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.ReportDiagnostics);

            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            // Verifica se é uma chamada para a função 'HashPassword' do objeto 'passwordEncryptor'
            if (invocationExpression.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Expression is IdentifierNameSyntax identifierName &&
                identifierName.Identifier.Text == "passwordEncryptor" &&  // Verifica se está chamando 'passwordEncryptor'
                memberAccess.Name.Identifier.Text == "HashPassword")
            {
                // Relate um diagnóstico de uso da função 'passwordEncryptor'
                var diagnostic = Diagnostic.Create(Rule, invocationExpression.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
