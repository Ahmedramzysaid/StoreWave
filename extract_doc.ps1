Add-Type -AssemblyName 'System.IO.Compression.FileSystem'

$docPath = Join-Path $PSScriptRoot 'LL(Final Documentation).docx'
$zip = [System.IO.Compression.ZipFile]::OpenRead($docPath)
$entry = $zip.GetEntry('word/document.xml')
$stream = $entry.Open()
$reader = New-Object System.IO.StreamReader($stream)
$xmlContent = $reader.ReadToEnd()
$reader.Close()
$stream.Close()
$zip.Dispose()

# Parse XML and extract text with styles
$xml = [xml]$xmlContent
$ns = New-Object System.Xml.XmlNamespaceManager($xml.NameTable)
$ns.AddNamespace('w', 'http://schemas.openxmlformats.org/wordprocessingml/2006/main')

$output = @()
$paragraphs = $xml.SelectNodes('//w:p', $ns)

foreach ($p in $paragraphs) {
    $styleNode = $p.SelectSingleNode('.//w:pPr/w:pStyle/@w:val', $ns)
    $style = if ($styleNode) { $styleNode.Value } else { 'Normal' }
    
    $texts = $p.SelectNodes('.//w:r/w:t', $ns)
    $fullText = ($texts | ForEach-Object { $_.InnerText }) -join ''
    
    if ($fullText.Trim()) {
        $output += "[$style] $($fullText.Trim())"
    }
}

$outputPath = Join-Path $PSScriptRoot 'docx_extracted.txt'
$output | Out-File -FilePath $outputPath -Encoding UTF8
Write-Host "Extracted $($output.Count) paragraphs to docx_extracted.txt"
