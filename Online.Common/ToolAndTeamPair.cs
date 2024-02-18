using Common;
using Tools;

namespace OnlineChess.Common;

public class ToolAndTeamPair
{
    public ToolId ToolId { get; }

    public TeamId TeamId { get; }

    public ToolAndTeamPair(ToolId  toolId
                          , TeamId teamId)
    {
        ToolId = toolId;
        TeamId = teamId;
    }

    public override string ToString()
    {
        return $"{nameof(ToolId)}: {ToolId}, {nameof(TeamId)}: {TeamId}";
    }
}