# PdfCompressor
Compress PDF files with Ghostscript

## Setup

> You need Ghostscript installed on the server. You can find the latest version here: https://www.ghostscript.com/download/gsdnld.html

Deploy the compiled code to an IIS Web Application with the name PdfCompress or run as self hosted application (port 8080)

## Usage

You must provide a `POST` request with the raw PDF document as body and the Content-Type set to `application/pdf`.

query parameters:

* preset: [screen|ebook|prepress|printer|default]
* imageresolution: \<int\>
* embedfonts: \<bool\>
