# MSBuild Organization & Repo Layout

## Directory Structure

```
BlazingSpire/
  Directory.Build.props          ← repo-wide defaults
  Directory.Build.targets        ← repo-wide targets (TFM-conditional AOT)
  Directory.Packages.props       ← Central Package Management
  Directory.Build.rsp            ← CLI flags
  BlazingSpire.sln
  src/
    Directory.Build.props        ← src-specific (imports repo-level)
    BlazingSpire.Primitives/     ← Razor Class Library (headless primitives)
    BlazingSpire.Primitives.SourceGenerators/  ← Roslyn source generators
    BlazingSpire.CLI/            ← dotnet tool
    BlazingSpire.Templates/      ← dotnet new templates
    BlazingSpire.Docs/           ← Blazor app (documentation site)
  test/
    Directory.Build.props        ← test-specific (imports repo-level)
    BlazingSpire.Primitives.Tests/       ← bUnit
    BlazingSpire.Tests.E2E/              ← Playwright
    BlazingSpire.Tests.Performance/      ← BenchmarkDotNet
```

## Root `Directory.Build.props`

```xml
<Project>
  <PropertyGroup>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
    <ArtifactsPath>$(MSBuildThisFileDirectory)artifacts</ArtifactsPath>
    <Authors>BlazingSpire Contributors</Authors>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
  </PropertyGroup>
</Project>
```

## Root `Directory.Build.targets`

```xml
<Project>
  <!-- TFM-conditional: TargetFramework is empty in .props for single-targeting projects -->
  <PropertyGroup Condition="$([MSBuild]::IsTargetFrameworkCompatible('$(TargetFramework)', 'net8.0'))">
    <IsAotCompatible>true</IsAotCompatible>
  </PropertyGroup>
</Project>
```

**Critical:** `$(TargetFramework)` is empty during `.props` evaluation for single-targeting projects. TFM-conditional properties MUST go in `.targets`.

## `src/Directory.Build.props`

```xml
<Project>
  <Import Project="$([MSBuild]::GetPathOfFileAbove('Directory.Build.props', '$(MSBuildThisFileDirectory)../'))" />

  <PropertyGroup>
    <IsPackable>true</IsPackable>
  </PropertyGroup>
</Project>
```

## `test/Directory.Build.props`

```xml
<Project>
  <Import Project="$([MSBuild]::GetPathOfFileAbove('Directory.Build.props', '$(MSBuildThisFileDirectory)../'))" />

  <PropertyGroup>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
</Project>
```

## `Directory.Packages.props` (Central Package Management)

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>

  <ItemGroup>
    <!-- Core -->
    <PackageVersion Include="TailwindMerge.NET" Version="1.3.0" />

    <!-- Testing -->
    <PackageVersion Include="bunit" Version="2.7.2" />
    <PackageVersion Include="xunit" Version="2.9.0" />
    <PackageVersion Include="Microsoft.Playwright" Version="1.58.0" />
    <PackageVersion Include="Deque.AxeCore.Playwright" Version="4.11.1" />
    <PackageVersion Include="Benchmark.Blazor" Version="1.0.0" />
    <PackageVersion Include="coverlet.collector" Version="6.0.0" />

    <!-- CLI -->
    <PackageVersion Include="Spectre.Console.Cli" Version="0.49.0" />

    <!-- Build -->
    <PackageVersion Include="Tailwind.MSBuild" Version="2.0.0" />
  </ItemGroup>

  <ItemGroup>
    <!-- Global analyzers for all projects -->
    <GlobalPackageReference Include="Microsoft.CodeAnalysis.NetAnalyzers" Version="10.0.0" />
  </ItemGroup>
</Project>
```

## `Directory.Build.rsp`

```
/maxcpucount
/nodeReuse:false
/consoleLoggerParameters:Summary;ForceNoAlign
```

## Key Decisions

- **Tailwind.MSBuild** only in `BlazingSpire.Docs` project (not global) — primitives and CLI don't need Tailwind at build time
- **Artifacts output layout** (`ArtifactsPath`) — all bin/obj/publish under single `artifacts/` folder
- **`IsAotCompatible`** in `.targets` covers both trim and AOT analyzers automatically
- **`TreatWarningsAsErrors`** globally — catch trim/AOT warnings as errors immediately
- **Central Package Management** — single source of truth for all versions
