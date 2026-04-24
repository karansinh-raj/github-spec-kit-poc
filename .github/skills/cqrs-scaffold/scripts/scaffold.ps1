<#
.SYNOPSIS
    Scaffolds CQRS artifacts for the BookShelf project.

.DESCRIPTION
    Generates a Command (or Query) record, handler, optional validator, and unit test file
    following the established BookShelf conventions.

.PARAMETER Aggregate
    The aggregate root name, e.g. Books, ReadingLists.

.PARAMETER OperationName
    The full operation name (PascalCase), e.g. CreateBook, GetReadingListById.

.PARAMETER Type
    'command' or 'query'.

.PARAMETER ReturnDto
    The DTO type returned by the handler, e.g. BookDto.

.PARAMETER WithTest
    When set, also generates a unit test stub.

.EXAMPLE
    .\scaffold.ps1 -Aggregate Books -OperationName ArchiveBook -Type command -ReturnDto BookDto -WithTest
    .\scaffold.ps1 -Aggregate Books -OperationName GetBookByIsbn -Type query  -ReturnDto BookDto -WithTest
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory)][string] $Aggregate,
    [Parameter(Mandatory)][string] $OperationName,
    [Parameter(Mandatory)][ValidateSet('command','query')][string] $Type,
    [Parameter(Mandatory)][string] $ReturnDto,
    [switch] $WithTest
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# ── Resolve root ──────────────────────────────────────────────────────────────
$scriptDir  = $PSScriptRoot                          # .github/skills/cqrs-scaffold/scripts/
$repoRoot   = Resolve-Path (Join-Path $scriptDir '../../../../')
$appSrc     = Join-Path $repoRoot 'src\BookShelf.Application'
$testSrc    = Join-Path $repoRoot 'tests\BookShelf.Application.Tests'

# ── Derived values ────────────────────────────────────────────────────────────
$typeFolder  = if ($Type -eq 'command') { 'Commands' } else { 'Queries' }
$targetDir   = Join-Path $appSrc "$Aggregate\$typeFolder\$OperationName"
$testTypeDir = if ($Type -eq 'command') { 'Commands' } else { 'Queries' }
$testDir     = Join-Path $testSrc "$Aggregate\$testTypeDir"
$namespace   = "BookShelf.Application.$Aggregate.$typeFolder.$OperationName"
$testNs      = "BookShelf.Application.Tests.$Aggregate.$testTypeDir"
$repoIface   = "I${Aggregate}Repository"
$entity      = $Aggregate -replace 's$',''   # naive singularisation: Books → Book

# ── Helpers ───────────────────────────────────────────────────────────────────
function New-File([string]$path, [string]$content) {
    $dir = Split-Path $path
    if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir | Out-Null }
    if (Test-Path $path) {
        Write-Warning "File already exists, skipping: $path"
    } else {
        Set-Content -Path $path -Value $content -Encoding UTF8
        Write-Host "  Created: $($path.Replace($repoRoot,'').TrimStart('\/'))"
    }
}

# ── Generate Command artifacts ────────────────────────────────────────────────
if ($Type -eq 'command') {

    # Command record
    New-File (Join-Path $targetDir "${OperationName}Command.cs") @"
using BookShelf.Application.$Aggregate.DTOs;
using BookShelf.Application.Common.Models;
using MediatR;

namespace $namespace;

// TODO: Replace 'object' with the actual request DTO (e.g. ${OperationName}Request).
public record ${OperationName}Command(object Request) : IRequest<Result<$ReturnDto>>;
"@

    # Handler
    New-File (Join-Path $targetDir "${OperationName}Handler.cs") @"
using BookShelf.Application.$Aggregate.DTOs;
using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using BookShelf.Domain.Entities;
using MediatR;

namespace $namespace;

public class ${OperationName}Handler : IRequestHandler<${OperationName}Command, Result<$ReturnDto>>
{
    private readonly $repoIface _repository;

    public ${OperationName}Handler($repoIface repository)
    {
        _repository = repository;
    }

    public async Task<Result<$ReturnDto>> Handle(${OperationName}Command command, CancellationToken cancellationToken)
    {
        // TODO: Implement handler logic.
        // 1. Validate business rules
        // 2. Map request to domain entity
        // 3. Persist via _repository
        // 4. Return Result<$ReturnDto>.Success(MapToDto(entity))
        throw new NotImplementedException();
    }

    private static $ReturnDto MapToDto($entity entity) =>
        throw new NotImplementedException(); // TODO: map entity properties to DTO
}
"@

    # Validator
    New-File (Join-Path $targetDir "${OperationName}Validator.cs") @"
using FluentValidation;

namespace $namespace;

public class ${OperationName}Validator : AbstractValidator<${OperationName}Command>
{
    public ${OperationName}Validator()
    {
        // TODO: Add validation rules.
        // Example:
        // RuleFor(x => x.Request.Title)
        //     .NotEmpty().WithMessage("Title is required.")
        //     .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");
    }
}
"@
}

