# 后端架构设计（.NET 10 + DDD）

## 目标与约束
- 严格按原型功能范围实现，提供规则配置、执行与任务查询等能力。
- 采用 DDD 分层，提升可维护性与扩展性。

## 技术栈
- .NET 10、ASP.NET Core Web API、EF Core、Serilog、FluentValidation、Swashbuckle(OpenAPI)。

## 项目结构
- Server.Domain：实体、值对象、聚合、领域服务、仓储接口
- Server.Application：用例（命令/查询）、DTO、映射、验证、事务边界
- Server.Infrastructure：EF Core 映射、仓储实现、DbContext、外部连接器
- Server.WebApi：API 入口、路由、Swagger、DI 组合根

## 领域模型
- Source：Id、Name、Type、Connection、Status
- Target：Id、Name、Type、Connection、Status
- SchemaTable/SchemaColumn：表与字段结构
- Rule：Id、Name、SourceTable、TargetTable、Mappings、DedupeBy、MergeStrategies、Status
- Job：Id、RuleId、Trigger、StartTime、EndTime、Status、RecordCount、Message
- 值对象：TableId、Strategy(Update|Skip|UpdateWhenEmpty)
- 领域服务：DeduplicationService、MergeService、SyncPlanner、SchemaExplorer
- 仓储接口：ISourceRepository、ITargetRepository、IRuleRepository、IJobRepository、ISchemaRepository

## API 契约与端点
- Sources：GET/POST/PUT/DELETE /api/sources；POST /api/sources/test
- Targets：GET/POST/PUT/DELETE /api/targets；POST /api/targets/test
- Schema：GET /api/schema/tables?sourceId；GET /api/schema/columns?sourceId&table
- Rules：GET /api/rules、GET /api/rules/{id}、POST/PUT /api/rules、POST /api/rules/{id}/disable
- Execute：POST /api/execute?ruleId、GET /api/execute/{jobId}/progress、POST /api/execute/{jobId}/cancel
- Jobs：GET /api/jobs、GET /api/jobs/{id}

## 非功能性
- 日志：Serilog；统一异常：ProblemDetails；输入验证：FluentValidation
- 持久化：开发期可用内存仓储或 SQLite；生产期按部署选择
- 测试：Domain/Application 单元测试，WebAPI 集成测试

## 代码参考
- Web API 入口：[Program.cs](file:///Users/wangboy/MyWork/dataSync/server/Server.WebApi/Program.cs)
- 领域实体示例：[Rule.cs](file:///Users/wangboy/MyWork/dataSync/server/Server.Domain/Entities/Rule.cs)
- 应用 DTO 示例：[RuleDto.cs](file:///Users/wangboy/MyWork/dataSync/server/Server.Application/DTOs/RuleDto.cs)

