using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;
using System.Linq;
using System.Collections.Immutable;

namespace MANAGERTODOAPI.DiagnosticRule
{
    public class startDiagnosis
    {
        public async Task TestAnalyzerOnSolutionAsync(DiagnosticAnalyzer analyzer)
        {
            string solutionPath = "E:\\REPOAPI\\MANAGERTODOAPI\\MANAGERTODOAPI.sln";

            using (var workspace = MSBuildWorkspace.Create())
            {
                var solution = await workspace.OpenSolutionAsync(solutionPath);

                List<string> diagnosticLog = new List<string>();

                foreach (var projectId in solution.ProjectIds)
                {
                    var project = solution.GetProject(projectId);

                    var projectDirectory = Path.GetDirectoryName(project.FilePath);

                    var csFiles = Directory.GetFiles(projectDirectory, "*.cs", SearchOption.AllDirectories);
                    foreach (var csFile in csFiles)
                    {
                        var syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(csFile));
                        var compilation = CSharpCompilation.Create("TempCompilation")
                            .AddReferences(project.MetadataReferences)
                            .AddSyntaxTrees(syntaxTree);

                        var analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(analyzer);
                        var compilationWithAnalyzers = compilation.WithAnalyzers(analyzers);
                        var diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();

                        foreach (var diagnostic in diagnostics)
                        {                            
                            var location = diagnostic.Location.GetMappedLineSpan();
                            string diagnosticMessage = $"Diagnóstico encontrado: {diagnostic.GetMessage()} em {csFile}, linha {location.StartLinePosition.Line}, coluna {location.StartLinePosition.Character}";
                            diagnosticLog.Add(diagnosticMessage);
                        }
                    }
                }

                // Salvar o registro de diagnósticos em um arquivo de log
                File.WriteAllLines($"Diagnósticos{analyzer.ToString()}.log", diagnosticLog);
            }
        }
    }
}
