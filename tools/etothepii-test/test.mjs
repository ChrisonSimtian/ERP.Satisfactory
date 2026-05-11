import { Parser } from '@etothepii/satisfactory-file-parser';
import { readFileSync } from 'fs';

const path = process.argv[2];
if (!path) { console.error('usage: node test.mjs <save.sav>'); process.exit(1); }

console.log(`Loading: ${path}`);
const nodeBuf = readFileSync(path);
const buf = nodeBuf.buffer.slice(nodeBuf.byteOffset, nodeBuf.byteOffset + nodeBuf.byteLength);
const t0 = Date.now();
const save = Parser.ParseSave('test', buf, undefined, { onProgressCallback: () => {} });
const elapsed = Date.now() - t0;
console.log(`Parsed in ${elapsed} ms`);
console.log(`Header version: ${save.header.headerVersion}, save version: ${save.header.saveVersion}, build: ${save.header.buildVersion}`);
console.log(`Session: ${save.header.sessionName}`);

console.log(`\nTop-level keys: ${Object.keys(save).join(', ')}`);
console.log(`save.levels type: ${typeof save.levels}, isArray: ${Array.isArray(save.levels)}, constructor: ${save.levels?.constructor?.name}`);
if (save.levels && typeof save.levels === 'object') {
  const lk = Object.keys(save.levels);
  console.log(`save.levels has ${lk.length} keys, first 5: ${lk.slice(0, 5).join(', ')}`);
  const first = save.levels[lk[0]];
  console.log(`first level keys: ${first ? Object.keys(first).join(', ') : 'null'}`);
}

// Iterate by Object.values
const levelEntries = save.levels ? Object.values(save.levels) : [];
console.log(`Iterating ${levelEntries.length} levels via Object.values\n`);

const counts = new Map();
const interesting = [];
for (const level of levelEntries) {
  for (const obj of level.objects ?? level.actors ?? []) {
    const cls = obj.typePath?.split('.').pop() ?? '(empty)';
    counts.set(cls, (counts.get(cls) ?? 0) + 1);
    if (cls.includes('Miner') || cls.includes('Smelter')) {
      interesting.push({ cls, pos: obj.transform?.translation });
    }
  }
}
const sorted = [...counts.entries()].sort((a,b) => b[1] - a[1]);
console.log(`\nTotal levels: ${save.levels.length}, top 20 classes:`);
for (const [name, count] of sorted.slice(0, 20)) console.log(`  ${count.toString().padStart(6)}  ${name}`);

console.log(`\nMiners + Smelters (${interesting.length}):`);
for (const a of interesting) {
  const p = a.pos ? `(${a.pos.x?.toFixed(0) ?? '?'}, ${a.pos.y?.toFixed(0) ?? '?'}, ${a.pos.z?.toFixed(0) ?? '?'})` : '(no pos)';
  console.log(`  ${a.cls.padEnd(30)}  ${p}`);
}
