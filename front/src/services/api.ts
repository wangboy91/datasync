export const apiBase = import.meta.env.VITE_API_BASE || "/api";

export async function getSources() {
  const r = await fetch(`${apiBase}/sources`);
  return r.json();
}

export async function createSource(data: any) {
  const r = await fetch(`${apiBase}/sources`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });
  return r.json();
}

export async function updateSource(id: string, data: any) {
  const r = await fetch(`${apiBase}/sources/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });
  return r.json();
}

export async function testSource(data: any) {
  const r = await fetch(`${apiBase}/sources/test`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });
  return r.json();
}

export async function getTargets() {
  const r = await fetch(`${apiBase}/targets`);
  return r.json();
}

export async function createTarget(data: any) {
  const r = await fetch(`${apiBase}/targets`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });
  return r.json();
}

export async function updateTarget(id: string, data: any) {
  const r = await fetch(`${apiBase}/targets/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });
  return r.json();
}

export async function testTarget(data: any) {
  const r = await fetch(`${apiBase}/targets/test`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });
  return r.json();
}

export async function getRules() {
  const r = await fetch(`${apiBase}/rules`);
  return r.json();
}

export async function createRule(data: any) {
  const r = await fetch(`${apiBase}/rules`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });
  return r.json();
}

export async function updateRule(id: string, data: any) {
  const r = await fetch(`${apiBase}/rules/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });
  return r.json();
}

export async function getJobs() {
  const r = await fetch(`${apiBase}/jobs`);
  return r.json();
}

export async function syncSchema(params: { sourceId?: string, targetId?: string }) {
  const qs = new URLSearchParams();
  if (params.sourceId) qs.append("sourceId", params.sourceId);
  if (params.targetId) qs.append("targetId", params.targetId);
  
  const r = await fetch(`${apiBase}/schema/sync?${qs.toString()}`, { method: "POST" });
  if (!r.ok) throw new Error("Sync failed");
}

export async function syncTableSchema(params: { sourceId?: string, targetId?: string }, table: string) {
  const qs = new URLSearchParams();
  if (params.sourceId) qs.append("sourceId", params.sourceId);
  if (params.targetId) qs.append("targetId", params.targetId);
  qs.append("table", table);
  
  const r = await fetch(`${apiBase}/schema/sync-table?${qs.toString()}`, { method: "POST" });
  if (!r.ok) throw new Error("Sync table failed");
}

export async function getSchemaTables(params: { sourceId?: string, targetId?: string }) {
  const qs = new URLSearchParams();
  if (params.sourceId) qs.append("sourceId", params.sourceId);
  if (params.targetId) qs.append("targetId", params.targetId);
  
  const r = await fetch(`${apiBase}/schema/tables?${qs.toString()}`);
  return r.json();
}

export async function getSchemaColumns(params: { sourceId?: string, targetId?: string }, table: string) {
  const qs = new URLSearchParams();
  if (params.sourceId) qs.append("sourceId", params.sourceId);
  if (params.targetId) qs.append("targetId", params.targetId);
  qs.append("table", table);

  const r = await fetch(`${apiBase}/schema/columns?${qs.toString()}`);
  return r.json();
}

export async function execute(ruleId: string) {
  const r = await fetch(`${apiBase}/execute?ruleId=${ruleId}`, { method: "POST" });
  return r.json();
}

export async function getProgress(jobId: string) {
  const r = await fetch(`${apiBase}/execute/${jobId}/progress`);
  return r.json();
}
