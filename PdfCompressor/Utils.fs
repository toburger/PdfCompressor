module Utils

module Boolean =
    let tryParse input =
        match System.Boolean.TryParse input with
        | true, v -> Some v
        | false, _ -> None

module Int32 =
    let tryParse input =
        match System.Int32.TryParse input with
        | true, v -> Some v
        | false, _ -> None

module PdfSettings =
    let tryParse = function
        | "screen" -> Some Pdf.Screen
        | "ebook" -> Some Pdf.EBook
        | "prepress" -> Some Pdf.Prepress
        | "printer" -> Some Pdf.Printer
        | "default" -> Some Pdf.Default
        | _ -> None