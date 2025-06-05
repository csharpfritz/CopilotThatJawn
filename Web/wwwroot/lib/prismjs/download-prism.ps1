# Download comprehensive PrismJS bundle with all required languages
# This should be run to get the latest PrismJS with proper language support

$languages = @(
    "csharp",
    "javascript", 
    "typescript",
    "json",
    "powershell",
    "bash",
    "yaml",
    "xml",
    "sql",
    "markdown"
)

$plugins = @(
    "line-numbers",
    "copy-to-clipboard",
    "show-language"
)

$theme = "default"

Write-Host "To get a comprehensive PrismJS bundle:"
Write-Host "1. Go to https://prismjs.com/download.html"
Write-Host "2. Select theme: $theme"
Write-Host "3. Select languages: $($languages -join ', ')"
Write-Host "4. Select plugins: $($plugins -join ', ')"
Write-Host "5. Download and replace the current prism.min.js and prism.min.css files"
