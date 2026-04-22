# sessionStart hook — records agent session starts to .github/hooks/logs/sessions.jsonl
# Input (stdin): JSON with fields: timestamp, cwd, source, initialPrompt
# Output: ignored by the agent runtime

$ErrorActionPreference = "Stop"

try {
    $inputData = [Console]::In.ReadToEnd() | ConvertFrom-Json

    $logDir = ".github/hooks/logs"
    if (-not (Test-Path $logDir)) {
        New-Item -ItemType Directory -Path $logDir -Force | Out-Null
    }

    $logEntry = [ordered]@{
        timestamp     = $inputData.timestamp
        date          = (Get-Date -Format "yyyy-MM-ddTHH:mm:ssZ")
        source        = $inputData.source
        cwd           = $inputData.cwd
        initialPrompt = if ($null -ne $inputData.initialPrompt) { $inputData.initialPrompt } else { "" }
    } | ConvertTo-Json -Compress

    Add-Content -Path "$logDir/sessions.jsonl" -Value $logEntry

    exit 0
} catch {
    # Never fail silently in a way that blocks the session
    exit 0
}
