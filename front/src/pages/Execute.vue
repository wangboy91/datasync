<template>
  <section>
    <h2>执行</h2>
    <div style="display:flex; gap:8px; margin:12px 0;">
      <select v-model="ruleId">
        <option value="">选择规则</option>
        <option v-for="r in rules" :key="r.id" :value="r.id">{{ r.name }}</option>
      </select>
      <button @click="run">运行同步</button>
    </div>
    <div style="border:1px solid #ddd; border-radius:6px; padding:8px;">
      <div>进度</div>
      <div style="height:12px; background:#eee; border-radius:6px; overflow:hidden;">
        <div :style="{width: percent + '%', height:'12px', background:'#5b9bd5'}"></div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import { getRules, execute, getProgress } from "../services/api";
const rules = ref<any[]>([]);
const ruleId = ref<string>("");
const percent = ref<number>(0);
const jobId = ref<string>("");
onMounted(async () => {
  rules.value = await getRules();
});
const run = async () => {
  if (!ruleId.value) return;
  const r = await execute(ruleId.value);
  jobId.value = r.jobId;
  const p = await getProgress(jobId.value);
  percent.value = p.percent || 0;
};
</script>
