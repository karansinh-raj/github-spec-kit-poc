#!/bin/bash
# sessionStart hook — records agent session starts to .github/hooks/logs/sessions.jsonl
# Input (stdin): JSON with fields: timestamp, cwd, source, initialPrompt
# Output: ignored by the agent runtime

set -e

INPUT=$(cat)

TIMESTAMP=$(echo "$INPUT"     | jq -r '.timestamp')
SOURCE=$(echo "$INPUT"        | jq -r '.source')
CWD=$(echo "$INPUT"           | jq -r '.cwd')
INITIAL_PROMPT=$(echo "$INPUT" | jq -r '.initialPrompt // ""')

LOG_DIR=".github/hooks/logs"
mkdir -p "$LOG_DIR"

# Append a JSON Lines entry
jq -n \
  --arg timestamp     "$TIMESTAMP" \
  --arg date          "$(date -u +"%Y-%m-%dT%H:%M:%SZ")" \
  --arg source        "$SOURCE" \
  --arg cwd           "$CWD" \
  --arg initialPrompt "$INITIAL_PROMPT" \
  '{timestamp: $timestamp, date: $date, source: $source, cwd: $cwd, initialPrompt: $initialPrompt}' \
  >> "$LOG_DIR/sessions.jsonl"

exit 0
