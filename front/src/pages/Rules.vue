<template>
  <div class="rule-edit">
    <div class="page-header">
      <el-page-header @back="goBack" content="规则配置" />
    </div>

    <el-card>
      <el-form :model="form" label-width="100px">
        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item label="规则名称">
              <el-input v-model="form.name" placeholder="请输入规则名称" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="源数据库">
              <div class="db-select-container">
                <el-select v-model="form.sourceId" placeholder="选择源数据库" @change="onSourceChange" class="flex-select">
                  <el-option v-for="s in sources" :key="s.id" :label="s.name" :value="s.id" />
                </el-select>
                <el-button 
                  icon="Refresh" 
                  circle 
                  :disabled="!form.sourceId" 
                  :loading="refreshingSource"
                  @click="refreshSourceSchema" 
                  title="同步表结构"
                />
              </div>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="目标数据库">
              <div class="db-select-container">
                <el-select v-model="form.targetId" placeholder="选择目标数据库" @change="onTargetChange" class="flex-select">
                  <el-option v-for="t in targets" :key="t.id" :label="t.name" :value="t.id" />
                </el-select>
                <el-button 
                  icon="Refresh" 
                  circle 
                  :disabled="!form.targetId" 
                  :loading="refreshingTarget"
                  @click="refreshTargetSchema"
                  title="同步表结构"
                />
              </div>
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="源表">
              <el-select v-model="form.sourceTable" placeholder="选择源表" style="width: 100%" filterable @change="onSourceTableChange">
                <el-option v-for="t in sourceTables" :key="t" :label="t" :value="t" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="目标表">
              <el-select v-model="form.targetTable" placeholder="选择目标表" style="width: 100%" filterable @change="onTargetTableChange">
                <el-option v-for="t in targetTables" :key="t" :label="t" :value="t" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      
      <h3>字段映射</h3>
      <el-table :data="form.mappings" border style="width: 100%; margin-bottom: 20px">
        <el-table-column label="源字段">
          <template #default="scope">
            <el-select v-model="scope.row.source" placeholder="选择源字段" filterable allow-create default-first-option style="width: 100%">
              <el-option v-for="c in sourceColumns" :key="c.name" :label="c.name" :value="c.name">
                <span style="float: left">{{ c.name }}</span>
                <span style="float: right; color: #8492a6; font-size: 13px">{{ c.type }}</span>
              </el-option>
            </el-select>
          </template>
        </el-table-column>
        <el-table-column label="目标字段">
          <template #default="scope">
            <el-select v-model="scope.row.target" placeholder="选择目标字段" filterable allow-create default-first-option style="width: 100%">
              <el-option v-for="c in targetColumns" :key="c.name" :label="c.name" :value="c.name">
                <span style="float: left">{{ c.name }}</span>
                <span style="float: right; color: #8492a6; font-size: 13px">{{ c.type }}</span>
              </el-option>
            </el-select>
          </template>
        </el-table-column>
        <el-table-column label="转换">
          <template #default="scope">
            <el-select v-model="scope.row.transform" placeholder="选择转换" style="width: 100%">
              <el-option label="直接映射 (Direct)" value="Direct" />
              <el-option label="转大写 (UpperCase)" value="UpperCase" />
              <el-option label="转小写 (LowerCase)" value="LowerCase" />
              <el-option label="去空格 (Trim)" value="Trim" />
              <el-option label="默认值 (Default)" value="Default" />
            </el-select>
          </template>
        </el-table-column>
        <el-table-column label="默认值">
          <template #default="scope">
            <el-input v-model="scope.row.defaultValue" :disabled="scope.row.transform !== 'Default'" />
          </template>
        </el-table-column>
        <el-table-column label="重复判定" width="100" align="center">
          <template #default="scope">
            <el-checkbox v-model="scope.row.isDedupe" />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="80" align="center">
          <template #default="scope">
            <el-button type="danger" icon="Delete" circle size="small" @click="removeMapping(scope.$index)" />
          </template>
        </el-table-column>
      </el-table>
      
      <div style="margin-bottom: 20px; display: flex; gap: 10px;">
        <el-button class="add-btn" type="primary" plain style="flex: 1" @click="addMapping">
          <el-icon><Plus /></el-icon> 添加映射
        </el-button>
        <el-button type="success" plain @click="quickMap" style="width: 120px">
          <el-icon><MagicStick /></el-icon> 快速配置
        </el-button>
      </div>

      <h3>合并策略</h3>
      <el-table :data="form.strategies" border style="width: 100%; margin-bottom: 20px">
        <el-table-column label="目标字段">
          <template #default="scope">
            <el-select v-model="scope.row.field" placeholder="选择目标字段" filterable allow-create default-first-option style="width: 100%">
              <el-option v-for="c in targetColumns" :key="c.name" :label="c.name" :value="c.name" />
            </el-select>
          </template>
        </el-table-column>
        <el-table-column label="策略">
          <template #default="scope">
            <el-select v-model="scope.row.strategy" placeholder="选择策略" style="width: 100%">
              <el-option label="更新" value="update" />
              <el-option label="跳过" value="skip" />
              <el-option label="仅空时更新" value="updateIfEmpty" />
            </el-select>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="80" align="center">
          <template #default="scope">
            <el-button type="danger" icon="Delete" circle size="small" @click="removeStrategy(scope.$index)" />
          </template>
        </el-table-column>
      </el-table>
      <el-button class="add-btn" type="primary" plain style="width: 100%; margin-bottom: 20px" @click="addStrategy">
        <el-icon><Plus /></el-icon> 添加策略
      </el-button>

      <div class="actions">
        <el-button type="primary" @click="save">保存规则</el-button>
        <el-button @click="goBack">取消</el-button>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import { createRule, getSources, getTargets, syncSchema, getSchemaTables, getSchemaColumns } from "../services/api";