# ── Generate Query artifacts ───────────────────────────────────────────────────
if ($Type -eq 'query') {

    # Query record
    New-File (Join-Path $targetDir "${OperationName}Query.cs") @"
using BookShelf.Application.$Aggregate.DTOs;
using BookShelf.Application.Common.Models;
using MediatR;

namespace $namespace;

// TODO: Replace 'int Id' with the actual query parameter(s).
public record ${OperationName}Query(int Id) : IRequest<Result<$ReturnDto>>;
"@

    # Handler
    New-File (Join-Path $targetDir "${OperationName}Handler.cs") @"
using BookShelf.Application.$Aggregate.DTOs;
using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using MediatR;

namespace $namespace;

public class ${OperationName}Handler : IRequestHandler<${OperationName}Query, Result<$ReturnDto>>
{
    private readonly $repoIface _repository;

    public ${OperationName}Handler($repoIface repository)
    {
        _repository = repository;
    }

    public async Task<Result<$ReturnDto>> Handle(${OperationName}Query query, CancellationToken cancellationToken)
    {
        // TODO: Implement query logic.
        // 1. Fetch entity from _repository using query.Id (or other params)
        // 2. Return Result<$ReturnDto>.Failure("…not found.") if null
        // 3. Return Result<$ReturnDto>.Success(MapToDto(entity))
        throw new NotImplementedException();
    }

    private static $ReturnDto MapToDto($entity entity) =>
        throw new NotImplementedException(); // TODO: map entity properties to DTO
}
"@
}

# ── Generate Test stub ────────────────────────────────────────────────────────
if ($WithTest) {
    $testFile = Join-Path $testDir "${OperationName}HandlerTests.cs"

    $recordType    = "${OperationName}$(if ($Type -eq 'command') {'Command'} else {'Query'})"
    $handlerType   = "${OperationName}Handler"

    New-File $testFile @"
using BookShelf.Application.$Aggregate.$(if ($Type -eq 'command') {'Commands'} else {'Queries'}).$OperationName;
using BookShelf.Application.$Aggregate.DTOs;
using BookShelf.Application.Common.Interfaces;
using BookShelf.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace $testNs;

public class ${OperationName}HandlerTests
{
    private readonly $repoIface _repository;
    private readonly $handlerType _handler;

    public ${OperationName}HandlerTests()
    {
        _repository = Substitute.For<$repoIface>();
        _handler = new $handlerType(_repository);
    }

    [Fact]
    public async Task Handle_ValidInput_ReturnsSuccess()
    {
        // Arrange
        // TODO: Set up mock returns and build the ${recordType}.

        // Act
        // var result = await _handler.Handle(new ${recordType}(/* params */), CancellationToken.None);

        // Assert
        // result.IsSuccess.Should().BeTrue();
        throw new NotImplementedException("TODO: implement test");
    }

    [Fact]
    public async Task Handle_InvalidInput_ReturnsFailure()
    {
        // Arrange
        // TODO: Set up mock to simulate failure condition.

        // Act
        // var result = await _handler.Handle(new ${recordType}(/* params */), CancellationToken.None);

        // Assert
        // result.IsSuccess.Should().BeFalse();
        // result.Errors.Should().ContainSingle().Which.Should().Contain("expected message");
        throw new NotImplementedException("TODO: implement test");
    }
}
"@
}

Write-Host ""
Write-Host "Scaffold complete for '$OperationName' ($Type)."
Write-Host "Next steps:"
Write-Host "  1. Fill in the TODO comments in each generated file."
Write-Host "  2. Add the endpoint to src/BookShelf.API/Endpoints/${Aggregate}Endpoints.cs."
Write-Host "  3. Run: dotnet build BookShelf.slnx"
Write-Host "  4. Run: dotnet test BookShelf.slnx"
