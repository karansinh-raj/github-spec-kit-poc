# preToolUse hook — logs every tool invocation to .github/hooks/logs/tool-usage.jsonl
# Input (stdin): JSON with fields: timestamp, cwd, toolName, toolArgs
# Output: none (exits 0 to allow all tools by default)

$ErrorActionPreference = "Stop"

try {
    $inputData = [Console]::In.ReadToEnd() | ConvertFrom-Json

    $logDir = ".github/hooks/logs"
    if (-not (Test-Path $logDir)) {
        New-Item -ItemType Directory -Path $logDir -Force | Out-Null
    }

    $logEntry = [ordered]@{
        timestamp = $inputData.timestamp
        date      = (Get-Date -Format "yyyy-MM-ddTHH:mm:ssZ")
        tool      = $inputData.toolName
        args      = $inputData.toolArgs
        cwd       = $inputData.cwd
    } | ConvertTo-Json -Compress

    Add-Content -Path "$logDir/tool-usage.jsonl" -Value $logEntry

    exit 0
} catch {
    # Never block tool execution due to a logging error
    exit 0
}
