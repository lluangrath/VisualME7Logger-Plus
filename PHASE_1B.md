# Phase 1b — SDK-Style + .NET 8 Migration (Windows / `dotnet` work)

**Why this doc exists:** Phase 1a (mechanical text edits — UTF7→UTF8, Scc strip,
ILMerge removal) was completed in the automated sandbox session. The remaining
Phase 1 work needs the `dotnet build` → fix → repeat loop, which only your
Windows machine has. This doc is the executable handoff.

**Time estimate:** 1-2 hours of focused work. Can be split into Phase 1b.1
(SDK conversion + build green) and Phase 1b.2 (rename + namespace cleanup).

---

## 0. Prerequisites

```powershell
# Verify .NET 8 SDK is installed (any version 8.0.x works)
dotnet --list-sdks
# If not installed: https://dotnet.microsoft.com/download/dotnet/8.0

# Install try-convert — Microsoft's official legacy-csproj-to-SDK-style tool
dotnet tool install -g try-convert
# (or: dotnet tool update -g try-convert if already installed)

# Pull the latest modernize branch (gets Phase 1a commits)
cd C:\Users\larry\Claude_Projects\VisualME7Logger\VisualME7Logger-Plus
git checkout modernize
git pull origin modernize  # if you've pushed; otherwise skip
```

---

## 1. SDK-Style Conversion

### 1a. Auto-convert with `try-convert`

```powershell
# Convert the entire solution at once
try-convert -w ME7Visual.sln

# OR per-project if the bulk run errors:
# try-convert -p src\VisualME7Logger.Output\VisualME7Logger.Output.csproj
# try-convert -p src\VisualME7Logger\VisualME7Logger.csproj
```

`try-convert` will:
- Replace the verbose `<Project ToolsVersion="12.0">` header with `<Project Sdk="Microsoft.NET.Sdk">`
- Drop the `<Import Project="...Microsoft.Common.props">` and `...Microsoft.CSharp.targets` imports (SDK includes them)
- Convert `<Compile Include="*.cs">` to implicit globbing
- Convert `<Reference Include="System.X" />` to implicit framework references
- Preserve your `<EmbeddedResource>` and `<Content>` items

### 1b. Set target framework to .NET 8 (Windows)

After try-convert, both csprojs will probably target `net48` or similar. Edit
each to target `net8.0-windows`:

**`src/VisualME7Logger.Output/VisualME7Logger.Output.csproj`** — top PropertyGroup should look like:
```xml
<PropertyGroup>
  <TargetFramework>net8.0-windows</TargetFramework>
  <Nullable>disable</Nullable>
  <LangVersion>latest</LangVersion>
  <RootNamespace>VisualME7Logger.Output</RootNamespace>
  <AssemblyName>VisualME7Logger.Output</AssemblyName>
</PropertyGroup>
```

**`src/VisualME7Logger/VisualME7Logger.csproj`** — top PropertyGroup should look like:
```xml
<PropertyGroup>
  <OutputType>WinExe</OutputType>
  <TargetFramework>net8.0-windows</TargetFramework>
  <UseWindowsForms>true</UseWindowsForms>
  <UseWPF>true</UseWPF>
  <Nullable>disable</Nullable>
  <LangVersion>latest</LangVersion>
  <RootNamespace>VisualME7Logger</RootNamespace>
  <AssemblyName>VisualME7Logger</AssemblyName>
  <ApplicationIcon>Icon.ico</ApplicationIcon>
  <ApplicationManifest>app.manifest</ApplicationManifest>
</PropertyGroup>
```

> **Note:** Don't rename `RootNamespace` / `AssemblyName` yet — that happens in
> step 4 below, after the project is building cleanly. Fewer moving parts at once.

---

## 2. Replace File-Referenced DLLs With NuGet PackageReferences

In `src/VisualME7Logger.Output/VisualME7Logger.Output.csproj`, find and **delete**:

```xml
<Reference Include="NCalc, Version=1.3.8.0, Culture=neutral, processorArchitecture=MSIL">
  <SpecificVersion>False</SpecificVersion>
  <HintPath>resources\NCalc.dll</HintPath>
</Reference>
```

Then add NuGet packages:

```powershell
cd src\VisualME7Logger.Output
dotnet add package NCalcSync         # modern fork of NCalc, .NET 8 compatible
cd ..\..

cd src\VisualME7Logger
# In Phase 1 we need the WinForms charting control; this is a community port
# of the legacy System.Windows.Forms.DataVisualization namespace.
dotnet add package WinForms.DataVisualization
cd ..\..
```

Once you confirm the NuGet packages are restored and build, you can delete the
bundled DLLs (which were shipped via file-reference):

