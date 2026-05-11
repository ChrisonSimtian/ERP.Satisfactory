#!/usr/bin/env node
// Stocktake sync: parses the latest Satisfactory save and emits a
// markdown summary shaped to compare against .satisfactory/stocktake.md.
// Usage:
//   node stocktake.mjs                       # auto-detect latest save
//   node stocktake.mjs <path/to/save.sav>    # explicit save path
//   node stocktake.mjs --json [path]         # raw JSON instead of markdown

import { Parser } from '@etothepii/satisfactory-file-parser';
import { readFileSync, readdirSync, statSync } from 'fs';
import { join } from 'path';

const DEFAULT_SAVE_DIR = process.env.ERP_SATISFACTORY_SAVE_DIR
  ?? 'C:/Users/ChrisSimon/AppData/Local/FactoryGame/Saved/SaveGames/76561198103946376';

const args = process.argv.slice(2);
const wantJson = args.includes('--json');
const positional = args.filter(a => a !== '--json');
const savePath = positional[0] ?? findLatestSave(DEFAULT_SAVE_DIR);

if (!savePath) {
  console.error(`No .sav files found under ${DEFAULT_SAVE_DIR}`);
  process.exit(1);
}

const saveStat = statSync(savePath);
const nodeBuf = readFileSync(savePath);
const buf = nodeBuf.buffer.slice(nodeBuf.byteOffset, nodeBuf.byteOffset + nodeBuf.byteLength);
const t0 = Date.now();
const save = Parser.ParseSave('stocktake', buf, undefined, { onProgressCallback: () => {} });
const parseMs = Date.now() - t0;

const allObjects = [];
for (const level of Object.values(save.levels)) {
  for (const obj of level.objects ?? []) allObjects.push(obj);
}

// Build instanceName → object map so we can resolve mExtractableResource refs.
const byInstance = new Map();
for (const o of allObjects) byInstance.set(o.instanceName, o);

const short = tp => (tp ?? '').split('.').pop() ?? '';
const shortRecipe = tp => short(tp).replace(/^Recipe_/, '').replace(/_C$/, '');
const shortFuel = tp => short(tp).replace(/^Desc_/, '').replace(/_C$/, '');
const shortBuildable = tp => short(tp).replace(/^Build_/, '').replace(/_C$/, '');
const shortResource = tp => short(tp).replace(/^Desc_/, '').replace(/^BP_ResourceNode/, 'Node').replace(/_C$/, '');

// Strip the `_C` suffix from class typepaths for consistent grouping.
const className = obj => shortBuildable(obj.typePath);

const groups = {
  miners: [],
  smelters: [],
  constructors: [],
  assemblers: [],
  foundries: [],
  refineries: [],
  manufacturers: [],
  packagers: [],
  blenders: [],
  particleAccelerators: [],
  coalGenerators: [],
  fuelGenerators: [],
  biomassBurners: [],
  geothermalGenerators: [],
  nuclearGenerators: [],
  waterExtractors: [],
  oilExtractors: [],
  resourceWellExtractors: [],
  beltsByTier: new Map(),
  liftsByTier: new Map(),
  pipesByTier: new Map(),
};

for (const obj of allObjects) {
  const tp = obj.typePath ?? '';
  if (!tp) continue;
  const cls = className(obj);

  if (cls.startsWith('MinerMk')) {
    groups.miners.push(obj);
  } else if (cls.startsWith('SmelterMk') || cls.startsWith('Smelter_')) {
    groups.smelters.push(obj);
  } else if (cls.startsWith('FoundryMk') || cls === 'Foundry') {
    groups.foundries.push(obj);
  } else if (cls.startsWith('ConstructorMk') || cls === 'Constructor') {
    groups.constructors.push(obj);
  } else if (cls.startsWith('AssemblerMk') || cls === 'Assembler') {
    groups.assemblers.push(obj);
  } else if (cls.startsWith('ManufacturerMk') || cls === 'Manufacturer') {
    groups.manufacturers.push(obj);
  } else if (cls.includes('OilRefinery') || cls === 'Refinery') {
    groups.refineries.push(obj);
  } else if (cls.includes('Packager')) {
    groups.packagers.push(obj);
  } else if (cls.includes('Blender')) {
    groups.blenders.push(obj);
  } else if (cls.includes('HadronCollider') || cls.includes('ParticleAccelerator')) {
    groups.particleAccelerators.push(obj);
  } else if (cls.includes('GeneratorCoal')) {
    groups.coalGenerators.push(obj);
  } else if (cls.includes('GeneratorFuel')) {
    groups.fuelGenerators.push(obj);
  } else if (cls.includes('GeneratorBiomass') || cls.includes('GeneratorBio')) {
    groups.biomassBurners.push(obj);
  } else if (cls.includes('GeneratorGeoThermal') || cls.includes('GeneratorGeothermal')) {
    groups.geothermalGenerators.push(obj);
  } else if (cls.includes('GeneratorNuclear')) {
    groups.nuclearGenerators.push(obj);
  } else if (cls === 'WaterPump' || cls.startsWith('WaterPump')) {
    groups.waterExtractors.push(obj);
  } else if (cls.includes('OilPump') || cls.includes('CrudeOilPump')) {
    groups.oilExtractors.push(obj);
  } else if (cls.includes('FrackingExtractor') || cls.includes('FrackingSmasher')) {
    groups.resourceWellExtractors.push(obj);
  } else if (cls.startsWith('ConveyorBeltMk')) {
    const tier = cls.replace('ConveyorBeltMk', 'Mk');
    groups.beltsByTier.set(tier, (groups.beltsByTier.get(tier) ?? 0) + 1);
  } else if (cls.startsWith('ConveyorLiftMk')) {
    const tier = cls.replace('ConveyorLiftMk', 'Mk');
    groups.liftsByTier.set(tier, (groups.liftsByTier.get(tier) ?? 0) + 1);
  } else if (cls.startsWith('PipelineMk')) {
    const tier = cls.replace('PipelineMk', 'Mk');
    groups.pipesByTier.set(tier, (groups.pipesByTier.get(tier) ?? 0) + 1);
  }
}

