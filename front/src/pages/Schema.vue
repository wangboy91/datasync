<template>
  <div class="schema-page">
    <div class="schema-layout">
      <!-- Left Panel: Selection -->
      <div class="left-panel">
        <h2 class="page-title">表结构查看</h2>
        
        <div class="selection-controls">
          <el-radio-group v-model="dbType" @change="onDbTypeChange" style="margin-bottom: 15px; width: 100%">
            <el-radio-button label="source">数据源</el-radio-button>
            <el-radio-button label="target">目标库</el-radio-button>
          </el-radio-group>

          <el-select 
            v-model="selectedDbId" 
            placeholder="选择数据库" 
            @change="onDbChange" 
            style="width: 100%; margin-bottom: 15px"
          >
            <el-option v-for="db in dbList" :key="db.id" :label="db.name" :value="db.id" />
          </el-select>

          <div v-if="selectedDbId" class="table-list-container">
            <div class="list-header">表列表</div>
            <el-input v-model="tableFilter" placeholder="搜索表..." prefix-icon="Search" style="margin-bottom: 10px" />
            <div class="table-list">
              <div 
                v-for="t in filteredTables" 
                :key="t" 
                class="table-item" 
                :class="{ active: selectedTable === t }"
                @click="onTableSelect(t)"
              >
                {{ t }}
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Right Panel: Details -->
      <div class="right-panel">
        <div v-if="selectedTable" class="table-details">
          <div class="details-header">
            <h3>表结构: {{ selectedTable }}</h3>
            <el-button type="primary" icon="Refresh" @click="updateTableSchema" :loading="updating">
              更新表结构
            </el-button>
          </div>
          
          <el-table :data="columns" style="width: 100%" stripe border v-loading="loadingColumns">
            <el-table-column prop="name" label="字段名" />
            <el-table-column prop="type" label="类型" />
            <el-table-column prop="nullable" label="可空">
              <template #default="scope">
                <el-tag :type="scope.row.nullable ? 'info' : 'danger'">{{ scope.row.nullable ? '是' : '否' }}</el-tag>
              </template>
            </el-table-column>
          </el-table>
        </div>
        <div v-else class="empty-state">
          <el-empty description="请选择左侧表查看详情" />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from "vue";
import { ElMessage } from "element-plus";
import { getSources, getTargets, getSchemaTables, getSchemaColumns, syncTableSchema } from "../services/api";

const dbType = ref<'source' | 'target'>('source');
const dbList = ref<any[]>([]);
const selectedDbId = ref<string>("");
const tables = ref<string[]>([]);
const tableFilter = ref("");
const selectedTable = ref<string>("");
const columns = ref<any[]>([]);
const loadingColumns = ref(false);
const updating = ref(false);

const filteredTables = computed(() => {
  if (!tableFilter.value) return tables.value;
  return tables.value.filter(t => t.toLowerCase().includes(tableFilter.value.toLowerCase()));
});

onMounted(async () => {
  await loadDbList();
});

const loadDbList = async () => {
  try {
    if (dbType.value === 'source') {
      dbList.value = await getSources();
    } else {
      dbList.value = await getTargets();
    }
    // Reset selection if current selection is not in new list (or simply reset always)
    selectedDbId.value = "";
    tables.value = [];
    selectedTable.value = "";
    columns.value = [];
  } catch (e) {
    console.error(e);
  }
};

const onDbTypeChange = () => {
  loadDbList();
};

const onDbChange = async () => {
  tables.value = [];
  selectedTable.value = "";
  columns.value = [];
  if (selectedDbId.value) {
    try {
      const params = dbType.value === 'source' ? { sourceId: selectedDbId.value } : { targetId: selectedDbId.value };
      tables.value = await getSchemaTables(params);
    } catch(e) {
      console.error(e);
      ElMessage.error('获取表列表失败');
    }
  }
};

const onTableSelect = async (table: string) => {
  selectedTable.value = table;
  await loadColumns();
};

const loadColumns = async () => {
  if (!selectedDbId.value || !selectedTable.value) return;
  
  loadingColumns.value = true;
  try {
    const params = dbType.value === 'source' ? { sourceId: selectedDbId.value } : { targetId: selectedDbId.value };
    const data = await getSchemaColumns(params, selectedTable.value);
    columns.value = data.columns || [];
  } catch(e) {
    console.error(e);
    ElMessage.error('获取表结构失败');
  } finally {
    loadingColumns.value = false;
  }
};

const updateTableSchema = async () => {
  if (!selectedDbId.value || !selectedTable.value) return;

  updating.value = true;
  try {
    const params = dbType.value === 'source' ? { sourceId: selectedDbId.value } : { targetId: selectedDbId.value };
    await syncTableSchema(params, selectedTable.value);
    ElMessage.success('表结构更新成功');
    await loadColumns(); // Reload columns to show latest state
  } catch (e) {
    console.error(e);
    ElMessage.error('更新表结构失败');
  } finally {
    updating.value = false;
  }
};
</script>

<style scoped>
.schema-page {
  height: calc(100vh - 120px); /* Adjust to ensure it fits within the viewport */
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.page-title {
  margin: 0 0 20px 0;
  font-size: 18px;
  flex-shrink: 0;
}

.schema-layout {
  display: flex;
  flex: 1;
  gap: 20px;
  overflow: hidden;
}

.left-panel {
  width: 300px;
  background-color: #fff;
  border-right: 1px solid #e0e0e0;
  padding: 20px;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.right-panel {
  flex: 1;
  background-color: #fff;
  padding: 20px;
  overflow-y: auto;
}

.selection-controls {
  display: flex;
  flex-direction: column;
  flex: 1;
  overflow: hidden;
}

.table-list-container {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  min-height: 0;
}

.list-header {
  font-weight: bold;
  margin-bottom: 10px;
  color: #606266;
  flex-shrink: 0;
}

.table-list {
  flex: 1;
  overflow-y: auto;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
}

.table-item {
  padding: 10px;
  cursor: pointer;
  border-bottom: 1px solid #ebeef5;
  transition: background-color 0.2s;
}

.table-item:hover {
  background-color: #f5f7fa;
}

.table-item.active {
  background-color: #ecf5ff;
  color: #409eff;
}

.details-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.details-header h3 {
  margin: 0;
}

.empty-state {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%;
}
</style>
