module Pdf

open System.IO
open Ghostscript.NET

type PdfSettings =
    | Screen
    | EBook
    | Prepress
    | Printer
    | Default
    override self.ToString() =
        match self with
        | Screen -> "screen"
        | EBook -> "ebook"
        | Prepress -> "prepress"
        | Printer -> "printer"
        | Default -> "default"

type QualitySettings =
    { PdfSettings: PdfSettings
      ImageResolution: int
      EmbedFonts: bool }
    static member Default =
        { PdfSettings = Screen
          ImageResolution = 96
          EmbedFonts = true }

let compress qualitySettings inputFile =
    use pipedOut = new GhostscriptPipedOutput()

    let pipedOutHandle = sprintf "%%handle%%%x" (int pipedOut.ClientHandle)

    let gv = GhostscriptVersionInfo.GetLastInstalledVersion()

    use p = new Processor.GhostscriptProcessor(gv, true)

    let args =
        [| "gs"
           "-sDEVICE=pdfwrite"
           "-dCompatibilityLevel=1.3"
           (sprintf "-dPDFSETTINGS=/%O" qualitySettings.PdfSettings)
           "-dBATCH"
           (sprintf "-dEmbedAllFonts=%b" qualitySettings.EmbedFonts)
           "-dSubsetFonts=true"
           "-dColorImageDownsampleType=/Bicubic"
           (sprintf "-dColorImageResolution=%i" qualitySettings.ImageResolution)
           "-dGrayImageDownsampleType=/Bicubic"
           (sprintf "-dGrayImageResolution=%i" qualitySettings.ImageResolution)
           "-dMonoImageDownsampleType=/Bicubic"
           (sprintf "-dMonoImageResolution=%i" qualitySettings.ImageResolution)
           (sprintf "-o%s" pipedOutHandle)
           inputFile |]

    p.StartProcessing(args, null)

    pipedOut.Data

let compressStream qualitySettings (stream: Stream) =
    let tmpFile = Path.GetTempFileName()
    try
        using
            (File.OpenWrite tmpFile)
            (fun wStream -> stream.CopyTo wStream)
        compress qualitySettings tmpFile
    finally
        if File.Exists tmpFile then
            File.Delete tmpFile

let compressBytes qualitySettings (bytes: byte[]) =
    use stream = new MemoryStream(bytes)
    compressStream qualitySettings stream