function getProp(obj, name) {
  const p = obj.properties?.[name];
  if (!p) return undefined;
  return Array.isArray(p) ? p[0]?.value : p.value;
}

function resolveRecipe(obj) {
  const r = getProp(obj, 'mCurrentRecipe');
  return r?.pathName ? shortRecipe(r.pathName) : '(none)';
}

function resolveExtractable(obj) {
  const r = getProp(obj, 'mExtractableResource');
  const ref = r?.pathName ? r.pathName.split('.').pop() : '(none)';

  // The static resource node object doesn't carry the resource class — the
  // game keeps that in static world data. But the miner's own OutputInventory
  // child component carries `mAllowedItemDescriptors` locked to the mined
  // resource (e.g. Desc_OreIron_C). Use that for the resource label.
  const outInv = byInstance.get(`${obj.instanceName}.OutputInventory`);
  const allowed = outInv?.properties?.mAllowedItemDescriptors;
  const arr = Array.isArray(allowed) ? allowed[0] : allowed;
  const first = arr?.values?.[0] ?? arr?.value?.[0];
  const resource = first?.pathName ? shortResource(first.pathName) : '(unknown)';

  return { ref, resource };
}

function resolveFuel(obj) {
  const f = getProp(obj, 'mCurrentFuelClass');
  return f?.pathName ? shortFuel(f.pathName) : '(none)';
}

function tally(items, keyFn) {
  const m = new Map();
  for (const it of items) {
    const k = keyFn(it);
    m.set(k, (m.get(k) ?? 0) + 1);
  }
  return [...m.entries()].sort((a, b) => b[1] - a[1]);
}

function pos(obj) {
  const t = obj.transform?.translation;
  return t ? `(${(t.x ?? 0).toFixed(0)}, ${(t.y ?? 0).toFixed(0)}, ${(t.z ?? 0).toFixed(0)})` : '(?, ?, ?)';
}

// ---------- Output ----------

if (wantJson) {
  const out = {
    save: {
      path: savePath,
      lastWriteIso: saveStat.mtime.toISOString(),
      sessionName: save.header.sessionName,
      saveVersion: save.header.saveVersion,
      buildVersion: save.header.buildVersion,
      parseMs,
    },
    totals: {
      miners: groups.miners.length,
      smelters: groups.smelters.length,
      constructors: groups.constructors.length,
      assemblers: groups.assemblers.length,
      foundries: groups.foundries.length,
      manufacturers: groups.manufacturers.length,
      refineries: groups.refineries.length,
      packagers: groups.packagers.length,
      coalGenerators: groups.coalGenerators.length,
      fuelGenerators: groups.fuelGenerators.length,
      biomassBurners: groups.biomassBurners.length,
      waterExtractors: groups.waterExtractors.length,
      oilExtractors: groups.oilExtractors.length,
    },
    miners: groups.miners.map(m => ({
      class: className(m),
      pos: m.transform?.translation,
      ...resolveExtractable(m),
    })),
    producers: ['smelters', 'foundries', 'constructors', 'assemblers', 'manufacturers', 'refineries', 'packagers', 'blenders', 'particleAccelerators'].reduce((acc, k) => {
      acc[k] = tally(groups[k], o => `${className(o)} :: ${resolveRecipe(o)}`).map(([key, count]) => {
        const [cls, recipe] = key.split(' :: ');
        return { class: cls, recipe, count };
      });
      return acc;
    }, {}),
    generators: {
      coal: tally(groups.coalGenerators, o => resolveFuel(o)).map(([fuel, count]) => ({ fuel, count })),
      fuel: tally(groups.fuelGenerators, o => resolveFuel(o)).map(([fuel, count]) => ({ fuel, count })),
      biomass: tally(groups.biomassBurners, o => resolveFuel(o)).map(([fuel, count]) => ({ fuel, count })),
      geothermal: groups.geothermalGenerators.length,
      nuclear: tally(groups.nuclearGenerators, o => resolveFuel(o)).map(([fuel, count]) => ({ fuel, count })),
    },
    waterExtractors: groups.waterExtractors.map(w => ({ class: className(w), pos: w.transform?.translation })),
    oilExtractors: groups.oilExtractors.map(o => ({ class: className(o), pos: o.transform?.translation, ...resolveExtractable(o) })),
    logistics: {
      beltsByTier: [...groups.beltsByTier.entries()].map(([tier, count]) => ({ tier, count })),
      liftsByTier: [...groups.liftsByTier.entries()].map(([tier, count]) => ({ tier, count })),
      pipesByTier: [...groups.pipesByTier.entries()].map(([tier, count]) => ({ tier, count })),
    },
  };
  console.log(JSON.stringify(out, null, 2));
  process.exit(0);
}