```powershell
Remove-Item src\VisualME7Logger.Output\resources\NCalc.dll
Remove-Item src\VisualME7Logger.Output\resources\Antlr3.Runtime.dll
```

(Antlr3 was a transitive dep of NCalc 1.3.8 — `NCalcSync` doesn't need it.)

---

## 3. First Build + Fix Loop

```powershell
dotnet restore
dotnet build
```

**Expected breakages (fix these as they appear):**

### 3a. `BinaryFormatter` removed in .NET 8
If anything in your code or settings serializes/deserializes via
`BinaryFormatter`, it'll fail to compile. Replace with `System.Text.Json`:

```csharp
// OLD
using (var fs = File.OpenRead(path))
    return (Profile)new BinaryFormatter().Deserialize(fs);

// NEW
return JsonSerializer.Deserialize<Profile>(File.ReadAllText(path));
```

### 3b. `WindowsIdentity.GetCurrent()` / admin check
In `SettingsForm.cs` lines 52-63 there's an `#if !DEBUG` block that warns when
not running as admin. The API still exists in .NET 8 but the check is no longer
needed (the WPF rebuild won't require elevation either). **Just delete that
`#if !DEBUG ... #endif` block.**

### 3c. `WebClient` → `HttpClient`
If you see `WebClient` anywhere (it's marked obsolete), swap to `HttpClient`.
Your repo doesn't use it as far as Phase 0 audit could tell, so this may be a
no-op.

### 3d. AssemblyInfo duplicates
SDK-style projects auto-generate `AssemblyVersion`, `AssemblyTitle`, etc., so
the existing `Properties/AssemblyInfo.cs` will produce CS0579 "duplicate
attribute" errors. Either:
- Delete the file, OR
- Keep it and add `<GenerateAssemblyInfo>false</GenerateAssemblyInfo>` to the csproj

Recommend: **delete it** and let the SDK generate the metadata from csproj
properties.

### 3e. `app.config` `<supportedRuntime version="v4.0">`
The `App.config` references .NET FW 4.0. Either delete `App.config` (SDK
projects don't need it for runtime selection) or update it to:
```xml
<configuration>
  <startup>
    <supportedRuntime version="v8.0" />
  </startup>
</configuration>
```

### 3f. PolyMonControls.dll
This DLL ships in `src/VisualME7Logger/resources/controls/` but was never
actually `<Reference>`-d (Phase 0 audit confirmed). After SDK conversion the
implicit `<Content>` glob may pick it up — that's fine, it stays in the output
folder but is unused. The Phase 3 WPF rebuild deletes it entirely.

---

## 4. Smoke Test: Run the Legacy App on .NET 8

```powershell
dotnet run --project src\VisualME7Logger\VisualME7Logger.csproj
```

When the SettingsForm opens:
1. Choose Connection → "Log file" (no ECU needed)
2. Browse to `src\VisualME7Logger.Output\resources\ME7Logger\logs\allroad-config_20131016_213900.csv`
3. Hit Start → the LineGraph form should open and play back the saved Audi A6 allroad telemetry.

**This is the Phase 1 done-line:** the WinForms app from 2013 is running on .NET 8 against an existing log fixture, confirming that all the parsing and charting code survived the SDK migration.

Commit:
```powershell
git add -A
git commit -m "refactor: convert csproj files to SDK-style on .NET 8

- Both projects now target net8.0-windows
- NCalc replaced with NCalcSync NuGet (Antlr3.Runtime no longer needed)
- WinForms.DataVisualization NuGet replaces the BCL System.Windows.Forms.DataVisualization
- BinaryFormatter usage replaced with System.Text.Json
- Admin check removed
- App.config refreshed for v8.0
- AssemblyInfo.cs deleted (SDK generates from csproj)

Smoke-tested by replaying allroad-config_20131016_213900.csv in
LogFile mode — line graph renders identical to the legacy build."
```

---

## 5. Project + Namespace Rename (Phase 1b.2 — separate session OK)

This is a clean follow-up commit once the app is building and running.

### 5a. Rename project folders + assemblies

```powershell
git mv src\VisualME7Logger src\ME7Visual.WinFormsLegacy
git mv src\VisualME7Logger.Output src\ME7Visual.Core

# Rename the .csproj files inside each folder
git mv src\ME7Visual.WinFormsLegacy\VisualME7Logger.csproj src\ME7Visual.WinFormsLegacy\ME7Visual.WinFormsLegacy.csproj
git mv src\ME7Visual.Core\VisualME7Logger.Output.csproj src\ME7Visual.Core\ME7Visual.Core.csproj

# Update paths inside ME7Visual.sln (and rename project display names)
# Easiest: open ME7Visual.sln in VS 2022 — it'll prompt to "fix project paths"
# and update the GUIDs/names automatically.

# Update RootNamespace and AssemblyName in each csproj
# ME7Visual.WinFormsLegacy.csproj:
#   <RootNamespace>ME7Visual.WinFormsLegacy</RootNamespace>
#   <AssemblyName>ME7Visual.WinFormsLegacy</AssemblyName>
# ME7Visual.Core.csproj:
#   <RootNamespace>ME7Visual.Core</RootNamespace>
#   <AssemblyName>ME7Visual.Core</AssemblyName>

# Update the ProjectReference inside ME7Visual.WinFormsLegacy.csproj:
#   <ProjectReference Include="..\ME7Visual.Core\ME7Visual.Core.csproj">
```

### 5b. Namespace rename

The legacy code uses:
- `namespace VisualME7Logger.Output` (in Core) → `namespace ME7Visual.Core`
- `namespace VisualME7Logger.Log` → `namespace ME7Visual.Core.Log`
- `namespace VisualME7Logger.Session` → `namespace ME7Visual.Core.Session`
- `namespace VisualME7Logger.Common` → `namespace ME7Visual.Core.Common`
- `namespace VisualME7Logger.Configuration` → `namespace ME7Visual.WinFormsLegacy.Configuration`
- `namespace VisualME7Logger` (in WinForms) → `namespace ME7Visual.WinFormsLegacy`

**Use ReSharper, Rider, or VS 2022's "Rename namespace" refactoring** — it
updates every `using` directive across the solution at once, which sed cannot
do safely.

If doing by hand:
```powershell
# In src\ME7Visual.Core\ — replace namespace VisualME7Logger.* with ME7Visual.Core.*
# In src\ME7Visual.WinFormsLegacy\ — replace namespace VisualME7Logger.* with ME7Visual.WinFormsLegacy.*
# Then fix all `using VisualME7Logger.X` references across both projects
dotnet build  # iterate until clean
```

> **Why not sed?** String literals like
> `Path.Combine(ME7LoggerDirectory, "VisualME7LoggerOutput.txt")` and settings
> keys would also be replaced. The IDE refactoring is namespace-aware; sed is not.

Commit:
```powershell
git commit -m "refactor: rename projects and namespaces to ME7Visual.*

- src/VisualME7Logger        -> src/ME7Visual.WinFormsLegacy
- src/VisualME7Logger.Output -> src/ME7Visual.Core
- namespace VisualME7Logger.* -> ME7Visual.Core.* / ME7Visual.WinFormsLegacy.*

Note: ME7Visual.WinFormsLegacy is a temporary holding spot for the
2013 UI code; it gets deleted at the end of Phase 3 once the WPF
rebuild reaches feature parity."
```

---

## 6. Push and Open the Phase 1 Wrap-Up Issue

```powershell
git push origin modernize

# Open a GitHub Issue titled "Phase 1 complete - .NET 8 migration"
# linking to:
# - The build log showing 0 errors / 0 warnings on net8.0-windows
# - A screenshot of the LineGraph rendering the allroad replay log
# - This PHASE_1B.md as the executed plan
```

---

## What Phase 1b Unlocks

After this is merged, **Phase 2** (Core extraction + ITelemetryStream seam) is
mostly cosmetic refactoring inside `ME7Visual.Core`:
- Move types into `Streaming/`, `Session/`, `Parsing/` sub-folders
- Add the `ME7Visual.Streaming` netstandard2.0 project with `ITelemetryStream`
- First xUnit tests against the existing CSV fixtures

That's all sandbox-friendly work — I can drive Phase 2 through the file/edit
tools once you've handed me a building .NET 8 codebase.

---

## Stuck? Quick Diagnosis Tips

| Symptom | Likely cause | Fix |
|---|---|---|
| `try-convert` errors on a project | Custom MSBuild targets it can't translate | Convert by hand using the SDK template at top of this doc |
| `CS0246: type or namespace name 'XYZ' could not be found` | Missing `using` or NuGet package | Check whether the type was in `System.Windows.Forms.DataVisualization` (now NuGet-only) |
| `error MSB4019: imported project ...not found` | Stale `<Import>` left over from try-convert | Delete the offending `<Import>` line |
| `CS0579: Duplicate 'AssemblyTitle' attribute` | Old `Properties/AssemblyInfo.cs` conflicts with SDK auto-gen | Delete `Properties/AssemblyInfo.cs` |
| App throws on startup, complains about `App.config` | .NET FW runtime version reference | Replace `<supportedRuntime version="v4.0">` with `v8.0` or delete `App.config` |
| Native ME7Logger.exe not found at runtime | Working directory / path to `bin/` | Verify `Program.cs:ME7LoggerDirectory` resolves to a folder containing `ME7Logger.exe` |
