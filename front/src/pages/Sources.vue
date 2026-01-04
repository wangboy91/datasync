<template>
  <div>
    <div class="header-actions">
      <h2>数据源</h2>
      <div class="header-btns">
        <el-button type="primary" icon="Plus" @click="openDialog()">新增</el-button>
        <el-button 
          icon="Connection" 
          :disabled="!currentRow" 
          @click="testConnection(currentRow)" 
          :loading="testing"
        >
          测试连接
        </el-button>
        <el-button 
          icon="Refresh" 
          :disabled="!currentRow" 
          @click="syncDbSchema(currentRow)" 
          :loading="currentRow?.syncing"
        >
          同步表结构
        </el-button>
      </div>
    </div>
    <el-card>
      <el-table 
        :data="items" 
        style="width: 100%" 
        stripe 
        highlight-current-row
        @current-change="handleCurrentChange"
      >
        <el-table-column prop="name" label="名称" />
        <el-table-column prop="type" label="类型" />
        <el-table-column prop="connection" label="连接信息" />
        <el-table-column prop="status" label="状态">
          <template #default="scope">
             <el-tag :type="scope.row.status === 'connected' ? 'success' : 'danger'">{{ scope.row.status }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="100">
          <template #default="scope">
            <el-button size="small" icon="Edit" @click="openDialog(scope.row)">编辑</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <el-dialog v-model="dialogVisible" :title="isEdit ? '编辑数据源' : '新增数据源'" width="500px">
      <el-form :model="form" label-width="100px">
        <el-form-item label="名称">
          <el-input v-model="form.name" />
        </el-form-item>
        <el-form-item label="类型">
          <el-select v-model="form.type" placeholder="选择类型">
            <el-option label="PostgreSQL" value="postgres" />
            <el-option label="MySQL" value="mysql" />
            <el-option label="SQL Server" value="sqlserver" />
          </el-select>
        </el-form-item>
        <el-form-item label="连接信息">
          <el-input 
            v-model="form.connection" 
            type="textarea" 
            :placeholder="connectionPlaceholder" 
            :rows="3"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="testConnection(form)" :loading="testing">测试连接</el-button>
          <el-button @click="dialogVisible = false">取消</el-button>
          <el-button type="primary" @click="save">保存</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, watch } from "vue";
import { ElMessage } from "element-plus";
import { getSources, createSource, updateSource, testSource, syncSchema } from "../services/api";

const items = ref<any[]>([]);
const dialogVisible = ref(false);
const isEdit = ref(false);
const testing = ref(false);
const form = ref<any>({ name: "", type: "", connection: "" });
const currentRow = ref<any>(null);

const connectionPlaceholder = computed(() => {
  switch (form.value.type) {
    case 'postgres':
      return 'Host=localhost;Port=5432;Database=mydb;Username=myuser;Password=mypass';
    case 'mysql':
      return 'Server=localhost;Port=3306;Database=mydb;Uid=myuser;Pwd=mypass;';
    case 'sqlserver':
      return 'Server=localhost,1433;Database=mydb;User Id=myuser;Password=mypass;';
    default:
      return '请输入连接字符串';
  }
});

watch(() => form.value.type, (newType) => {
  if (!form.value.connection) {
    switch (newType) {
      case 'postgres':
        form.value.connection = 'Host=localhost;Port=5432;Database=mydb;Username=myuser;Password=mypass';
        break;
      case 'mysql':
        form.value.connection = 'Server=localhost;Port=3306;Database=mydb;Uid=myuser;Pwd=mypass;';
        break;
      case 'sqlserver':
        form.value.connection = 'Server=localhost,1433;Database=mydb;User Id=myuser;Password=mypass;';
        break;
    }
  }
});

const loadData = async () => {
  items.value = await getSources();
};

onMounted(loadData);

const handleCurrentChange = (val: any) => {
  currentRow.value = val;
};

const openDialog = (row?: any) => {
  if (row) {
    isEdit.value = true;
    form.value = { ...row };
  } else {
    isEdit.value = false;
    form.value = { name: "", type: "", connection: "" };
  }
  dialogVisible.value = true;
};

const testConnection = async (data: any) => {
  if (!data || !data.type || !data.connection) {
    ElMessage.warning('请先填写类型和连接信息');
    return;
  }
  testing.value = true;
  try {
    const res = await testSource(data);
    if (res.ok) {
      ElMessage.success('连接成功');
    } else {
      ElMessage.error('连接失败');
    }
  } catch (e) {
    ElMessage.error('连接测试发生错误');
    console.error(e);
  } finally {
    testing.value = false;
  }
};

const syncDbSchema = async (row: any) => {
  if (!row) return;
  row.syncing = true;
  try {
    await syncSchema({ sourceId: row.id });
    ElMessage.success('表结构同步成功');
  } catch (e) {
    ElMessage.error('表结构同步失败');
    console.error(e);
  } finally {
    row.syncing = false;
  }
};

const save = async () => {
  try {
    if (isEdit.value) {
      await updateSource(form.value.id, form.value);
    } else {
      await createSource({ ...form.value, id: crypto.randomUUID(), status: 'disconnected' });
    }
    dialogVisible.value = false;
    await loadData();
  } catch (e) {
    console.error(e);
  }
};
</script>

<style scoped>
.header-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}
.header-btns {
  display: flex;
  gap: 10px;
}
h2 {
  margin: 0;
}
</style>
