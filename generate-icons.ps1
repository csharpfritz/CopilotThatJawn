# Set working directory to the script location
Set-Location -Path $PSScriptRoot

# Create necessary directories
$imgDir = "Web\wwwroot\img"
if (-not (Test-Path $imgDir)) {
    New-Item -ItemType Directory -Path $imgDir -Force
}

# Source image
$sourceImage = "Web\wwwroot\img\icon-with-bg.webp"

# Generate various sizes
$sizes = @(
    @{name="favicon-16x16.png"; size="16x16"},
    @{name="favicon-32x32.png"; size="32x32"},
    @{name="apple-touch-icon.png"; size="180x180"},
    @{name="android-chrome-192x192.png"; size="192x192"},
    @{name="android-chrome-512x512.png"; size="512x512"},
    @{name="mstile-150x150.png"; size="150x150"},
    @{name="maskable-icon.png"; size="512x512"}
)

# Generate each size
foreach ($icon in $sizes) {
    & "C:\Program Files\ImageMagick-7.1.1-Q16-HDRI\magick.exe" convert $sourceImage -resize $icon.size -background none "$imgDir\$($icon.name)"
}

# Generate favicon.ico (multi-size)
& "C:\Program Files\ImageMagick-7.1.1-Q16-HDRI\magick.exe" convert $sourceImage -define icon:auto-resize=16,32,48,64 "$imgDir\favicon.ico"

Write-Host "Icon generation complete!"
