namespace JetBrains.ReSharper.Plugins.FSharp.Tests.Features

open System
open System.IO
open JetBrains.ProjectModel
open JetBrains.ReSharper.Feature.Services.Daemon
open JetBrains.ReSharper.FeaturesTestFramework.Daemon
open JetBrains.ReSharper.Plugins.FSharp.ProjectModelBase
open JetBrains.ReSharper.Plugins.FSharp.Psi
open JetBrains.ReSharper.Psi
open JetBrains.ReSharper.Psi.Files
open JetBrains.ReSharper.Psi.Resolve
open NUnit.Framework

type CSharpResolveTest() =
    inherit TestWithTwoProjects()

    let highlightingManager = HighlightingSettingsManager.Instance

    [<Test>] member x.``Records 01 - Generated members``() = x.DoNamedTest()
    [<Test>] member x.``Records 02 - CliMutable``() = x.DoNamedTest()
    [<Test>] member x.``Records 03 - Override generated members``() = x.DoNamedTest()
    [<Test>] member x.``Records 04 - Sealed``() = x.DoNamedTest()
    [<Test>] member x.``Records 05 - Struct``() = x.DoNamedTest()
    [<Test>] member x.``Records 06 - Struct CliMutable``() = x.DoNamedTest()

    [<Test>] member x.``Exceptions 01 - Empty``() = x.DoNamedTest()
    [<Test>] member x.``Exceptions 02 - Single field``() = x.DoNamedTest()
    [<Test>] member x.``Exceptions 03 - Multiple fields``() = x.DoNamedTest()
    [<Test>] member x.``Exceptions 04 - Protected ctor``() = x.DoNamedTest()

    [<Test>] member x.``Unions 01 - Simple generated members``() = x.DoNamedTest()
    [<Test>] member x.``Unions 02 - Singletons``() = x.DoNamedTest()
    [<Test>] member x.``Unions 03 - Nested types``() = x.DoNamedTest()
    [<Test>] member x.``Unions 04 - Single case with fields``() = x.DoNamedTest()
    [<Test>] member x.``Unions 05 - Struct single case with fields``() = x.DoNamedTest()
    [<Test>] member x.``Unions 06 - Struct nested types``() = x.DoNamedTest()
    [<Test>] member x.``Unions 07 - Private representation 01, singletons``() = x.DoNamedTest()
    [<Test>] member x.``Unions 08 - Private representation 02, nested types``() = x.DoNamedTest()
    [<Test>] member x.``Unions 09 - Private representation 03, struct``() = x.DoNamedTest()

    [<Test>] member x.``Simple types 01 - Members``() = x.DoNamedTest()

    [<Test>] member x.``Class 01 - Abstract``() = x.DoNamedTest()
    [<Test>] member x.``Class 02 - Sealed``() = x.DoNamedTest()

    override x.RelativeTestDataPath = "cache/csharpResolve"

    override x.MainFileExtension = CSharpProjectFileType.CS_EXTENSION
    override x.SecondFileExtension = FSharpProjectFileType.FsExtension

    override x.DoTest(project: IProject, secondProject: IProject) =
        x.Solution.GetPsiServices().Files.CommitAllDocuments()
        x.ExecuteWithGold(fun writer ->
            let projectFile = project.GetAllProjectFiles() |> Seq.exactlyOne
            let sourceFile = projectFile.ToSourceFiles().Single()
            let psiFile = sourceFile.GetPrimaryPsiFile()

            let daemon = TestHighlightingDumper(sourceFile, writer, null, Func<_,_,_,_>(x.ShouldHighlight))
            daemon.DoHighlighting(DaemonProcessKind.VISIBLE_DOCUMENT)
            daemon.Dump()

            let referenceProcessor = RecursiveReferenceProcessor(fun r -> x.ProcessReference(r, writer))
            psiFile.ProcessThisAndDescendants(referenceProcessor)) |> ignore

    member x.ShouldHighlight highlighting sourceFile settings =
        let severity = highlightingManager.GetSeverity(highlighting, sourceFile, x.Solution, settings)
        severity = Severity.ERROR

    member x.ProcessReference(reference: IReference, writer: TextWriter) =
        match reference.Resolve().DeclaredElement with
        | :? IFSharpTypeMember as typeMember -> writer.WriteLine(typeMember.XMLDocId)
        | _ -> ()
