$settings = .\settings.ps1
$sourceFilePath = ".\replace-text-from-hashtable.example.txt"
$outputFilePath = ".\replace-text-from-hashtable.example.out.txt"

.\replace-file-text.ps1 -settings $settings -sourceFilePath $sourceFilePath -outputFilePath $outputFilePath 