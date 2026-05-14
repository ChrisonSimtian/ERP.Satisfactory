# SatisfactoryPakExtractor

One-shot extractor that reads vanilla resource-node placements from a local
Satisfactory install and emits the JSON dataset consumed by
[`Satisfactory.Save.KnownResourceNodes`](../../src/Satisfactory/Save/KnownResourceNodes.cs).

Re-run after every game patch — Coffee Stain occasionally moves nodes.

## Run

```powershell
dotnet run --project tools/SatisfactoryPakExtractor -- `
    --paks "C:\Program Files (x86)\Steam\steamapps\common\Satisfactory\FactoryGame\Content\Paks" `
    --out  src/Satisfactory/Save/Data/known-resource-nodes.json
```

Options:
- `--paks <dir>` — pak directory (required).
- `--out <file>` — output JSON path (required).
- `--ue-version <EGame>` — override the UE5 version flag (default `GAME_UE5_3`).
- `--verbose` / `-v` — extra diagnostics.

On first run it downloads `oo2core_9_win64.dll` (Oodle decompressor) next to
the binary — needed for UE5 chunk decompression. The DLL is **not** committed.

## Output schema

See [`Data/README.md`](../../src/Satisfactory/Save/Data/README.md). Keys: `x`,
`y`, `z` (cm), `resource` (`Desc_*_C`), `purity` (`Impure`/`Normal`/`Pure`).
The extractor also writes a diagnostic `class` field (`BP_ResourceNode_C`
etc.); the loader ignores unknown properties.

## Known blocker (2026-05 / Satisfactory 1.x)

`CUE4Parse` 1.2.2 (latest on NuGet, Feb 2025) **cannot parse
`FactoryGame-Windows.utoc`'s container header** on current Satisfactory builds.
Every attempt throws:

```
CUE4Parse.UE4.Exceptions.ParserException: Invalid bool value (2362396)
at CUE4Parse.UE4.IO.Objects.FIoContainerHeaderSoftPackageReferences..ctor
at CUE4Parse.UE4.IO.Objects.FIoContainerHeader..ctor
at CUE4Parse.UE4.IO.IoStoreReader.ReadContainerHeader
```

The mismatch reproduces with every `EGame.GAME_UE5_0..GAME_UE5_6` flag —
the parser's offset is off before reaching the `SoftPackageReferences` block,
suggesting Coffee Stain's UE5 fork uses a custom container header layout
CUE4Parse doesn't yet recognize. Symptoms:

- `FactoryGame-Windows.pak` (loose pak) mounts fine — 13,592 files, audio/UI.
- `FactoryGame-Windows.utoc` (IoStore w/ directory index, all the BPs/levels)
  **fails to mount**. Stays in `UnloadedVfs`.
- `global.utoc` (no directory index — engine container) likewise unmounted.

Net result: 0 placements extracted. The extractor exits 5 and prints
`No content mounted — the IoStore container header probably failed to parse`.

### Paths forward

1. **Wait for an upstream CUE4Parse fix.** Open an issue at
   <https://github.com/FabianFG/CUE4Parse/issues> with the stack trace and a
   minimal `FactoryGame-Windows.utoc` hex dump near offset `0x240F00`. The
   project ships fixes for new UE forks frequently.
2. **Vendor `CUE4Parse` master.** The NuGet release is behind master; current
   `FIoContainerHeader.cs` may already handle this. Pull master into
   `vendor/CUE4Parse/`, reference the .csproj here, and retry. The
   redistribution licence (MIT) is compatible.
3. **Use a `.usmap` + ranked alternative.** Generate a `.usmap` mappings file
   via FModel against the same install (manual, GUI), then read assets by
   known chunk ID. Heavier integration than `CUE4Parse` 1.2.2 supports.
4. **Source from the Satisfactory wiki (CC-BY-SA).** Lower fidelity but
   redistributable. See the "Sourcing the data" section in
   [`Data/README.md`](../../src/Satisfactory/Save/Data/README.md).

Until one of the above lands the dataset stays empty — `KnownResourceNodes`
returns `Empty` and `SaveFileReader` falls back to `NodePurity.Unknown` for
mining nodes, exactly as it does today.

## Project structure

- `SatisfactoryPakExtractor.csproj` — console project, references CUE4Parse
  via NuGet (no submodule).
- `Program.cs` — mount + iterate + emit. Heavily commented.

## Constraints

- OSS only. CUE4Parse is MIT; Oodle is closed-source but distributed by Epic
  for asset access and fetched at runtime (not committed).
- Never commit the pak file or the Oodle DLL — both are large and licence-
  encumbered.
- The Steam install path is *not* hardcoded in committed source; pass it via
  `--paks`.
