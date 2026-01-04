<template>
  <div>
    <h2 class="page-title">仪表盘</h2>
    <el-row :gutter="20" class="mb-4">
      <el-col :span="8">
        <el-card shadow="hover">
          <template #header>
            <div class="card-header">
              <span>数据源</span>
            </div>
          </template>
          <div class="card-content">
            <span class="big-number">0</span>
          </div>
        </el-card>
      </el-col>
      <el-col :span="8">
        <el-card shadow="hover">
          <template #header>
            <div class="card-header">
              <span>目标库</span>
            </div>
          </template>
          <div class="card-content">
            <span class="big-number">0</span>
          </div>
        </el-card>
      </el-col>
      <el-col :span="8">
        <el-card shadow="hover">
          <template #header>
            <div class="card-header">
              <span>规则数量</span>
            </div>
          </template>
          <div class="card-content">
            <span class="big-number">0</span>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <el-card class="box-card">
      <template #header>
        <div class="clearfix">
          <span>最近任务</span>
        </div>
      </template>
      <el-table :data="tableData" style="width: 100%" stripe border>
        <el-table-column prop="rule" label="规则" />
        <el-table-column prop="trigger" label="触发" />
        <el-table-column prop="start" label="开始" />
        <el-table-column prop="end" label="结束" />
        <el-table-column prop="status" label="状态" />
        <el-table-column prop="records" label="记录数" />
      </el-table>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';

const tableData = ref([
  { rule: '-', trigger: '-', start: '-', end: '-', status: '-', records: 0 }
]);
</script>

<style scoped>
.page-title {
  margin-bottom: 20px;
  font-weight: 500;
}
.mb-4 {
  margin-bottom: 20px;
}
.card-header {
  font-weight: bold;
}
.big-number {
  font-size: 28px;
  font-weight: bold;
  color: #409EFF;
  text-align: center;
  display: block;
}
</style>
