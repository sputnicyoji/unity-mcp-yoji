# Bundled Python Server

这是 `com.yoji.unity-mcp` 随包携带的 Python MCP server。

它由 Unity Editor 包通过 `uvx --from <package>/Server~ yoji-unity-mcp ...` 启动。

隐私约束：

- telemetry 代码已移除。
- analytics endpoint 已移除。
- 不写入 customer UUID。
- 不向外部服务发送使用数据。

仅保留本地 Unity Editor 通信和 MCP transport。
