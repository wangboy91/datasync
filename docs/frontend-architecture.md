# 前端架构设计（Vue 3）

## 目标与约束
- 1:1 复原原型的页面结构、文案与交互，允许轻微样式美化。

## 技术栈
- Vue 3、Vite、Vue Router、Pinia、Fetch/Axios。

## 目录结构
- src/pages：Dashboard、Sources、Targets、Schema、RulesList、Rules、Execute、Jobs、Settings
- src/router：index.ts（与原型导航一致）
- src/services：api.ts（封装 /api 端点）
- src/App.vue：导航与布局；src/main.ts：应用入口

## 页面要点
- 规则配置：字段映射表包含“源字段、目标字段、转换、默认值、重复判定”；合并策略表包含“目标字段、策略（更新/跳过/仅空时更新）”
- 列名与按钮文案与原型一致
- 数据通过 stores/services 调用后端 API

## 接口集成
- Sources/Targets：列表展示，后续扩展新增、编辑、测试连接
- Schema：数据源与表选择，字段列表展示
- RulesList/Rules：规则列表与配置读写
- Execute/Jobs：运行同步与任务进度展示

## 原型参考
- 规则页：[rules.html](file:///Users/wangboy/MyWork/dataSync/prototype-design/pages/rules.html)
- 规则列表：[rules-list.html](file:///Users/wangboy/MyWork/dataSync/prototype-design/pages/rules-list.html)
- 执行页：[execute.html](file:///Users/wangboy/MyWork/dataSync/prototype-design/pages/execute.html)
- 任务页：[jobs.html](file:///Users/wangboy/MyWork/dataSync/prototype-design/pages/jobs.html)
- 表结构：[schema.html](file:///Users/wangboy/MyWork/dataSync/prototype-design/pages/schema.html)
- 数据源：[sources.html](file:///Users/wangboy/MyWork/dataSync/prototype-design/pages/sources.html)
- 目标库：[targets.html](file:///Users/wangboy/MyWork/dataSync/prototype-design/pages/targets.html)
- 仪表盘：[index.html](file:///Users/wangboy/MyWork/dataSync/prototype-design/pages/index.html)

