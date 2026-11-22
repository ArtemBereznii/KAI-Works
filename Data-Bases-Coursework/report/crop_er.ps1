# Вирізає з docs/er.png два фрагменти для записки (методичка: для великої моделі —
# загальна схема та окремі фрагменти). Межі задано в пікселях вихідного зображення 4800×3400.
# Запуск: powershell -ExecutionPolicy Bypass -File report\crop_er.ps1
Add-Type -AssemblyName System.Drawing
$docs = Join-Path $PSScriptRoot '..\docs'
$src = [System.Drawing.Image]::FromFile((Resolve-Path "$docs\er.png").Path)

function Save-Crop($x, $y, $w, $h, $name) {
    $rect = New-Object System.Drawing.Rectangle($x, $y, $w, $h)
    $bmp = New-Object System.Drawing.Bitmap($w, $h)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.Clear([System.Drawing.Color]::White)
    $g.DrawImage($src, (New-Object System.Drawing.Rectangle(0, 0, $w, $h)), $rect, [System.Drawing.GraphicsUnit]::Pixel)
    $bmp.Save((Join-Path $docs $name), [System.Drawing.Imaging.ImageFormat]::Png)
    $g.Dispose(); $bmp.Dispose()
    "$name ${w}x${h}"
}

try {
    "source $($src.Width)x$($src.Height)"
    # Фрагмент 1: пільги, пасажири, картка, види транспорту, тарифи, тарифні плани
    Save-Crop 0 0 3400 1640 'er-part1.png'
    # Фрагмент 2: картка та операції — історія, поповнення, валідатори, проїзні, поїздки, списання
    Save-Crop 0 1180 3400 2220 'er-part2.png'
}
finally { $src.Dispose() }
