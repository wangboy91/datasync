# DataSync

DataSync 是一个现代化的多数据库表结构同步与数据迁移工具。它旨在简化不同数据库之间的数据同步流程，提供灵活的规则配置和可视化的操作界面。

## ✨ 特性

- **多数据库支持**：
  - MySQL
  - PostgreSQL
  - SQL Server
  - 更多数据库支持正在开发中...
- **可视化配置**：直观的 Web 界面，轻松管理源数据库和目标数据库连接。
- **灵活的同步规则**：
  - 自定义字段映射
  - 数据转换规则
  - 增量/全量同步策略
- **表结构同步**：自动检测并同步源表与目标表的结构差异。
- **任务管理**：创建、监控和管理数据同步任务。

## 🛠 技术栈

### 前端 (Front)
- **框架**: [Vue 3](https://vuejs.org/)
- **构建工具**: [Vite](https://vitejs.dev/)
- **语言**: TypeScript
- **UI 组件**: (根据实际使用的组件库填写，如 Element Plus / Ant Design Vue)

### 后端 (Server)
- **框架**: [.NET 9](https://dotnet.microsoft.com/)
- **ORM**: Entity Framework Core
- **API**: ASP.NET Core Web API

## 🚀 快速开始

### 前提条件
- Node.js (v18+)
- .NET 9 SDK
- 相应的数据库服务 (MySQL/PostgreSQL/SQL Server)

### 运行后端服务
```bash
cd server
dotnet restore
dotnet run --project DataSync.WebApi
```
后端服务默认运行在 `http://localhost:5000` (具体端口请查看 `launchSettings.json`)。

### 运行前端项目
```bash
cd front
npm install
npm run dev
```
前端开发服务器将启动，通常访问 `http://localhost:5173`。

## 🤝 贡献

欢迎提交 Issue 和 Pull Request！如果您有任何建议或发现了 Bug，请随时反馈。

## 📄 许可证

本项目采用 [MIT 许可证](LICENSE) 开源。
