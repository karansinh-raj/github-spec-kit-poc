#!/bin/bash
# preToolUse hook — logs every tool invocation to .github/hooks/logs/tool-usage.jsonl
# Input (stdin): JSON with fields: timestamp, cwd, toolName, toolArgs
# Output: none (exits 0 to allow all tools by default)

set -e

INPUT=$(cat)

TIMESTAMP=$(echo "$INPUT" | jq -r '.timestamp')
TOOL_NAME=$(echo "$INPUT"  | jq -r '.toolName')
TOOL_ARGS=$(echo "$INPUT"  | jq -r '.toolArgs')
CWD=$(echo "$INPUT"        | jq -r '.cwd')

LOG_DIR=".github/hooks/logs"
mkdir -p "$LOG_DIR"

# Append a JSON Lines entry
jq -n \
  --arg timestamp "$TIMESTAMP" \
  --arg date      "$(date -u +"%Y-%m-%dT%H:%M:%SZ")" \
  --arg tool      "$TOOL_NAME" \
  --arg args      "$TOOL_ARGS" \
  --arg cwd       "$CWD" \
  '{timestamp: $timestamp, date: $date, tool: $tool, args: $args, cwd: $cwd}' \
  >> "$LOG_DIR/tool-usage.jsonl"

exit 0