import { ElMessage } from 'element-plus';

const router = useRouter();
const form = ref({
  name: '',
  sourceId: '',
  targetId: '',
  sourceTable: '',
  targetTable: '',
  mappings: [] as any[],
  strategies: [] as any[]
});

const sources = ref<any[]>([]);
const targets = ref<any[]>([]);
const sourceTables = ref<string[]>([]);
const targetTables = ref<string[]>([]);
const sourceColumns = ref<any[]>([]);
const targetColumns = ref<any[]>([]);

const refreshingSource = ref(false);
const refreshingTarget = ref(false);

onMounted(async () => {
  try {
    const [s, t] = await Promise.all([getSources(), getTargets()]);
    sources.value = s;
    targets.value = t;
  } catch(e) {
    console.error(e);
  }
  
  if (form.value.mappings.length === 0) {
    addMapping();
  }
  if (form.value.strategies.length === 0) {
    addStrategy();
  }
});

const onSourceChange = async (id: string) => {
  form.value.sourceTable = '';
  sourceTables.value = [];
  sourceColumns.value = [];
  if (!id) return;

  try {
    // Try to get cached tables first
    const tables = await getSchemaTables({ sourceId: id });
    if (tables && tables.length > 0) {
      sourceTables.value = tables;
    } else {
      // If no tables, try to sync automatically
      await refreshSourceSchema();
    }
  } catch (e) {
    ElMessage.error('获取源表结构失败');
  }
};

const refreshSourceSchema = async () => {
  if (!form.value.sourceId) return;
  refreshingSource.value = true;
  try {
    await syncSchema({ sourceId: form.value.sourceId });
    sourceTables.value = await getSchemaTables({ sourceId: form.value.sourceId });
    ElMessage.success('源表结构同步成功');
  } catch (e) {
    ElMessage.error('同步源表结构失败');
  } finally {
    refreshingSource.value = false;
  }
};

const onTargetChange = async (id: string) => {
  form.value.targetTable = '';
  targetTables.value = [];
  targetColumns.value = [];
  if (!id) return;

  try {
    const tables = await getSchemaTables({ targetId: id });
    if (tables && tables.length > 0) {
      targetTables.value = tables;
    } else {
      await refreshTargetSchema();
    }
  } catch (e) {
    ElMessage.error('获取目标表结构失败');
  }
};

const refreshTargetSchema = async () => {
  if (!form.value.targetId) return;
  refreshingTarget.value = true;
  try {
    await syncSchema({ targetId: form.value.targetId });
    targetTables.value = await getSchemaTables({ targetId: form.value.targetId });
    ElMessage.success('目标表结构同步成功');
  } catch (e) {
    ElMessage.error('同步目标表结构失败');
  } finally {
    refreshingTarget.value = false;
  }
};

const onSourceTableChange = async (table: string) => {
  if (!form.value.sourceId || !table) return;
  try {
    const res = await getSchemaColumns({ sourceId: form.value.sourceId }, table);
    sourceColumns.value = res.columns || [];
  } catch (e) {
    console.error(e);
  }
};

const onTargetTableChange = async (table: string) => {
  if (!form.value.targetId || !table) return;
  try {
    const res = await getSchemaColumns({ targetId: form.value.targetId }, table);
    targetColumns.value = res.columns || [];
  } catch (e) {
    console.error(e);
  }
};

const addMapping = () => {
  form.value.mappings.push({source:'', target:'', transform:'Direct', defaultValue:'', isDedupe: false});
};

const quickMap = () => {
  if (sourceColumns.value.length === 0) {
    ElMessage.warning('请先选择源表并确保其有字段信息');
    return;
  }
  
  const newMappings = sourceColumns.value.map(sc => {
    const targetMatch = targetColumns.value.find(tc => tc.name.toLowerCase() === sc.name.toLowerCase());
    return {
      source: sc.name,
      target: targetMatch ? targetMatch.name : '',
      transform: 'Direct',
      defaultValue: '',
      isDedupe: false
    };
  });
  
  form.value.mappings = newMappings;
  ElMessage.success(`已快速生成 ${newMappings.length} 条映射关系`);
};

const removeMapping = (index: number) => {
  form.value.mappings.splice(index, 1);
};

const addStrategy = () => {
  form.value.strategies.push({field:'', strategy:'update'});
};

const removeStrategy = (index: number) => {
  form.value.strategies.splice(index, 1);
};

const goBack = () => {
  router.push('/rules-list');
};

const save = async () => {
  try {
    const payload = {
      ...form.value,
      dedupeBy: form.value.mappings.filter(m => m.isDedupe).map(m => m.source),
      mergeStrategies: form.value.strategies.map(s => ({ targetField: s.field, strategy: s.strategy }))
    };
    await createRule(payload);
    ElMessage.success('保存成功');
    goBack();
  } catch (e) {
    console.error(e);
    ElMessage.error('保存失败');
  }
};
</script>

<style scoped>
.page-header {
  margin-bottom: 20px;
}
.actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}
h3 {
  margin: 20px 0 10px;
  border-left: 4px solid #409EFF;
  padding-left: 10px;
}
.add-btn {
  border-style: dashed;
}
.db-select-container {
  display: flex;
  gap: 10px;
  align-items: center;
  width: 100%;
}
.flex-select {
  flex: 1;
  width: 100%;
}
</style>
