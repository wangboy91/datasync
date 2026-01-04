<template>
  <div class="rules-list">
    <div class="header-actions">
      <h2>规则列表</h2>
      <el-button type="primary" @click="$router.push('/rules')">
        <el-icon><Plus /></el-icon> 新建规则
      </el-button>
    </div>
    
    <el-card>
      <el-table :data="items" style="width: 100%" stripe>
        <el-table-column prop="name" label="名称" width="180" />
        <el-table-column prop="sourceTable" label="源表" />
        <el-table-column prop="targetTable" label="目标表" />
        <el-table-column label="去重字段数" width="120">
          <template #default="scope">
            {{ scope.row.dedupeBy?.length || 0 }}
          </template>
        </el-table-column>
        <el-table-column label="合并策略数" width="120">
          <template #default="scope">
            {{ scope.row.mergeStrategies?.length || 0 }}
          </template>
        </el-table-column>
        <el-table-column prop="status" label="状态" width="100">
          <template #default="scope">
            <el-tag :type="scope.row.status === 'active' ? 'success' : 'info'">{{ scope.row.status || 'unknown' }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="150">
          <template #default="scope">
            <el-button size="small" @click="viewRule(scope.row.id)">查看</el-button>
            <el-button size="small" type="primary" @click="editRule(scope.row.id)">编辑</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import { getRules } from "../services/api";

const router = useRouter();
const items = ref<any[]>([]);

onMounted(async () => {
  try {
    items.value = await getRules();
  } catch (e) {
    console.error(e);
  }
});

const viewRule = (id: string) => {
  console.log('view', id);
};

const editRule = (id: string) => {
  router.push('/rules');
};
</script>

<style scoped>
.header-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}
.header-actions h2 {
  margin: 0;
}
</style>
