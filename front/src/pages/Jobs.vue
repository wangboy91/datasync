<template>
  <section>
    <h2>任务</h2>
    <table>
      <thead>
        <tr>
          <th>规则</th><th>触发</th><th>开始</th><th>结束</th><th>状态</th><th>记录数</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="j in items" :key="j.id">
          <td>{{ j.ruleId }}</td><td>{{ j.trigger }}</td><td>{{ j.startTime }}</td><td>{{ j.endTime }}</td><td>{{ j.status }}</td><td>{{ j.recordCount }}</td>
        </tr>
      </tbody>
    </table>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import { getJobs } from "../services/api";
const items = ref<any[]>([]);
onMounted(async () => {
  items.value = await getJobs();
});
</script>
