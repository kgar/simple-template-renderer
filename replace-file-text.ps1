Param([hashtable]$settings, [string]$sourceFilePath, [string]$outputFilePath)

$file = Get-Content $sourceFilePath

foreach ($row in $settings.GetEnumerator()) {
    $file = $file -replace ("__" + $row.Key + "__"), $row.Value
}

Set-Content -Path $outputFilePath -value $file -Force
