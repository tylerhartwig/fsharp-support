namespace JetBrains.ReSharper.Plugins.FSharp.Daemon.Stages

open JetBrains.ReSharper.Daemon.UsageChecking
open JetBrains.ReSharper.Feature.Services.Daemon
open JetBrains.ReSharper.Plugins.FSharp.Common.Util
open JetBrains.ReSharper.Plugins.FSharp.Daemon.Cs
open JetBrains.ReSharper.Plugins.FSharp.Daemon.Cs.Stages
open JetBrains.ReSharper.Plugins.FSharp.Daemon.Highlightings
open JetBrains.ReSharper.Plugins.FSharp.Psi.Tree
open JetBrains.ReSharper.Plugins.FSharp.Psi.Util
open Microsoft.FSharp.Compiler.SourceCodeServices

[<AllowNullLiteral>]
type UnusedOpensStageProcess(fsFile: IFSharpFile, checkResults, daemonProcess) =
    inherit FSharpDaemonStageProcessBase(daemonProcess)

    let document = fsFile.GetSourceFile().Document
    let lines = document.GetLines()
    let getLine line = lines.[line - 1]
    
    override x.Execute(committer) =
        let highlightings = ResizeArray()
        let interruptChecker = daemonProcess.CreateInterruptChecker()
        for range in UnusedOpens.getUnusedOpens(checkResults, getLine).RunAsTask(interruptChecker) do
            let range = range.ToDocumentRange(document)
            highlightings.Add(HighlightingInfo(range, UnusedWarningHighlighting(range)))
            x.SeldomInterruptChecker.CheckForInterrupt()
        committer.Invoke(DaemonStageResult(highlightings))

[<DaemonStage(StagesBefore = [| typeof<HighlightIdentifiersStage> |], StagesAfter = [| typeof<CollectUsagesStage> |])>]
type UnusedOpensStage(daemonProcess, errors) =
    inherit FSharpDaemonStageBase()

    override x.CreateProcess(fsFile, daemonProcess) =
        daemonProcess.CustomData.GetData(FSharpDaemonStageBase.TypeCheckResults)
        |> Option.map (fun checkResults -> UnusedOpensStageProcess(fsFile, checkResults, daemonProcess))
        |> Option.defaultValue null :> _

    override x.NeedsErrorStripe(_, _) = ErrorStripeRequest.STRIPE_AND_ERRORS
