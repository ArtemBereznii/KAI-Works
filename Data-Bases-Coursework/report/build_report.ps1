# Збирає пояснювальну записку (DOCX + PDF) з текстової розмітки через Microsoft Word (COM).
# Запуск: powershell -ExecutionPolicy Bypass -File report\build_report.ps1 [-Source report\zapyska.txt] [-Out report\Пояснювальна_записка]
#
# Розмітка (кожен блок — окремий рядок):
#   # Заголовок               розділ (Heading 1, з нової сторінки, у змісті)
#   #! Заголовок              розділ без нової сторінки
#   ## Заголовок              підрозділ (Heading 2)
#   - текст                   пункт переліку («– текст»)
#   TABLE: Таблиця 1.1 – …    назва таблиці; далі рядки «| a | b |», перший — шапка; кінець — порожній рядок
#   WIDTHS: 20,30,50          (необов'язково, одразу після TABLE:) ширини стовпців у відсотках
#   IMG: шлях | Рисунок 2.1 – …   рисунок на ширину сторінки з підписом
#   CODE: / ENDCODE           лістинг моноширинним шрифтом (відступи зберігаються)
#   CENTER: / BCENTER: / RIGHT: / LEFT:   вирівняний рядок без абзацного відступу (BCENTER — жирний)
#   GAP: n                    n порожніх рядків
#   PAGEBREAK                 розрив сторінки
#   TOC                       автоматичний зміст
#   LANDSCAPE / PORTRAIT      нова сторінка в альбомній / книжковій орієнтації
#   **Жирний початок.** текст абзацу    жирний фрагмент на початку абзацу
#   // коментар               ігнорується
# Порожні рядки між абзацами ігноруються.

param(
    [string]$Source = "$PSScriptRoot\zapyska.txt",
    [string]$Out    = "$PSScriptRoot\Пояснювальна_записка"
)
$ErrorActionPreference = 'Stop'

$cm = 28.3465
$lines = [System.IO.File]::ReadAllLines((Resolve-Path $Source), [System.Text.Encoding]::UTF8)
$baseDir = Split-Path (Resolve-Path $Source) -Parent

