module Program

open Suave
open Suave.Filters
open Suave.Operators
open Utils

let extractQualitySettings (req: HttpRequest) =
    let def = Pdf.QualitySettings.Default
    let pdfSettings =
        req.queryParamOpt "preset"
        |> Option.bind snd
        |> Option.bind PdfSettings.tryParse
        |> Option.defaultValue def.PdfSettings
    let imageResolution =
        req.queryParamOpt "imageresolution"
        |> Option.bind snd
        |> Option.bind Int32.tryParse
        |> Option.defaultValue def.ImageResolution
    let embedFonts =
        req.queryParamOpt "embedfonts"
        |> Option.bind snd
        |> Option.bind Boolean.tryParse
        |> Option.defaultValue def.EmbedFonts
    { Pdf.PdfSettings = pdfSettings
      Pdf.ImageResolution = imageResolution
      Pdf.EmbedFonts = embedFonts }

let checkContentTypeForPdf = request (fun req ->
    match req.header "content-type" with
    | Choice1Of2 "application/pdf" ->
        succeed
    | _ ->
        never)

let compressPdf =
    Writers.setMimeType "application/pdf"
    >=> request (fun req ->
        let pdf = req.rawForm
        let qualitySettings = extractQualitySettings req
        let compressedPdf =
            Pdf.compressBytes
                qualitySettings
                pdf
        Successful.ok compressedPdf)

let badRequest =
    Writers.setMimeType "text/plain"
    >=> RequestErrors.BAD_REQUEST
        "You must provide a POST request \
         with the raw PDF document as body and \
         the Content-Type set to application/pdf.\n\
         \n\
         query parameters:\n\
         * preset: [screen|ebook|prepress|printer|default]\n\
         * imageresolution: <int>\n\
         * embedfonts: <bool>"

let app =
    choose [
        POST >=> checkContentTypeForPdf >=> compressPdf
        badRequest
    ]

[<EntryPoint>]
let main argv =
    let webServerConfig =
        defaultConfig
        |> IIS.Configuration.withPort argv

    startWebServer webServerConfig app

    0