// Markdown output
const lines = [];
const push = (...l) => lines.push(...l);

push(`# Stocktake Sync — ${new Date().toISOString().slice(0, 10)}`);
push('');
push(`- **Save file:** \`${savePath}\``);
push(`- **Last saved:** ${saveStat.mtime.toISOString()}`);
push(`- **Session:** ${save.header.sessionName}`);
push(`- **Save / Build version:** ${save.header.saveVersion} / ${save.header.buildVersion}`);
push(`- **Parse time:** ${parseMs} ms — ${allObjects.length.toLocaleString()} total objects`);
push('');

push('## Miners');
push('');
const minerByClass = tally(groups.miners, o => className(o));
push('| Class | Count |');
push('| --- | ---: |');
for (const [cls, count] of minerByClass) push(`| ${cls} | ${count} |`);
push('');
push('| Class | Position | Resource | Node ref |');
push('| --- | --- | --- | --- |');
for (const m of groups.miners.sort((a, b) => className(a).localeCompare(className(b)))) {
  const ex = resolveExtractable(m);
  push(`| ${className(m)} | ${pos(m)} | ${ex.resource} | ${ex.ref} |`);
}
push('');

function pushProducerSection(title, items) {
  push(`## ${title}`);
  push('');
  if (items.length === 0) { push('_None._', ''); return; }
  push(`Total: **${items.length}**`);
  push('');
  push('| Class | Recipe | Count |');
  push('| --- | --- | ---: |');
  for (const [key, count] of tally(items, o => `${className(o)} :: ${resolveRecipe(o)}`)) {
    const [cls, recipe] = key.split(' :: ');
    push(`| ${cls} | ${recipe} | ${count} |`);
  }
  push('');
}

pushProducerSection('Smelters', groups.smelters);
pushProducerSection('Foundries', groups.foundries);
pushProducerSection('Constructors', groups.constructors);
pushProducerSection('Assemblers', groups.assemblers);
pushProducerSection('Manufacturers', groups.manufacturers);
pushProducerSection('Refineries', groups.refineries);
pushProducerSection('Packagers', groups.packagers);
pushProducerSection('Blenders', groups.blenders);
pushProducerSection('Particle accelerators', groups.particleAccelerators);

push('## Power generation');
push('');
function pushGenSection(label, items) {
  if (items.length === 0) return;
  const byFuel = tally(items, o => resolveFuel(o));
  push(`- **${label}:** ${items.length}`);
  for (const [fuel, count] of byFuel) push(`  - ${fuel}: ${count}`);
}
pushGenSection('Coal generators', groups.coalGenerators);
pushGenSection('Fuel generators', groups.fuelGenerators);
pushGenSection('Biomass burners', groups.biomassBurners);
pushGenSection('Nuclear power plants', groups.nuclearGenerators);
if (groups.geothermalGenerators.length) push(`- **Geothermal generators:** ${groups.geothermalGenerators.length}`);
push('');

push('## Fluid extractors');
push('');
push(`- **Water extractors:** ${groups.waterExtractors.length}`);
for (const w of groups.waterExtractors) push(`  - ${className(w)} @ ${pos(w)}`);
push(`- **Oil extractors:** ${groups.oilExtractors.length}`);
for (const o of groups.oilExtractors) {
  const ex = resolveExtractable(o);
  push(`  - ${className(o)} @ ${pos(o)} → ${ex.resource}`);
}
if (groups.resourceWellExtractors.length) {
  push(`- **Resource well extractors:** ${groups.resourceWellExtractors.length}`);
}
push('');

push('## Logistics');
push('');
push('| Type | Tier | Count |');
push('| --- | --- | ---: |');
for (const [tier, count] of [...groups.beltsByTier.entries()].sort()) push(`| Belt | ${tier} | ${count} |`);
for (const [tier, count] of [...groups.liftsByTier.entries()].sort()) push(`| Lift | ${tier} | ${count} |`);
for (const [tier, count] of [...groups.pipesByTier.entries()].sort()) push(`| Pipe | ${tier} | ${count} |`);
push('');

console.log(lines.join('\n'));

// ---------- helpers ----------

function findLatestSave(dir) {
  let entries;
  try { entries = readdirSync(dir); } catch { return null; }
  const sav = entries
    .filter(n => n.toLowerCase().endsWith('.sav'))
    .map(n => ({ name: n, path: join(dir, n), mtime: statSync(join(dir, n)).mtimeMs }))
    .sort((a, b) => b.mtime - a.mtime);
  return sav[0]?.path ?? null;
}