# Коли Word зайнятий (запуск, вікно активації), він відхиляє COM-виклики з
# RPC_E_CALL_REJECTED. Фільтр повідомлень повторює такий виклик до 2 хвилин.
Add-Type -TypeDefinition @"
using System;
using System.Runtime.InteropServices;
[ComImport, Guid("00000016-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IOleMessageFilter {
    [PreserveSig] int HandleInComingCall(int callType, IntPtr caller, int tick, IntPtr info);
    [PreserveSig] int RetryRejectedCall(IntPtr callee, int tick, int rejectType);
    [PreserveSig] int MessagePending(IntPtr callee, int tick, int pendingType);
}
public class ComRetryFilter : IOleMessageFilter {
    [DllImport("Ole32.dll")]
    private static extern int CoRegisterMessageFilter(IOleMessageFilter newFilter, out IOleMessageFilter oldFilter);
    public static void Register() { IOleMessageFilter old; CoRegisterMessageFilter(new ComRetryFilter(), out old); }
    public static void Revoke() { IOleMessageFilter old; CoRegisterMessageFilter(null, out old); }
    int IOleMessageFilter.HandleInComingCall(int callType, IntPtr caller, int tick, IntPtr info) { return 0; }
    int IOleMessageFilter.RetryRejectedCall(IntPtr callee, int tick, int rejectType) { return (rejectType == 2 && tick < 120000) ? 250 : -1; }
    int IOleMessageFilter.MessagePending(IntPtr callee, int tick, int pendingType) { return 2; }
}
"@
[ComRetryFilter]::Register()

$word = New-Object -ComObject Word.Application
$word.Visible = $false
$word.ScreenUpdating = $false
$word.Options.CheckSpellingAsYouType = $false
$word.Options.CheckGrammarAsYouType = $false
$doc = $word.Documents.Add()

try {
    # Сторінка
    $ps = $doc.PageSetup
    $ps.PaperSize = 7          # A4
    $ps.LeftMargin = 2.5 * $cm; $ps.RightMargin = 1.5 * $cm
    $ps.TopMargin = 2.0 * $cm;  $ps.BottomMargin = 2.0 * $cm
    $ps.HeaderDistance = 1.0 * $cm

    # Стилі
    function Set-Font($style, $name, $size, $bold) {
        $style.Font.Name = $name; $style.Font.Size = $size; $style.Font.Bold = [int]$bold
        $style.Font.Color = -16777216; $style.Font.Italic = 0
    }
    $normal = $doc.Styles.Item(-1)
    Set-Font $normal 'Times New Roman' 14 $false
    $normal.LanguageID = 1058   # українська
    $pf = $normal.ParagraphFormat
    $pf.LineSpacingRule = 1; $pf.Alignment = 3; $pf.FirstLineIndent = 1.25 * $cm
    $pf.SpaceBefore = 0; $pf.SpaceAfter = 0; $pf.WidowControl = -1

    $h1 = $doc.Styles.Item(-2)
    Set-Font $h1 'Times New Roman' 14 $true
    $h1.ParagraphFormat.Alignment = 1; $h1.ParagraphFormat.FirstLineIndent = 0; $h1.ParagraphFormat.LeftIndent = 0
    $h1.ParagraphFormat.SpaceBefore = 0; $h1.ParagraphFormat.SpaceAfter = 12
    $h1.ParagraphFormat.LineSpacingRule = 1; $h1.ParagraphFormat.KeepWithNext = -1
    $h1.ParagraphFormat.PageBreakBefore = 0

    $h2 = $doc.Styles.Item(-3)
    Set-Font $h2 'Times New Roman' 14 $true
    $h2.ParagraphFormat.Alignment = 0; $h2.ParagraphFormat.FirstLineIndent = 1.25 * $cm; $h2.ParagraphFormat.LeftIndent = 0
    $h2.ParagraphFormat.SpaceBefore = 12; $h2.ParagraphFormat.SpaceAfter = 6
    $h2.ParagraphFormat.LineSpacingRule = 1; $h2.ParagraphFormat.KeepWithNext = -1

    # Стилі змісту (wdStyleTOC1 = -20, wdStyleTOC2 = -21)
    $toc1 = $doc.Styles.Item(-20); Set-Font $toc1 'Times New Roman' 14 $false
    $toc1.ParagraphFormat.FirstLineIndent = 0; $toc1.ParagraphFormat.LeftIndent = 0; $toc1.ParagraphFormat.SpaceAfter = 0
    $toc2 = $doc.Styles.Item(-21); Set-Font $toc2 'Times New Roman' 14 $false
    $toc2.ParagraphFormat.FirstLineIndent = 0; $toc2.ParagraphFormat.LeftIndent = 0.75 * $cm; $toc2.ParagraphFormat.SpaceAfter = 0

    function New-ParaStyle($name, $font, $size, $align, $spacingRule, $indent) {
        try { $s = $doc.Styles.Item($name) } catch { $s = $doc.Styles.Add($name, 1) }
        $s.BaseStyle = -1
        Set-Font $s $font $size $false
        $s.ParagraphFormat.Alignment = $align
        $s.ParagraphFormat.FirstLineIndent = [single]$indent
        $s.ParagraphFormat.LeftIndent = [single]0
        $s.ParagraphFormat.LineSpacingRule = $spacingRule
        $s.ParagraphFormat.SpaceBefore = 0; $s.ParagraphFormat.SpaceAfter = 0
        return $s
    }
    $sCode    = New-ParaStyle 'Лістинг'        'Courier New'     10 0 0 0
    $sTable   = New-ParaStyle 'Текст таблиці'  'Times New Roman' 12 0 0 0
    $sCapTab  = New-ParaStyle 'Назва таблиці'  'Times New Roman' 14 0 1 0
    $sCapTab.ParagraphFormat.KeepWithNext = -1; $sCapTab.ParagraphFormat.SpaceBefore = 6
    $sCapFig  = New-ParaStyle 'Підпис рисунка' 'Times New Roman' 14 1 1 0
    $sCapFig.ParagraphFormat.SpaceAfter = 6
    $sFig     = New-ParaStyle 'Рисунок'        'Times New Roman' 14 1 0 0
    $sFig.ParagraphFormat.KeepWithNext = -1; $sFig.ParagraphFormat.SpaceBefore = 6
    $sPlain   = New-ParaStyle 'Без відступу'   'Times New Roman' 14 1 1 0

    # Номер сторінки праворуч угорі, на титульному не друкується
    $sec = $doc.Sections.Item(1)
    $sec.PageSetup.DifferentFirstPageHeaderFooter = -1
    $null = $sec.Headers.Item(1).PageNumbers.Add(2, $false)
    $sec.Headers.Item(1).Range.Font.Name = 'Times New Roman'
    $sec.Headers.Item(1).Range.Font.Size = 14

    $sel = $word.Selection

    function Add-Para([string]$text, $style, [int]$align = -99, [bool]$bold = $false) {
        $sel.Style = $style
        if ($align -ne -99) { $sel.ParagraphFormat.Alignment = $align }
        if ($text -match '^(– )?\*\*(.+?)\*\*(.*)$') {
            if ($Matches[1]) { $sel.TypeText($Matches[1]) }
            $sel.Font.Bold = 1; $sel.TypeText($Matches[2]); $sel.Font.Bold = 0
            $sel.TypeText($Matches[3])
        } else {
            if ($bold) { $sel.Font.Bold = 1 }
            $sel.TypeText($text)
            if ($bold) { $sel.Font.Bold = 0 }
        }
        $sel.TypeParagraph()
    }

    function Get-PageWidth() {
        $p = $sel.Sections.Item(1).PageSetup
        return $p.PageWidth - $p.LeftMargin - $p.RightMargin
    }

    $i = 0
    $n = $lines.Count
    while ($i -lt $n) {
        $line = $lines[$i].TrimEnd()
        if ($line -eq '' -or $line.StartsWith('//')) { $i++; continue }

        if ($line -eq 'TOC') {
            $r = $sel.Range
            $null = $doc.TablesOfContents.Add($r, $true, 1, 2)
            $sel.EndKey(6) | Out-Null
            $sel.TypeParagraph()
            $i++; continue
        }
        if ($line -eq 'PAGEBREAK') { $sel.InsertBreak(7); $i++; continue }
        if ($line -eq 'LANDSCAPE' -or $line -eq 'PORTRAIT') {
            $sel.InsertBreak(2)   # розрив розділу з нової сторінки
            $nps = $sel.Sections.Item(1).PageSetup
            $nps.Orientation = $(if ($line -eq 'LANDSCAPE') { 1 } else { 0 })
            $nps.DifferentFirstPageHeaderFooter = 0   # номер друкується й на першій сторінці нового розділу
            $i++; continue
        }
        if ($line -match '^GAP:\s*(\d+)') {
            for ($k = 0; $k -lt [int]$Matches[1]; $k++) { Add-Para '' $sPlain }
            $i++; continue
        }
        if ($line -match '^#!\s+(.*)$') { Add-Para $Matches[1] $h1; $i++; continue }
        if ($line -match '^#\s+(.*)$') {
            if ($sel.Start -gt 0) { $sel.InsertBreak(7) }
            Add-Para $Matches[1] $h1; $i++; continue
        }
        if ($line -match '^##\s+(.*)$') { Add-Para $Matches[1] $h2; $i++; continue }
        if ($line -match '^BCENTER:\s?(.*)$') { Add-Para $Matches[1] $sPlain 1 $true; $i++; continue }
        if ($line -match '^CENTER:\s?(.*)$') { Add-Para $Matches[1] $sPlain 1; $i++; continue }
        if ($line -match '^RIGHT:\s?(.*)$')  { Add-Para $Matches[1] $sPlain 2; $i++; continue }
        if ($line -match '^LEFT:\s?(.*)$')   { Add-Para $Matches[1] $sPlain 0; $i++; continue }
        if ($line -match '^-\s+(.*)$') { Add-Para ("– " + $Matches[1]) $normal; $i++; continue }

        if ($line -eq 'CODE:') {
            $i++
            $codeLines = New-Object System.Collections.Generic.List[string]
            while ($i -lt $n -and $lines[$i].TrimEnd() -ne 'ENDCODE') { $codeLines.Add($lines[$i].TrimEnd()); $i++ }
            # короткий лістинг не розриваємо між сторінками; довгий — переноситься вільно
            $keep = $codeLines.Count -le 25
            for ($c = 0; $c -lt $codeLines.Count; $c++) {
                $sel.Style = $sCode
                $sel.ParagraphFormat.KeepWithNext = $(if ($keep -and $c -lt $codeLines.Count - 1) { -1 } else { 0 })
                if ($c -eq 0) { $sel.ParagraphFormat.SpaceBefore = 6 }
                if ($c -eq $codeLines.Count - 1) { $sel.ParagraphFormat.SpaceAfter = 6 }
                $sel.TypeText($codeLines[$c])
                $sel.TypeParagraph()
            }
            $sel.Style = $sPlain
            $sel.ParagraphFormat.SpaceAfter = 0
            $i++; continue
        }

        if ($line -match '^IMG:\s*(.+?)\s*\|\s*([^|]+?)\s*(\|\s*(\d+))?\s*$') {
            $path = $(if ([System.IO.Path]::IsPathRooted($Matches[1])) { $Matches[1] } else { Join-Path $baseDir $Matches[1] })
            $caption = $Matches[2]
            $pct = $(if ($Matches[4]) { [double]$Matches[4] } else { 100.0 })   # максимум висоти, % від доступної
            $sel.Style = $sFig
            $shape = $sel.InlineShapes.AddPicture((Resolve-Path $path).Path, $false, $true)
            $p = $sel.Sections.Item(1).PageSetup
            $maxW = (Get-PageWidth) * [math]::Min(1.0, $pct / 100.0)
            $maxH = ($p.PageHeight - $p.TopMargin - $p.BottomMargin - 2.5 * $cm) * $pct / 100.0
            $ratio = $shape.Height / $shape.Width
            $w = $maxW; if ($w * $ratio -gt $maxH) { $w = $maxH / $ratio }
            $shape.Width = $w; $shape.Height = $w * $ratio
            $sel.TypeParagraph()
            Add-Para $caption $sCapFig
            $i++; continue
        }

        if ($line -match '^TABLE:\s*(.+)$') {
            $caption = $Matches[1]
            $i++
            $widths = $null
            if ($i -lt $n -and $lines[$i] -match '^WIDTHS:\s*(.+)$') {
                $widths = $Matches[1].Split(',') | ForEach-Object { [double]$_.Trim() }
                $i++
            }
            $rows = New-Object System.Collections.Generic.List[string[]]
            while ($i -lt $n -and $lines[$i].Trim().StartsWith('|')) {
                $raw = $lines[$i].Trim()
                $cells = $raw.Substring(1, $raw.Length - 2).Split('|') | ForEach-Object { $_.Trim() }
                $rows.Add([string[]]$cells)
                $i++
            }
            Add-Para $caption $sCapTab
            $cols = $rows[0].Count
            $start = $sel.Start
            $sel.Style = $sTable
            $text = ($rows | ForEach-Object { ($_ -join "`t") }) -join "`r"
            $sel.TypeText($text)
            $r = $doc.Range($start, $sel.End)
            $t = $r.ConvertToTable(1, $rows.Count, $cols)
            $t.Borders.Enable = 1
            $t.Range.Style = $sTable
            $t.TopPadding = 1; $t.BottomPadding = 1
            $t.AllowAutoFit = $false
            $t.Rows.Item(1).HeadingFormat = -1
            $t.Rows.Item(1).Range.Font.Bold = 1
            $t.Rows.AllowBreakAcrossPages = -1
            $tw = Get-PageWidth
            if ($null -eq $widths) { $widths = @(1..$cols | ForEach-Object { 100.0 / $cols }) }
            for ($c = 1; $c -le $cols; $c++) {
                $t.Columns.Item($c).PreferredWidthType = 3   # пункти
                $t.Columns.Item($c).PreferredWidth = $tw * $widths[$c - 1] / 100.0
            }
            $sel.EndKey(6) | Out-Null
            Add-Para '' $sPlain
            continue
        }

        Add-Para $line $normal
        $i++
    }

    # Зміст і поля оновлюються після вставлення всього тексту — у режимі розмітки сторінки,
    # бо саме ним Word розбиває сторінки під час експорту в PDF.
    # Word лишається прихованим: у видимому вікні неактивований Office переходить в обмежений
    # режим. Остаточні номери сторінок у змісті ставляться нижче, у перевідкритому файлі.
    $word.ScreenUpdating = $true
    $doc.ActiveWindow.View.Type = 3
    $doc.Repaginate()
    foreach ($toc in $doc.TablesOfContents) { $toc.Update() }
    $doc.Fields.Update() | Out-Null
    $doc.Repaginate()
    foreach ($toc in $doc.TablesOfContents) { $toc.UpdatePageNumbers() }

    $outDocx = [System.IO.Path]::GetFullPath("$Out.docx")
    $outPdf  = [System.IO.Path]::GetFullPath("$Out.pdf")
    $doc.SaveAs2($outDocx, 16)
    $doc.Close(0)

    # Щойно створений документ Word розбиває на сторінки інакше, ніж той самий файл,
    # відкритий повторно. Тому зміст остаточно оновлюється в перевідкритому DOCX,
    # і PDF експортується з нього ж — номери в змісті DOCX і PDF збігаються.
    $doc = $word.Documents.Open($outDocx)
    $doc.ActiveWindow.View.Type = 3
    $doc.Repaginate()
    foreach ($toc in $doc.TablesOfContents) { $toc.Update() }
    $doc.Repaginate()
    foreach ($toc in $doc.TablesOfContents) { $toc.UpdatePageNumbers() }
    $doc.Save()
    $doc.ExportAsFixedFormat($outPdf, 17)
    Write-Output ("Сторінок: " + $doc.ComputeStatistics(2))
    Write-Output "DOCX: $outDocx"
    Write-Output "PDF:  $outPdf"
}
finally {
    $doc.Close(0)
    $word.Quit()
    [System.Runtime.InteropServices.Marshal]::ReleaseComObject($word) | Out-Null
}
