# Bundled Python Server

这是 `com.yoji.unity-mcp` 随包携带的 Python MCP server。

它由 Unity Editor 包通过 `uvx --from <package>/Server~ yoji-unity-mcp ...` 启动。

隐私约束：

- telemetry 代码已移除。
- analytics endpoint 已移除。
- 不写入 customer UUID。
- 不向外部服务发送使用数据。

默认只保留本地 Unity Editor 通信和 MCP transport。

边界：

- `unity_docs` 等工具在用户显式调用时会访问 Unity 官方文档。
- HTTP Remote / API key validation 只在用户显式配置远端 URL 时访问对应服务。
