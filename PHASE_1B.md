# Phase 1b — Build, Fix, and Smoke-Test on .NET 8

> **Status update (commit `7c777f6`):** the SDK-style csproj conversion has
> been committed to the `modernize` branch directly, bypassing
> `dotnet/try-convert` entirely. `try-convert` errors against the .NET 8 SDK
> with a `System.Runtime, Version=8.0.0.0` assembly-binding failure (a known
> incompatibility — see [dotnet/docs #36659](https://github.com/dotnet/docs/issues/36659)).
> Both projects now target `net8.0-windows`, NCalc is on the `NCalcSync` NuGet
> package, and the System.Windows.Forms.DataVisualization charting control is
> on the `WinForms.DataVisualization` NuGet package — all baked into the
> csproj files.
>
> **What's left for you to do:** restore packages, run a build, fix the few
> expected source-level breakages, and confirm the legacy WinForms app runs
> against a sample log file on .NET 8.

**Time estimate:** 30-90 minutes.

---

## 0. Prerequisites

```powershell
# Verify .NET 8 SDK is installed
dotnet --list-sdks
# Should show at least one 8.0.x line. If not:
# https://dotnet.microsoft.com/download/dotnet/8.0

# Pull the latest modernize branch (gets commit 7c777f6 with SDK csprojs)
cd C:\Users\larry\Claude_Projects\VisualME7Logger\VisualME7Logger-Plus
git checkout modernize
git pull origin modernize
```

> No `try-convert` install required — bypassed. The PowerShell commands in
> the previous version of this doc that involved `try-convert -w ...` are no
> longer needed.

---

## 1. Restore + First Build

```powershell
dotnet restore
dotnet build
```

`dotnet restore` should resolve:
- `NCalcSync` (latest stable — likely 3.x)
- `WinForms.DataVisualization` (latest — likely 1.9.x)

After first restore, **pin the versions** by replacing `Version="*"` with the
actual resolved version in each csproj. Find resolved versions with:

```powershell
dotnet list src\VisualME7Logger.Output\VisualME7Logger.Output.csproj package
dotnet list src\VisualME7Logger\VisualME7Logger.csproj package
```

(Floating wildcards are OK to start — but pinning produces deterministic
builds. Commit the pinned versions as a follow-up "chore: pin NuGet
versions".)

---

## 2. Expected Build Breakages and Fixes

`dotnet build` will likely surface 3-5 errors. Walk through them in order:

### 2a. `BinaryFormatter` removed in .NET 8

**Symptom:**
```
error SYSLIB0011: 'BinaryFormatter.Serialize(Stream, object)' is obsolete:
  'BinaryFormatter serialization is obsolete and should not be used.'
```
or in stricter builds:
```
error CS0246: The type or namespace name 'BinaryFormatter' could not be found.
```

**Where to look:** Any code that serializes profiles or settings to disk.
Likely in `SettingsForm.cs` around the LoadSettings / SaveSettings methods.

**Fix:** Replace with `System.Text.Json`.

```csharp
// OLD
using System.Runtime.Serialization.Formatters.Binary;
...
using (var fs = File.OpenRead(path))
    return (Profile)new BinaryFormatter().Deserialize(fs);

// NEW
using System.Text.Json;
...
return JsonSerializer.Deserialize<Profile>(File.ReadAllText(path));
```

For the writing side:
```csharp
// OLD
using (var fs = File.Create(path))
    new BinaryFormatter().Serialize(fs, profile);

// NEW
File.WriteAllText(path, JsonSerializer.Serialize(profile,
    new JsonSerializerOptions { WriteIndented = true }));
```

> **Migration note:** existing user profiles serialized as binary will no
> longer load. If you have old profiles to preserve, do one final read with
> a `BinaryFormatter` re-enable via
> `<EnableUnsafeBinaryFormatterSerialization>true</EnableUnsafeBinaryFormatterSerialization>`
> in the csproj, write them out as JSON, then remove the unsafe property.

### 2b. `WindowsIdentity.GetCurrent()` admin check

**Symptom:** Either compiles fine but produces a useless warning popup, OR
compiles fine and you can ignore it. The API still exists in .NET 8.

**Where:** `src/VisualME7Logger/SettingsForm.cs` lines 52-63 — the
`#if !DEBUG` block that pops a "you should run as admin" message.

**Fix:** Just **delete the entire `#if !DEBUG ... #endif` block.** Modern
COM-port access doesn't need elevation, and the WPF rebuild won't either.

### 2c. AssemblyInfo.cs "duplicate attribute" (only if you edit `<GenerateAssemblyInfo>`)

**Symptom (if you flip GenerateAssemblyInfo to true):**
```
error CS0579: Duplicate 'AssemblyTitle' attribute
error CS0579: Duplicate 'AssemblyVersion' attribute
```

**Why it doesn't happen by default:** the new csprojs set
`<GenerateAssemblyInfo>false</GenerateAssemblyInfo>`, so the existing
`Properties/AssemblyInfo.cs` files stay as the source of truth and there's
no duplicate.

**Fix (optional cleanup, not required):** delete `Properties/AssemblyInfo.cs`
in both projects, then remove the `<GenerateAssemblyInfo>false</GenerateAssemblyInfo>`
line from each csproj. The SDK will then generate metadata from the csproj's
`<AssemblyName>`, `<RootNamespace>`, etc.

### 2d. App.config `<supportedRuntime>`

**Symptom:** Build fine. The element is silently ignored by .NET 8.

**Where:** `src/VisualME7Logger/App.config` has:
```xml
<supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.0"/>
```

**Fix (optional):** delete `App.config` entirely (modern .NET doesn't need
runtime selection there) OR replace its body with an empty `<configuration/>`.

### 2e. `WebClient` / other obsolete APIs

**Symptom:**
```
warning SYSLIB0014: 'WebClient' is obsolete: 'WebRequest, HttpWebRequest,
  ServicePoint, and WebClient are obsolete. Use HttpClient instead.'
```

**Where:** Phase 0 audit didn't find any `WebClient` usage in the codebase,
so this likely doesn't apply. If it appears, swap to `HttpClient`.

### 2f. `Resources.Designer.cs` namespace mismatch (rare)

**Symptom:** "Could not find type 'X' in assembly 'Y'" errors related to
`Properties.Resources`.

**Fix:** Open `Properties/Resources.resx` in Visual Studio (or right-click >
Run Custom Tool: ResXFileCodeGenerator). VS regenerates `Resources.Designer.cs`
matching the SDK conventions.

---

## 3. Smoke Test: Run the Legacy App on .NET 8

```powershell
dotnet run --project src\VisualME7Logger\VisualME7Logger.csproj
```

When the SettingsForm opens:

1. Choose Connection → "Log file" (no ECU needed)
2. Browse to:
   `src\VisualME7Logger.Output\resources\ME7Logger\logs\allroad-config_20131016_213900.csv`
3. Hit Start → the LineGraph form should open and play back the saved
   Audi A6 allroad telemetry from October 2013.

**This is the Phase 1 done-line** ✅: the WinForms app from 2013 is running on
.NET 8 against an existing log fixture, confirming that all the parsing and
charting code survived the SDK migration.

If the line graph renders, commit any source-level fixes you made:

```powershell
git add -A
git commit -m "fix: address .NET 8 build breakages

- BinaryFormatter -> System.Text.Json in SettingsForm.cs
- Removed admin-check popup in SettingsForm.cs
- (other fixes as encountered)

Smoke-tested by replaying allroad-config_20131016_213900.csv in
LogFile mode -- LineGraph renders identical to the legacy build."
```

---

## 4. (Optional) Project + Namespace Rename — Phase 1b.2

Can be a separate session. Only do this once the build is green.

### 4a. Rename project folders + assemblies

```powershell
git mv src\VisualME7Logger src\ME7Visual.WinFormsLegacy
git mv src\VisualME7Logger.Output src\ME7Visual.Core

git mv src\ME7Visual.WinFormsLegacy\VisualME7Logger.csproj `
       src\ME7Visual.WinFormsLegacy\ME7Visual.WinFormsLegacy.csproj
git mv src\ME7Visual.Core\VisualME7Logger.Output.csproj `
       src\ME7Visual.Core\ME7Visual.Core.csproj
```

Then update **inside** each csproj:
```xml
<RootNamespace>ME7Visual.WinFormsLegacy</RootNamespace>
<AssemblyName>ME7Visual.WinFormsLegacy</AssemblyName>
```
(and similar for Core)

And update the `<ProjectReference>` in `ME7Visual.WinFormsLegacy.csproj`:
```xml
<ProjectReference Include="..\ME7Visual.Core\ME7Visual.Core.csproj" />
```

And update the project paths inside `ME7Visual.sln` — easiest in VS 2022:
open the .sln, it'll prompt to fix the broken project paths.

### 4b. Namespace rename (use IDE refactoring — not sed)

Legacy code uses:
- `namespace VisualME7Logger.Output` → `namespace ME7Visual.Core`
- `namespace VisualME7Logger.Log` → `namespace ME7Visual.Core.Log`
- `namespace VisualME7Logger.Session` → `namespace ME7Visual.Core.Session`
- `namespace VisualME7Logger.Common` → `namespace ME7Visual.Core.Common`
- `namespace VisualME7Logger.Configuration` → `namespace ME7Visual.WinFormsLegacy.Configuration`
- `namespace VisualME7Logger` (in WinForms) → `namespace ME7Visual.WinFormsLegacy`

**Use ReSharper / Rider / VS 2022's "Rename namespace" refactoring** — it
updates every `using` directive across the solution at once. **Don't sed**:
string literals like `Path.Combine(ME7LoggerDirectory, "VisualME7LoggerOutput.txt")`
and settings keys would also be replaced incorrectly.

`dotnet build` after each rename to catch issues early.

```powershell
git commit -m "refactor: rename projects and namespaces to ME7Visual.*

- src/VisualME7Logger        -> src/ME7Visual.WinFormsLegacy
- src/VisualME7Logger.Output -> src/ME7Visual.Core
- namespace VisualME7Logger.* -> ME7Visual.Core.* / ME7Visual.WinFormsLegacy.*

ME7Visual.WinFormsLegacy is a temporary holding spot for the 2013 UI;
gets deleted at the end of Phase 3 once the WPF rebuild reaches parity."
```

---

## 5. Push and Wrap Up

```powershell
git push origin modernize

# Open a GitHub Issue / Project card titled "Phase 1 complete - .NET 8 migration"
# Attach:
# - dotnet build output (0 errors)
# - Screenshot of LineGraph rendering the allroad replay log
```

**After this is merged, ping me and I'll drive Phase 2** — Core extraction +
ITelemetryStream seam. That's all internal-refactor work inside a building
codebase, sandbox-friendly: I can do it without dotnet because the Phase 4
test suite catches regressions later.

---

## Stuck? Quick Diagnosis

| Symptom | Likely cause | Fix |
|---|---|---|
| `NU1101: Unable to find package 'NCalcSync'` | NuGet feed not configured for nuget.org | `dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org` |
| `NU1101: Unable to find package 'WinForms.DataVisualization'` | Same | Same as above |
| `CS0246: type or namespace 'X' could not be found` after restore | Old `using` references something now in a different namespace | Add `using` for the new namespace, or check the package's docs |
| `error MSB4019: imported project ... not found` | Should not happen — SDK style doesn't import legacy props/targets | Ensure you pulled commit `7c777f6` cleanly (no merge conflicts) |
| `CS0579: Duplicate 'AssemblyTitle'` | You set `<GenerateAssemblyInfo>true</GenerateAssemblyInfo>` and kept `Properties/AssemblyInfo.cs` | Either delete `Properties/AssemblyInfo.cs` or set `<GenerateAssemblyInfo>false</GenerateAssemblyInfo>` |
| Native ME7Logger.exe not found at runtime | RealTime mode was selected; `Program.cs` resolves `ME7LoggerDirectory` to where the .exe is | Either use LogFile mode (no native binary needed) or pass the path as a command-line arg: `dotnet run -- "src\VisualME7Logger.Output\resources\ME7Logger\bin"` |
| `dotnet build` hangs forever | NuGet restore stuck on a broken cache | `dotnet nuget locals all --clear` then retry |
